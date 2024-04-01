using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private List<Modifier> modifiers = new List<Modifier>();

        public IEnumerable<(float absoluteModifier, float percentModifier)> GetModifiers(Stat stat)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier.stat == stat)
                {
                    yield return (modifier.additiveValue, modifier.percentageValue);
                }
            }
        }
        public IEnumerable<Modifier> GetAllModifiers()
        {
            foreach (var modifier in modifiers)
            {
                yield return modifier;
            }
        }
        [System.Serializable]
        public struct Modifier
        {
            public Stat stat;
            public float additiveValue;
            public float percentageValue;
        }
        public IEnumerable<Modifier> GetModifiers()
        {
            return modifiers;
        }
        public void AddModifier(Stat stat, float additiveValue, float percentageValue)
        {
            Modifier modifier = new Modifier();
            modifier.stat = stat;
            modifier.additiveValue = additiveValue;
            modifier.percentageValue = percentageValue;
            modifiers.Add(modifier);
        }
    }
}
