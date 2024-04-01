using UnityEngine;

namespace RPG.UI
{
    public class MoveToEffect : MonoBehaviour
    {
        [SerializeField] private float expandSpeed = 1f;
        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private float destroyDelay = 3f;
        [SerializeField] private Transform ringTransform;
        [SerializeField] private Renderer ringRenderer;

        private void Start()
        {
            // Start the destruction countdown
            Destroy(gameObject, destroyDelay);
        }

        private void Update()
        {
            // Expand the ring
            float currentScale = ringTransform.localScale.x;
            float newScale = currentScale + expandSpeed * Time.deltaTime;
            ringTransform.localScale = new Vector3(newScale, newScale, newScale);

            // Fade the ring
            Color currentColor = ringRenderer.material.color;
            float newAlpha = currentColor.a - fadeSpeed * Time.deltaTime;
            ringRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
        }
    }
}
