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
            this.Damage = 10f;
            this.attackerType = actor.Affiliation;
        }

        public void preOneAttack()
        {
            dic.Clear();
        }

        void OnCollisionEnter(Collision col)
        {
            /*
            if (col.gameObject.layer == LayerMask.NameToLayer("weapon"))
            {
                return;
            }*/
            Debug.Log("Enemy Collision!");
            Actor target = col.gameObject.GetComponent<Actor>();
            if (target)
            {
                if (target.Affiliation == this.attackerType)
                    return;
            }
            
            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable && !dic.ContainsKey(damageable.gameObject))
            {

                dic.Add(damageable.gameObject, 1);

                Actor actor = col.gameObject.GetComponent<Actor>();
                ActorProperties colliderProperty = actor.GetActorProperties();
                float finalDamage = this.Damage - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                if (finalDamage < 0f)
                    finalDamage = 0f;
                Debug.Log("Enemy Atttack!");
                damageable.InflictDamage(finalDamage, false, Owner);

            }

        }
    }
}
