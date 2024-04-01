using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Stats
{
    [System.Serializable]
    public struct Modifier
    {
        public Stat stat;
        public float additiveValue;
        public float percentageValue;
        public Modifier(Stat stat, float additive, float percentage) 
        {
            this.stat = stat;
            this.additiveValue = additive;
            this.percentageValue = percentage;
        }
    }
}