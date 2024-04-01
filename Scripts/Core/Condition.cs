using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;
        // Check if all the AND conditions are true
        public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(predicateEvaluators)) return false;
            }
            return true;
        }
        [System.Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;
            // Check if any of the OR conditions are true
            public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluators)
            {
                foreach (Predicate predicate in or)
                {
                    if (predicate.Check(predicateEvaluators)) return true;
                }
                return false;
            }
        }
        [System.Serializable]
        public class Predicate
        {
            [SerializeField] private EPredicate predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;
            public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluators)
            {
                foreach (IPredicateEvaluator predicateEvaluator in predicateEvaluators)
                {
                    bool? result = predicateEvaluator.Evaluate(predicate, parameters);
                    if (result == null) continue;
                    if (result == negate) return false;
                }
                return true;
            }
        }
    }
}