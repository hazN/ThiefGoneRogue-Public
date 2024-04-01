using RPG.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private int priority;
        [SerializeField] private string overwriteName = "";
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private Condition condition;
        public string GetText => text;

        public string GetUID => name;
        public int GetPriority() => priority;
        public bool IsPlayerSpeaking() => isPlayerSpeaking;
        public void SetUID(string UID) => this.name = UID;

        public Rect GetRect() => rect;

        public void SetRect(Rect newRect)
        {
            rect = newRect;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }
        public string GetOnExitAction()
        {
            return onExitAction;
        }
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }
        public string GetOverwriteName()
        {
            return overwriteName;
        }

#if UNITY_EDITOR
        public void SetPlayerSpeaking(bool isPlayerSpeaking)
        {
            Undo.RecordObject(this, "Update Dialogue Speaker");
            this.isPlayerSpeaking = isPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
        public void SetText(string text)
        {
            if (text == this.text) return;
            Undo.RecordObject(this, "Update Dialogue Text");
            this.text = text;

            EditorUtility.SetDirty(this);
        }
        public void SetPosition(Vector2 position)
        {
            if (rect.position == position) return;
            GUI.changed = true;
            Undo.RecordObject(this, "Moved Dialogue Node");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string childUID)
        {
            GUI.changed = true;
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childUID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childUID)
        {
            GUI.changed = true;
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childUID);
            EditorUtility.SetDirty(this);
        }

#endif
    }
}