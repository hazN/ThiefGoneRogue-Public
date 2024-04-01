using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEditor;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        [SerializeField] private float maxMana;
        [SerializeField] private float mana;
        [Tooltip("Mana percentage regained per second")]
        [Range(0f, 1f)]
        [SerializeField] private float manaRegenRate = 0.05f;
        private void Awake()
        {
            mana = 0;
        }
        private void Update()
        {
            if (mana < GetMaxMana())
            {
                var manaRegenMods = GetComponent<BaseStats>().GetModifiers(Stat.ManaRegenRate);
                float totalManaRegen = (manaRegenMods.Item1 + manaRegenMods.Item2) / 200f + manaRegenRate;
                mana += totalManaRegen * maxMana * Time.deltaTime;
                mana = Mathf.Clamp(mana, 0, GetMaxMana());
            }
        }
        public float GetMana()
        {
            return mana;
        }
        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }
        public void AddMana(float manaToAdd)
        {
            mana += manaToAdd;
            mana = Mathf.Clamp(this.mana, 0, GetMaxMana());
        }
        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana)
            {
                return false;
            }
            mana -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return mana;
        }

        public void RestoreState(object state)
        {
            float loadedMana = (float)state;
            if (loadedMana > 0)
            {
                mana = loadedMana;
            }
            else
            {
                mana = GetMaxMana();
            }
        }
    }
}