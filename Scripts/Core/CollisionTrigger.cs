using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    public class CollisionTrigger : MonoBehaviour
    {
        public UnityEvent collisionEvent = null;
        [SerializeField] private string[] tagsToCheck = { "Player" };
        private void OnTriggerEnter(Collider other)
        {
            foreach (string tag in tagsToCheck)
            {
                if (other.gameObject.tag == tag)
                {
                    CallEvent();
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                CallEvent();
            }
        }
        private void CallEvent()
        {
            collisionEvent.Invoke();
        }
    }
}