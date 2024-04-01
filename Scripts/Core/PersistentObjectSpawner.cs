using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab;
        static bool hasSpawned = false;
        private void Awake()
        {
            if (persistentObjectPrefab != null && hasSpawned == false) 
            {
                GameObject persistentObject = Instantiate(persistentObjectPrefab);
                DontDestroyOnLoad(persistentObject);
                hasSpawned = true;
            }
        }
    }
}