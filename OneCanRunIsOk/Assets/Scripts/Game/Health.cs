using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")] 
        public float MaxHealth = 10f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }
        
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        public bool m_IsDead;

        public Share.ActorProperties properties;
        float totalTime = 1f;
        void Start()
        {
            properties = GetComponent<Actor>().GetActorProperties();

            if (this.gameObject.name != "Dragon")
            {
                MaxHealth = properties.getMaxHealth();
                CurrentHealth = properties.getMaxHealth();

            }
            CurrentHealth = MaxHealth;
            
        }
        void Update()
        {
            totalTime -= Time.deltaTime;
            if (totalTime <= 0f)
            {
                Heal(properties.getHealRate());
                totalTime = 1f;
            }
        }

        public void Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }

        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
                return;
            //Debug.Log(" 受伤，扣血："+damage);
            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
            // call OnDamage action
            float trueDamageAmount = healthBefore - CurrentHealth;
            if (trueDamageAmount > 0f)
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource);
            }

            HandleDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f;

            // call OnDamage action
            OnDamaged?.Invoke(MaxHealth, null);

            HandleDeath();
        }

        void HandleDeath()
        {
            if (m_IsDead)
                return;

            // call OnDie action
            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;
                OnDie?.Invoke();
                GameObject player = GameObject.Find("Player1");
                if (player)
                {
                    OneCanRun.Game.Actor mPlayer = player.GetComponent<Actor>();
                    Share.Modifier mod = new Share.Modifier(400, Share.Modifier.ModifierType.experience, this);
                    mPlayer.addModifier(mod);
                }
                // Share.MonsterPoolManager monsterPoolManager = Game.Share.MonsterPoolManager.getInstance();
                // monsterPoolManager.release(this.gameObject);
                //Destroy(this.gameObject);
            }
        }
    }
}
