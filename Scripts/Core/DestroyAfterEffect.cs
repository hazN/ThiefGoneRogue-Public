using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private GameObject goToDestroy;

        private void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (goToDestroy != null)
                    Destroy(goToDestroy);
                else
                    Destroy(gameObject);
            }
        }
    }
}