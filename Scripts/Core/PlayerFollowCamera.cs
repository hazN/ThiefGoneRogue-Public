using UnityEngine;

namespace RPG.Core
{
    public class PlayerFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}