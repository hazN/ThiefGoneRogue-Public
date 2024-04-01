using RPG.Attributes;
using RPG.Control;
using System;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] private float dialogueRadius = 10f;
        [SerializeField] private string conversantName = "NPC";
        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public float GetRadius()
        {
            return dialogueRadius;
        }

        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (dialogue == null)
            {
                return false;
            }
            if (GetComponent<Health>() && GetComponent<Health>().IsDead())
            {
                return false;
            }
            if (mouseClicked)
            {
                playerController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }
        public Transform GetTransform()
        {
            return transform;
        }

        public string GetName()
        {
           return conversantName;
        }
    }
}