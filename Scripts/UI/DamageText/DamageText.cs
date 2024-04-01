using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI damageText = null;
        public void DestroyText()
        {
            Destroy(gameObject);
        }
        public void SetText(string text)
        {
            damageText.text = string.Format("{0:0.0}", text);
        }
    }
}