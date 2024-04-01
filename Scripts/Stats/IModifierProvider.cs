
using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<(float absoluteModifier, float percentModifier)> GetModifiers(Stat stat);
    }
}