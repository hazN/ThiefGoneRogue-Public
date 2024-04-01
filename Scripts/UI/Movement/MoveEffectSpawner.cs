using RPG.Core;
using System.Collections;
using UnityEngine;

namespace RPG.UI
{
    public class MoveEffectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject moveEffectPrefab = null;
        public void Spawn(Vector3 position)
        {
            GameObject instance = Instantiate(moveEffectPrefab, position, Quaternion.identity);
        }
    }
}