using RPG.Attributes;
using RPG.Inventories;
using RPG.Movement;
using RPG.UI;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        public struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMapping;

        // Differiantion between mouse click and mouse hold
        // is because we want to move on click or hold but only attack on click
        private bool mouseHeld = false;

        private bool mouseClicked = false;

        private bool isDraggingUI = false;

        [SerializeField] private float clickRadius = 1f;
        [SerializeField] private MoveEffectSpawner moveEffectSpawner;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private LayerMask excludedLayers;
        [SerializeField] private int numberOfAbilities;
        private IRaycastable curRaycastable = null;
        private RaycastHit hit;
        private Mover mover;
        private Health health;
        private ActionStore actionStore;

        public void LeftMouseHold(InputAction.CallbackContext obj)
        {
            // Started = click
            if (obj.started)
            {
                curRaycastable = null;
                mouseHeld = false;
                mouseClicked = true;
                if (EventSystem.current.IsPointerOverGameObject())
                    isDraggingUI = true;
            }
            // Performed = hold
            if (obj.performed)
            {
                mouseHeld = true;
                mouseClicked = false;
                if (EventSystem.current.IsPointerOverGameObject())
                    isDraggingUI = true;
            }
            // Canceled = release
            if (obj.canceled)
            {
                mouseHeld = false;
                mouseClicked = false;
                isDraggingUI = false;
            }
        }

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            actionStore = GetComponent<ActionStore>();
        }

        private void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (isDraggingUI) return;
            UseAbilities();
            if (CheckCurrentComponent()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private void UseAbilities()
        {
            for (int i = 0; i < numberOfAbilities; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    actionStore.Use(i, gameObject);
                }
            }
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool CheckCurrentComponent()
        {
            if (curRaycastable == null) return false;
            float distanceToCur = Vector3.Distance(transform.position, curRaycastable.GetTransform().position);
            if (distanceToCur <= curRaycastable.GetRadius())
            {
                mover.Cancel();
                if (curRaycastable.HandleRaycast(this, true, mouseHeld))
                {
                    curRaycastable = null;
                    return true;
                }
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            // Raycast and check for hit
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    // Check if the player is outside the radius of the raycastable
                    // if so then move to the raycastable
                    if (Vector3.Distance(transform.position, raycastable.GetTransform().position) > raycastable.GetRadius())
                    {
                        if (mouseClicked)
                        {
                            // Set the current raycastable to the one we are moving to
                            curRaycastable = raycastable;
                            GetComponent<Mover>().StartMoveAction(hit.transform.position, 1f);
                        }
                        SetCursor(raycastable.GetCursorType());
                        if (mouseHeld)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        if (raycastable.HandleRaycast(this, mouseClicked, mouseHeld))
                        {
                            curRaycastable = null;
                            SetCursor(raycastable.GetCursorType());
                            if (mouseHeld)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Returns false if raycast fails, true otherwise
        private bool InteractWithMovement()
        {
            // Get all layers except whats in the layerMask
            LayerMask includedLayers = ~excludedLayers;
            Debug.DrawRay(GetMouseRay().origin, GetMouseRay().direction * 100, Color.red);
            // If mouse is clicked or held then move to the hit point
            if (mouseClicked)
            {
                // Raycast and check for hit
                Vector3 target;
                bool hasHit = RaycastNavMesh(out target);
                if (hasHit)
                {
                    if (!mover.CanMoveTo(target))
                    {
                        return false;
                    }
                    mover.StartMoveAction(target, 1f);
                    moveEffectSpawner.Spawn(target);
                    mouseClicked = false;
                }
            }
            else if (mouseHeld)
            {
                // Raycast and check for hit
                RaycastHit[] hits = RaycastAllSorted();
                // Get the direction from the character's position to the mouse hit point
                Vector3 direction = hits[0].point - transform.position;
                direction.Normalize();
                Vector3 destination = transform.position + direction;
                mover.StartMoveAction(destination, 1f);
                hit = hits[0];
            }
            if (hit.transform != null && hit.transform.tag != "Uninteractable")
            {
                SetCursor(CursorType.Movement);
                return true;
            }
            else
            {
                SetCursor(CursorType.None);
            }
            return false;
        }

        public void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping mapping in cursorMapping)
            {
                if (mapping.cursorType == cursorType)
                {
                    return mapping;
                }
            }
            return cursorMapping[0];
        }

        private RaycastHit[] RaycastAllSorted()
        {
            // Get all layers except whats in the layerMask
            LayerMask includedLayers = ~excludedLayers;
            // Raycast and check for hit
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), clickRadius, 50, includedLayers);
            // Sort hits by distance
            Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            return hits;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            // Raycast and check for hit
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                // Check if hit is on navmesh
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas))
                {
                    target = navMeshHit.position;
                    return true;
                }
            }
            return false;
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void OnDrawGizmos()
        {
            if (hit.collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
        }
    }
}