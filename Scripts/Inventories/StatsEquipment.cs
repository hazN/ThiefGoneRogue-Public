using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<(float absoluteModifier, float percentModifier)> GetModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (var modifier in item.GetModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}