using RPG.Control;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "RPG/Abilities/Targeting/Delayed Click Targeting")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;
        [SerializeField] private InputActionReference actionButton;
        [SerializeField] private InputActionReference cancelButton;
        [SerializeField] private LayerMask excludedLayers;
        [SerializeField] private float areaEffectRadius;
        [SerializeField] private GameObject targetingPrefab;

        private Transform targetingPrefabInstance = null;
        private bool mouseClicked = false;
        private bool mouseReleased = false;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            actionButton.action.performed += ActionButton;
            actionButton.action.canceled += ReleasedButton;
            playerController.StartCoroutine(Targeting(data, playerController, finished));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            // Disable player controller while targeting
            playerController.enabled = false;
            // Instantiate targeting prefab if its not already instantiated
            if (!targetingPrefabInstance)
            {
                targetingPrefabInstance = Instantiate(targetingPrefab, data.GetUser().transform).transform;
            }
            else // Otherwise, just enable it
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }
            targetingPrefabInstance.localScale = new Vector3(areaEffectRadius, areaEffectRadius, areaEffectRadius);
            // Run every frame
            while (!data.IsCanceled())
            {
                // Set mouseReleased to false to make sure it wasn't
                // triggered from a previous click
                mouseReleased = false;
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit hit;
                LayerMask includedLayers = ~excludedLayers;

                if (Physics.Raycast(PlayerController.GetMouseRay(), out hit, 1000, includedLayers))
                {
                    targetingPrefabInstance.position = hit.point;
                    targetingPrefabInstance.rotation = Quaternion.LookRotation(-hit.normal);

                
                    if (mouseClicked)
                    {
                        // Wait for mouse release
                        while (!mouseReleased)
                        {
                            yield return null;
                        }

                        // Get all game objects in radius
                        Transform transform = targetingPrefabInstance.transform;
                        transform.Rotate(-90f, 0f, 0f);
                        transform.localScale = targetingPrefabInstance.localScale / 2f;
                        data.SetTargetedPoint(transform);
                        data.SetTargets(GetGameObjectsInRadius(hit.point));
                        data.SetOriginalTarget(transform.position);

                        break;
                    }
                    // If cancel button is pressed, cancel the action
                    if (cancelButton.action.triggered)
                    {
                        data.GetUser().GetComponent<ActionScheduler>().CancelCurrentAction();
                        break;
                    }
                }
                yield return null;
            }
            // Reenable player controller
            playerController.enabled = true;

            // Reset bools and unsubscribe from events
            targetingPrefabInstance.gameObject.SetActive(false);
            ResetVariables();
            finished();

        }

        private void ResetVariables()
        {
            mouseClicked = false;
            mouseReleased = false;
            actionButton.action.performed -= ActionButton;
            actionButton.action.canceled -= ActionButton;
        }

        private void ActionButton(InputAction.CallbackContext obj)
        {
            mouseClicked = true;
        }

        private void ReleasedButton(InputAction.CallbackContext obj)
        {
            mouseReleased = true;
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaEffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.transform.gameObject;
            }
        }
    }
}