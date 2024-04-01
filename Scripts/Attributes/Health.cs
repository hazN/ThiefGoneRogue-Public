using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [SelectionBase]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private TakeDamageEvent takeDamage;
        public UnityEvent onPoison;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        { }

        [SerializeField] public UnityEvent onDie;
        [SerializeField] public UnityEvent onHeal;
        private LazyValue<float> health;
        [SerializeField] private float maxRegen = 0.75f;
        [SerializeField] private float regenPercentPerSecond = 0.01f;
        [SerializeField] private float regenWaitTime = 5f;
        private float timeSinceTakenDamage = Mathf.Infinity;
        private bool isDead = false;
        private BuffStore buffStore;
        private float poisonValue = 0;

        private void Awake()
        {
            health = new LazyValue<float>(GetMaxHealth);
            buffStore = GetComponent<BuffStore>();
        }

        private void Update()
        {
            if (isDead) return;

            // Regenerate health
            timeSinceTakenDamage += Time.deltaTime;
            if (timeSinceTakenDamage >= regenWaitTime)
            {
                // Get regenrate modifiers
                float totalRegenRate = GetComponent<BaseStats>().GetStat(Stat.HealthRegenRate) + regenPercentPerSecond;
                float regenHealth = GetMaxHealth() * totalRegenRate * Time.deltaTime;
                // Regenerate health if not under max regen
                if (health.value < GetMaxHealth() * maxRegen)
                    health.value = Mathf.Min(health.value + regenHealth, GetMaxHealth() * maxRegen);
            }
            // Take poison damage
            float poisonValue = buffStore.GetBuffValue(Buff.Buffs.Poison);
            if (poisonValue > 0)
            {
                // poison is ap ercentage of maxhealth
                float poisonDamage = GetMaxHealth() * poisonValue * Time.deltaTime;
                health.value = Mathf.Max(health.value - poisonDamage, 0);
                onPoison.Invoke();
                if (health.value == 0)
                {
                    onDie.Invoke();
                    Die();
                    if (tag == "Enemy")
                    {
                        var player = GameObject.FindWithTag("Player");
                        AwardExperience(player);
                    }
                }
            }
        }
        public void UpdatePoison()
        {
            poisonValue = buffStore.GetBuffValue(Buff.Buffs.Poison);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += onLevelUpHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= onLevelUpHealth;
        }

        public float Heal(float healthToRestore)
        {
            onHeal?.Invoke();
            health.value = Mathf.Min(GetHealth() + healthToRestore, GetMaxHealth());
            return health.value;
        }

        public float GetHealth()
        {
            return health.value;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return health.value / GetMaxHealth();
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isDead) return;

            timeSinceTakenDamage = 0;
            // Check for debuffs to apply
            foreach (var buff in instigator.GetComponent<Fighter>().GetCurrentWeapon().GetBuffs())
            {
                buffStore.AddBuff(buff);
            }
            damage += damage * buffStore.GetBuffValue(Buff.Buffs.Burn);
            health.value = Mathf.Max(health.value - damage, 0);
            if (health.value == 0)
            {
                Die();
                AwardExperience(instigator);
                if (tag != "Player")
                {
                    GetComponentInChildren<HealthBar>().gameObject.SetActive(false);
                }
                onDie?.Invoke();
            }
            int damageInt = Mathf.RoundToInt(damage);
            takeDamage.Invoke(damageInt);
        }

        private void onLevelUpHealth()
        {
            health.value = GetMaxHealth();
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            // Disable components
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false;

            // If this is not the player we should destroy it
            if (!gameObject.CompareTag("Player"))
            {
                Destroy(gameObject, 10f);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            if (instigator.TryGetComponent<Experience>(out Experience experience))
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void Revive()
        {
            isDead = false;
            health.value = GetMaxHealth();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<Animator>().Rebind();
        }

        public object CaptureState()
        {
            float state = health.value;
            return state;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            // If the health is 0, destroy the object as it was dead when the game was saved
            if (health.value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}