using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "RPG/Abilities/Filters/TagFilter")]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] private string[] tagsToFilter;

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> targets)
        {
            foreach (GameObject target in targets)
            {
                if (target == null) continue;
                foreach (string tag in tagsToFilter)
                {
                    if (target.CompareTag(tag))
                    {
                        yield return target;
                    }
                }
            }
        }
    }
}