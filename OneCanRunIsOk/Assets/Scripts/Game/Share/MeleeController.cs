using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{
    public class MeleeController : MonoBehaviour
    {
        WeaponController wc;
        private float Damage;
        private affiliationType attackerType;
        public GameObject Owner { get; private set; }

        public DamageType damageType;
        public GameObject attackVfx;
        public float attackVfxDelay;
        public Transform attackSocket;

        [Header("Attack Range (center-attackSocket)")]
        public float attackLength = 1.0f;
        public float attackWidth = 1.0f;
        public float attackHeight = 2.0f;

        [Header("Attack Sfx")]
        AudioSource m_ShootAudioSource;
        public AudioClip WeaponAttackSfx;
        public AudioClip hitSfx;

        public void Init(WeaponController wc)
        {
            this.wc = wc;
            Damage = wc.damage;
            this.attackerType = wc.Owner.GetComponent<Actor>().Affiliation;
            m_ShootAudioSource = GetComponent<AudioSource>();
            this.Owner = wc.Owner;
            damageType = wc.damageType;
            attackSocket = GameObject.Find("SkillSocket").transform;

        }

        private float calculateDamage(ActorProperties colliderProperty, float damage, DamageType damageType)
        {
            if (colliderProperty == null)
                return 0;
            float finalDamage = 0;
            if (damageType == DamageType.magic)
            {
                finalDamage = damage - colliderProperty.getMagicDefence();
            }
            else
            {
                finalDamage = damage - colliderProperty.getPhysicalDefence();
            }
            if (finalDamage < 0f)
                finalDamage = 0f;

            //下取整
            finalDamage = Mathf.Floor(finalDamage);


            return finalDamage;
        }

        void PlayAttackVfx()
        {
            //float time = 0;
            //time += Time.deltaTime;
            //if (time > attackVfxDelay)
            //{
                //vfx
                GameObject VfxInstance = Instantiate(attackVfx, attackSocket);
                //VfxInstance.transform.position -= GetComponentInParent<Camera>().transform.right;
                Destroy(VfxInstance.gameObject, 1);
            //}
        }

        void Attack()
        {
            m_ShootAudioSource.PlayOneShot(WeaponAttackSfx);
            Collider[] affectedColliders = Physics.OverlapBox(attackSocket.position, new Vector3(attackLength, attackHeight, attackWidth), Quaternion.identity);
            
            foreach (var col in affectedColliders)
            {
                //damage
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    m_ShootAudioSource.PlayOneShot(hitSfx);
                    Actor actor = col.gameObject.GetComponentInParent<Actor>();
                    ActorProperties colliderProperty = actor.GetActorProperties();
                    float finalDamage = calculateDamage(colliderProperty, Damage, damageType);
                    damageable.InflictDamage(finalDamage, false, Owner, col.gameObject, damageType);
                }
            }
        }
    }
}
