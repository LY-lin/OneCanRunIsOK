using OneCanRun.Game;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    
    public class Damageable : MonoBehaviour
    {
        [Tooltip("Multiplier to apply to the received damage")]
        public float DamageMultiplier = 1f;

        [Range(0, 1)]
        [Tooltip("Multiplier to apply to self damage")]
        public float SensibilityToSelfdamage = 0.5f;

        public Health Health { get; private set; }
        CollectDamageNumber collect;
        void Awake()
        {
            // find the health component either at the same level, or higher in the hierarchy
            Health = GetComponent<Health>();
            if (!Health)
            {
                Health = GetComponentInParent<Health>();
            }
            collect = FindObjectOfType<CollectDamageNumber>();
        }

        public void InflictDamage(float damage, bool isExplosionDamage, GameObject damageSource)
        {
            if (Health)
            {
                var totalDamage = damage;

                // skip the crit multiplier if it's from an explosion
                if (!isExplosionDamage)
                {
                    totalDamage *= DamageMultiplier;
                }

                // potentially reduce damages if inflicted by self
                if (Health.gameObject == damageSource)
                {
                    totalDamage *= SensibilityToSelfdamage;
                }

                // apply the damages
                //Debug.Log(totalDamage);
                Health.TakeDamage(totalDamage, damageSource);
            }
        }

        public void InflictDamage(float damage, bool isExplosionDamage, GameObject damageSource,GameObject Damaged)
        {
            if (Health)
            {
                var totalDamage = damage;

                // skip the crit multiplier if it's from an explosion
                if (!isExplosionDamage)
                {
                    totalDamage *= DamageMultiplier;
                }

                // potentially reduce damages if inflicted by self
                if (Health.gameObject == damageSource)
                {
                    totalDamage *= SensibilityToSelfdamage;
                }

                // apply the damages
                //Debug.Log(totalDamage);
                Health.TakeDamage(totalDamage, damageSource);
                collect.produce(Damaged, 0, damage);

            }
        }
    }

}
