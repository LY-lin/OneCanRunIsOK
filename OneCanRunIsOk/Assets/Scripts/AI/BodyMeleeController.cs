using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.AI
{
    public class BodyMeleeController : MonoBehaviour
    {
        public GameObject Owner { get; private set; }
        private affiliationType attackerType;
        private float Damage;

        private Dictionary<GameObject, int> dic = new Dictionary<GameObject, int>();

        public void init(Actor actor)
        {
            this.Owner = actor.gameObject;
            this.Damage = actor.GetActorProperties().getMagicAttack() + actor.GetActorProperties().getPhysicalAttack();
            this.attackerType = actor.Affiliation;
        }

        public void preOneAttack()
        {
            dic.Clear();
        }

        void OnCollisionEnter(Collision col)
        {
            
            if (col.gameObject.layer == LayerMask.NameToLayer("physical dectect nut not collision"))
            {
                return;
            }
            //Debug.Log("Enemy Collision!");
            Actor target = col.gameObject.GetComponentInParent<Actor>();
            if (target)
            {
                if (target.Affiliation == this.attackerType)
                    return;
            }
            
            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable && !dic.ContainsKey(damageable.gameObject))
            {

                dic.Add(damageable.gameObject, 1);

                Actor actor = col.gameObject.GetComponentInParent<Actor>();
                ActorProperties colliderProperty = actor.GetActorProperties();
                float finalDamage = this.Damage - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                if (finalDamage < 0f)
                    finalDamage = 0f;
                Debug.Log("Enemy Atttack!");
                Debug.Log(finalDamage);
                damageable.InflictDamage(finalDamage, false, Owner);
            }

        }

        void OnTriggerEnter(Collider col)
        {
            Actor target = col.gameObject.GetComponent<Actor>();
            if (target)
            {
                if (target.Affiliation == this.attackerType)
                    return;
            }

            Damageable damageable = col.gameObject.GetComponent<Damageable>();
            if (damageable && !dic.ContainsKey(damageable.gameObject))
            {

                dic.Add(damageable.gameObject, 1);

                Actor actor = col.gameObject.GetComponent<Actor>();
                ActorProperties colliderProperty = actor.GetActorProperties();
                float finalDamage = this.Damage - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                if (finalDamage < 0f)
                    finalDamage = 0f;
                Debug.Log("Enemy Atttack!");
                Debug.Log(finalDamage);
                damageable.InflictDamage(finalDamage, false, Owner);
            }
        }
    }
}
