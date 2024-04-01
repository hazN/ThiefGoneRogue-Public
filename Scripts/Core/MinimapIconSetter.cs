using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Core
{
    public class MinimapIconSetter : MonoBehaviour
    {
        [System.Serializable]
        public class MinimapIcon
        {
            public Sprite icon;
            public Vector2 size = new Vector2(20,20);
            public bool rotateWithObject = false;
            public bool clampIconInBorder = false;
        }
        [SerializeField] private MinimapIcon[] icons;
        private void Start()
        {
            SetIcon(0);
        }
        public void SetIcon(int iconIndex)
        {
            if (iconIndex < 0 || iconIndex >= icons.Length)
            {
                Debug.LogError("Icon index out of range.");
                return;
            }
            MiniMapComponent miniMapComponent = GetComponent<MiniMapComponent>();
            miniMapComponent.icon = icons[iconIndex].icon;
            miniMapComponent.size = icons[iconIndex].size;
            miniMapComponent.rotateWithObject = icons[iconIndex].rotateWithObject;
            miniMapComponent.clampIconInBorder = icons[iconIndex].clampIconInBorder;
            miniMapComponent.Refresh();
        }
    }
}