using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class BuffStore : MonoBehaviour
    {
        [SerializeField] BuffEffectController effectController;
        private List<Buff> buffs = new List<Buff>();
        private Dictionary<Buff, float> buffTimes = new Dictionary<Buff, float>();
        public UnityEvent buffEvent;
        public IEnumerable<Buff> GetBuffs()
        {
            return buffs;
        }
        public void AddBuff(Buff buff)
        {
            // Check if buff already exists from same source
            if (buffTimes.ContainsKey(buff))
            {
                buffTimes[buff] = buff.GetBuffDuration();
                return;
            }
            buff.ResetTimeLeft();
            buffs.Add(buff);
            buffTimes[buff] = buff.GetBuffDuration();
            buffEvent.Invoke();
            if (effectController != null)
            {
                effectController.SetBuff(true, buff.GetBuffType());
            }
        }
        public void RemoveBuff(Buff buff)
        {
            buffs.Remove(buff);
            buffEvent.Invoke();
            buffTimes.Remove(buff);
            if (effectController != null)
            {
                effectController.SetBuff(false, buff.GetBuffType());
            }
        }
        public float GetBuffValue(Buff.Buffs buffToCheck)
        {
            float buffValue = 0;
            foreach (var buff in buffs)
            {
                if (buff.GetBuffType() == buffToCheck)
                {
                    buffValue += buff.GetBuffPercentageModifier();
                }
            }
            return buffValue;
        }
        private void Update()
        {
            if (buffs.Count == 0) return;
            foreach (var buff in buffs.ToList())
            {
                if (!buffTimes.ContainsKey(buff)) buffTimes[buff] = buff.GetBuffDuration();
                buffTimes[buff] -= Time.deltaTime;
                if (buffTimes[buff] <= 0)
                {
                    RemoveBuff(buff);
                }
            }
        }
    }
}