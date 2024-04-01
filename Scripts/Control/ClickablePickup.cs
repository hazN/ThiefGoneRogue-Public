using RPG.Core;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;
        [SerializeField] private float pickupRadius = 3f;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }
        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (mouseClicked)
            {
                pickup.PickupItem();
            }
            return true;
        }

        public float GetRadius()
        {
            return pickupRadius;
        }
        public Transform GetTransform()
        {
            return transform;
        }
    }
}