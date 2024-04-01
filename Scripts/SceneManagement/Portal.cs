using RPG.Control;
using RPG.Movement;
using RPG.Saving;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] private bool PortalIsOpen = true;
        private enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] private string sceneToLoad = "";
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier source;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private float fadeInTime = 1f;
        [SerializeField] private float fadeWaitTime = 0.5f;
        public void TogglePortal(bool isOpen)
        {
            PortalIsOpen = isOpen;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && PortalIsOpen)
            {
                StartCoroutine(LoadScene());
            }
        }

        private IEnumerator LoadScene()
        {
            // Make sure the scene to load is set
            if (sceneToLoad == "")
            {
                Debug.LogError("Scene to load is not set.");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            // Get fader and fade out
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            // Remove player control
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;

            // Saving 
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            // Load scene async
            yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
            savingWrapper.Load();

            // Match portal to destination
            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayer(destinationPortal);
            savingWrapper.Save();

            // Fade in
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            // Restore player control
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal destinationPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(destinationPortal.spawnPoint.position);
            player.transform.rotation = destinationPortal.spawnPoint.rotation;
        }

        private Portal GetDestinationPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.source == destination)
                {
                    return portal;
                }
            }
            print("Error: No portal found");
            return null;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Portal;
        }

        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (mouseClicked)
            {
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public float GetRadius()
        {
            return Mathf.Infinity;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public object CaptureState()
        {
            return PortalIsOpen;
        }

        public void RestoreState(object state)
        {
            PortalIsOpen = (bool)state;
        }
    }
}