using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class WeaponHitbox : MonoBehaviour
    {
        static public List<GameObject> targets = new List<GameObject>();
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                targets.Add(other.gameObject);
            }
        }
    }
}