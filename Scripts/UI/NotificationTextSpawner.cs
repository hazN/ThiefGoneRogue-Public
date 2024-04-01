using UnityEngine;

namespace RPG.UI
{
    public class NotificationTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText notificationTextPrefab = null;
        [SerializeField] private Transform transform = null;

        private static DamageText prefab = null;
        private static Transform parent = null;

        private void Awake()
        {
            prefab = notificationTextPrefab;
            parent = transform;
        }

        public static void Spawn(string text)
        {
            DamageText instance = Instantiate<DamageText>(prefab, parent);
            instance.SetText(text);
        }
    }
}