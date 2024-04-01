using RPG.Attributes;
using RPG.Control;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weapon = null;
        [SerializeField] private float healthToRestore = 0f;
        [SerializeField] private float respawnTime = 5f;
        [SerializeField] private bool destroyOnPickup = false;
        [SerializeField] private float pickupRadius = 10f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject player)
        {
            if (weapon != null)
            {
                player.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0)
            {
                player.GetComponent<Health>().Heal(healthToRestore);
            }
            if (!destroyOnPickup)
                StartCoroutine(HideForSeconds(respawnTime));
            else Destroy(gameObject);
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool showPickup)
        {
            if (showPickup)
            {
                GetComponent<Collider>().enabled = true;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
        public bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld)
        {
            if (mouseClicked)
            {
                PickUp(playerController.gameObject);
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