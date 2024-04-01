using UnityEngine;
using Cinemachine;
using RPG.Control;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RPG.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] GameObject freeLookCamera;
        [SerializeField] float scrollSpeed = 20f;
        [SerializeField] float cameraSpeed = 500f;
        [SerializeField]
        [Range(0f, 1f)] float startingZoom = 1f;
        private CinemachineFreeLook cinemachineCamera;
        private PlayerController playerControllerScript;
        private InputAction rightMouseButtonHold;

        private void Awake()
        {
            cinemachineCamera = freeLookCamera.GetComponent<CinemachineFreeLook>();
            playerControllerScript = GetComponent<PlayerController>();
            cinemachineCamera.m_YAxis.Value = startingZoom;
            // Find the right mouse button hold input action
            rightMouseButtonHold = new InputAction("RightMouseButtonHold", binding: "<Mouse>/rightButton");
            rightMouseButtonHold.performed += RightMouseButtonHoldPerformed;
            rightMouseButtonHold.canceled += RightMouseButtonHoldCanceled;
        }

        private void OnEnable()
        {
            // Enable the right mouse button hold input action
            rightMouseButtonHold.Enable();
        }

        private void OnDisable()
        {
            // Disable the right mouse button hold input action
            rightMouseButtonHold.Disable();
        }

        private void Update()
        {
            // Check if mouse wheel is scrolled only if not hovering over UI
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!Mathf.Approximately(Mouse.current.scroll.y.ReadValue(), 0f))
            {
                cinemachineCamera.m_YAxis.m_MaxSpeed = scrollSpeed;
            }
        }

        private void RightMouseButtonHoldPerformed(InputAction.CallbackContext context)
        {
            //if (playerControllerScript.isDraggingUI) return;
            cinemachineCamera.m_XAxis.m_MaxSpeed = cameraSpeed;
        }

        private void RightMouseButtonHoldCanceled(InputAction.CallbackContext context)
        {
            cinemachineCamera.m_XAxis.m_MaxSpeed = 0;
        }
    }
}
