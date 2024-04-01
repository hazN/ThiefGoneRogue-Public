using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RPG.Attributes
{
    [Serializable]
    public struct Buff
    {
        public enum Buffs { None, Burn, Freeze, Poison }
        [SerializeField] private float buffDuration;
        [SerializeField] private float buffPercentageModifier;
        [SerializeField] private Buffs buffType;
        [SerializeField] private string source;
        public float timeLeft;
        public void SetBuff(float buffDuration, float buffPercentageModifier, Buffs buffType)
        {
            this.buffDuration = buffDuration;
            this.buffPercentageModifier = buffPercentageModifier;
            this.buffType = buffType;
            timeLeft = buffDuration;
        }
        public void Update()
        {
            timeLeft -= Time.deltaTime;
        }
        public Buffs GetBuffType()
        {
            return buffType;
        }
        public float GetBuffDuration()
        {
            return buffDuration;
        }
        public float GetBuffPercentageModifier()
        {
            return buffPercentageModifier / 50f;
        }
        public float GetTimeLeft()
        {
            return timeLeft;
        }
        public void ResetTimeLeft()
        {
            timeLeft = buffDuration;
        }
        public string GetSource()
        {
            return source;
        }
        public void SetSource(string source)
        {
            this.source = source;
        }
    }
}