using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using UnityEngine.InputSystem;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        [SerializeField] private float combatRadius = 30f;
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public float GetRadius()
        {
            return combatRadius;
        }

        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (!enabled) return false;
            // Return false if this cant be attacked
            if (!playerController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            // If mouse clicked then attack
            if (mouseClicked)
            {
                playerController.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }
        public Transform GetTransform()
        {
            return transform;
        }

    }
}