using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField]
        private Vector2 newNodeOffset = new Vector2(200, 0);

        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR

        private void Reset()
        {
            if (nodes.Count == 0)
            {
                DialogueNode newNode = CreateInstance<DialogueNode>();
                newNode.SetUID(Guid.NewGuid().ToString());
                nodes.Add(newNode);
                PopulateNodeLookup();
            }
        }

        public Rect CalculateNeededWindowSize()
        {
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            foreach (var dialogueNode in GetAllNodes())
            {
                rect.xMax = Mathf.Max(rect.xMax, dialogueNode.GetRect().xMax);
                rect.yMax = Mathf.Max(rect.yMax, dialogueNode.GetRect().yMax);
            }
            rect.xMax += 200;
            rect.yMax += 200;
            return rect;
        }

#endif

        private void Awake()
        {
            PopulateNodeLookup();
        }

        private void OnValidate()
        {
            PopulateNodeLookup();
        }

        private void PopulateNodeLookup()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.GetUID] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childNode in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childNode))
                {
                    result.Add(nodeLookup[childNode]);
                }
            }
            return result;
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (DialogueNode node in GetAllChildren(parentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (DialogueNode node in GetAllChildren(parentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

#if UNITY_EDITOR

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                Undo.RecordObject(this, "Added Dialogue Node");
            }
            nodes.Add(newNode);
            PopulateNodeLookup();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            PopulateNodeLookup();
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.GetUID);
            }
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.SetUID(Guid.NewGuid().ToString());
            if (parentNode != null)
            {
                parentNode.AddChild(newNode.GetUID);
                newNode.SetPlayerSpeaking(!parentNode.IsPlayerSpeaking());
                newNode.SetPosition(parentNode.GetRect().position + newNodeOffset);
            }
            return newNode;
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (node != null && AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}