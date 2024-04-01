using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        [NonSerialized] private GUIStyle nodeStyle = null;
        [NonSerialized] private GUIStyle playerNodeStyle = null;
        [NonSerialized] private DialogueNode selectedNode = null;
        [NonSerialized] private Vector2 draggingOffset = Vector2.zero;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingParentNode = null;
        Vector2 scrollPosition = Vector2.zero;
        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset = Vector2.zero;
        const float canvasSize = 4000;
        const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue dialogue)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.fontStyle = FontStyle.Bold;
            nodeStyle.alignment = TextAnchor.MiddleLeft;
            nodeStyle.fontSize = 13;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.fontStyle = FontStyle.Bold;
            playerNodeStyle.alignment = TextAnchor.MiddleLeft;
            playerNodeStyle.fontSize = 13;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue)
            {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!selectedDialogue)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Rect bounds = selectedDialogue.CalculateNeededWindowSize();
                Rect canvas = GUILayoutUtility.GetRect(bounds.width, bounds.height);
                Texture2D backgroundTex = Resources.Load("editorbackground") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvas.xMax / backgroundSize, canvas.yMax / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();
                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
            string content = "Dialogue Editor";
            if (selectedDialogue)
            {
                content = selectedDialogue.name + (EditorUtility.IsDirty(selectedDialogue) ? " *" : "");
            }
            titleContent = new GUIContent(content);
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && selectedNode == null)
            {
                selectedNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (selectedNode != null)
                {
                    draggingOffset = selectedNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = selectedNode;
                    GUI.changed = true;
                    GUI.FocusControl(null);
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && selectedNode != null)
            {
                selectedNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
                GUI.FocusControl(null);
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
                GUI.FocusControl(null);
            }
            else if (Event.current.type == EventType.MouseUp && selectedNode != null)
            {
                selectedNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            var areaStyle = new GUIStyle(GUI.skin.textArea);
            areaStyle.wordWrap = true;
            areaStyle.CalcHeight(new GUIContent(node.GetText), node.GetRect().width);

            Rect rect = node.GetRect();
            rect.height = areaStyle.CalcHeight(new GUIContent(node.GetText), node.GetRect().width / 1.6f);
            // Make sure height is at least 100
            rect.height = Mathf.Max(rect.height, 100f);
            GUILayout.BeginArea(rect, node.IsPlayerSpeaking() ? playerNodeStyle : nodeStyle);


            node.SetText(EditorGUILayout.TextArea(node.GetText, areaStyle));

            GUILayout.BeginHorizontal();
            Color defaultGUIColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            if (linkingParentNode == null)
            {
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.GetUID))
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChild(node.GetUID);
                    linkingParentNode = null;
                }
            }
            else
            {
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChild(node.GetUID);
                    linkingParentNode = null;
                }
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            GUI.backgroundColor = defaultGUIColor;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax - 6f, node.GetRect().center.y);

            Texture2D arrowIcon = (Texture2D)EditorGUIUtility.Load("Assets/Editor Default Resources/arrow-icon.png");

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                if (childNode == null) continue;
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin - 8f, childNode.GetRect().center.y);

                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.6f;

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 6f);

                if (arrowIcon)
                {
                    GUI.DrawTexture(new Rect(endPosition.x, endPosition.y - 8, 16, 16), arrowIcon, ScaleMode.StretchToFill);
                }
            }
        }
    }
}