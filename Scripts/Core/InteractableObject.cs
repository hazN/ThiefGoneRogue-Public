using RPG.Control;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    public class InteractableObject : MonoBehaviour, IRaycastable
    {
        [SerializeField] private float radius = 2f;
        [SerializeField] private bool oneTimeUse = true;
        public UnityEvent interactEvent;
        private bool hasBeenUsed = false;
        public CursorType GetCursorType()
        {
            if (hasBeenUsed)
            {
                return CursorType.None;
            }
            return CursorType.Hand;
        }

        public float GetRadius()
        {
            return radius;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (mouseClicked && !hasBeenUsed)
            {
                interactEvent.Invoke();
                if (oneTimeUse)
                {
                    hasBeenUsed = true;
                }
            }
            return true;
        }
    }
}