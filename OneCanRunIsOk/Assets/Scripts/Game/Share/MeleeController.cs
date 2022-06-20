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
        private Dictionary<GameObject, int> dic = new Dictionary<GameObject, int>();

        public GameObject hurtNumber;
        public DamageType damageType;
        public GameObject attackVfx;
        public float attackVfxDelay;
        public Transform attackSocket;

        [Header("Attack Range (center-attackSocket)")]
        public float attackLength = 1.0f;
        public float attackWidth = 1.0f;
        public float attackHeight = 2.0f;

        //UnityAction<Vector3> Dmg;

        public void Init(WeaponController wc)
        {
            this.wc = wc;
            Damage = wc.damage;
            this.attackerType = wc.Owner.GetComponent<Actor>().Affiliation;
            this.Owner = wc.Owner;
            damageType = wc.damageType;
            attackSocket = GameObject.Find("SkillSocket").transform;

        }
        //public void ReleaseDic()
        //{
        //    dic.Clear();
        //}
        // Update is called once per frame
        //void Update()
        //{
            
        //    /*
        //    ActorProperties tmp = wc.Owner.GetComponent<Actor>().GetActorProperties();
        //    float tmpDamage = tmp.getMagicAttack() + tmp.getPhysicalAttack();
        //    Damage *= tmpDamage;
        //    */
        //}

        //void OnCollisionEnter(Collision col)
        //{
        //    /*
        //    if (col.gameObject.layer == LayerMask.NameToLayer("weapon"))
        //    {
        //        return;
        //    }*/

        //    Actor target = col.gameObject.GetComponent<Actor>();
        //    if (target)
        //    {
        //        if (target.Affiliation == this.attackerType)
        //            return;
        //    }

        //    Damageable damageable = col.collider.GetComponent<Damageable>();
        //    if (damageable&&!dic.ContainsKey(damageable.gameObject))
        //    {
                
        //        dic.Add(damageable.gameObject, 1);
                
        //        Actor actor = col.gameObject.GetComponent<Actor>();
        //        ActorProperties colliderProperty = actor.GetActorProperties();
        //        float finalDamage = calculateDamage(colliderProperty, Damage, damageType, col.gameObject.transform.position + transform.up);
        //        damageable.InflictDamage(finalDamage, false, Owner);

        //    }
        //}

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

            //下取整
            finalDamage = Mathf.Floor(finalDamage);

            GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            if (hurtNumber && hurtNumberParent)
            {
                Debug.Log("count!");
                GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
                hurt.transform.position = damagePoint;
                hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            }
            //Dmg.Invoke(damagePoint, damageType, finalDamage);

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
            Collider[] affectedColliders = Physics.OverlapBox(attackSocket.position, new Vector3(attackLength, attackHeight, attackWidth), Quaternion.identity);
            foreach (var col in affectedColliders)
            {
                //damage
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    Actor actor = col.gameObject.GetComponent<Actor>();
                    ActorProperties colliderProperty = actor.GetActorProperties();
                    float finalDamage = calculateDamage(colliderProperty, Damage, damageType, col.gameObject.transform.position + transform.up);
                    damageable.InflictDamage(finalDamage, false, Owner);
                }
            }
        }
    }
}
