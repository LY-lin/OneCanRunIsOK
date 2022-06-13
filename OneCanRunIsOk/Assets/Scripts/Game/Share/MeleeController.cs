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

        public void Init(WeaponController wc)
        {
            this.wc = wc;
            Damage = wc.damage;
            this.attackerType = wc.Owner.GetComponent<Actor>().Affiliation;
            this.Owner = wc.Owner;

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
                Debug.Log(11111);
                Actor actor = col.gameObject.GetComponent<Actor>();
                ActorProperties colliderProperty = actor.GetActorProperties();
                float finalDamage = this.Damage - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                if (finalDamage < 0f)
                    finalDamage = 0f;
                damageable.InflictDamage(finalDamage, false, Owner);

            }

        }
    }
}
