using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class MeleeController : MonoBehaviour
    {
        WeaponController wc;
        private float Damage;
        private affiliationType attackerType;
        public GameObject Owner { get; private set; }
        private Dictionary<GameObject, int> dic = new Dictionary<GameObject, int>();

        public GameObject hurtNumber;
        public DamageType damageType;

        public void Init(WeaponController wc)
        {
            this.wc = wc;
            Damage = wc.damage;
            this.attackerType = wc.Owner.GetComponent<Actor>().Affiliation;
            this.Owner = wc.Owner;
            damageType = wc.damageType;

        }
        public void ReleaseDic()
        {
            dic.Clear();
        }
        // Update is called once per frame
        void Update()
        {
            
            /*
            ActorProperties tmp = wc.Owner.GetComponent<Actor>().GetActorProperties();
            float tmpDamage = tmp.getMagicAttack() + tmp.getPhysicalAttack();
            Damage *= tmpDamage;
            */
        }

        void OnCollisionEnter(Collision col)
        {
            /*
            if (col.gameObject.layer == LayerMask.NameToLayer("weapon"))
            {
                return;
            }*/

            Actor target = col.gameObject.GetComponent<Actor>();
            if (target)
            {
                if (target.Affiliation == this.attackerType)
                    return;
            }

            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable&&!dic.ContainsKey(damageable.gameObject))
            {
                
                dic.Add(damageable.gameObject, 1);
                
                Actor actor = col.gameObject.GetComponent<Actor>();
                ActorProperties colliderProperty = actor.GetActorProperties();
                float finalDamage = calculateDamage(colliderProperty, Damage, damageType, col.gameObject.transform.position + transform.up);
                damageable.InflictDamage(finalDamage, false, Owner);

            }
        }

        private float calculateDamage(ActorProperties colliderProperty, float damage, DamageType damageType, Vector3 damagePoint)
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

            //ÏÂÈ¡Õû
            finalDamage = Mathf.Floor(finalDamage);

            GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            if (hurtNumber && hurtNumberParent)
            {
                Debug.Log("count!");
                GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
                hurt.transform.position = damagePoint;
                hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            }

            return finalDamage;
        }
    }
}
