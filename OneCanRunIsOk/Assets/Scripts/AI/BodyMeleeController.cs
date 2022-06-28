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

        private bool Attacking = false;

        private Dictionary<GameObject, int> dic = new Dictionary<GameObject, int>();

        DisplaceActionsManager displaceActionsManager;

        void Start()
        {
            displaceActionsManager = GameObject.FindObjectOfType<DisplaceActionsManager>();
            DebugUtility.HandleErrorIfNullFindObject<DisplaceActionsManager, AoeCalculator>(displaceActionsManager, this);
        }

        public void init(Actor actor)
        {
            this.Owner = actor.gameObject;
            this.Damage = actor.GetActorProperties().getMagicAttack() + actor.GetActorProperties().getPhysicalAttack();
            this.attackerType = actor.Affiliation;
        }

        public bool GetAttacking()
        {
            return Attacking;
        }

        public void SetAttacking(bool attacking)
        {
            Attacking = attacking;
        }

        public void preOneAttack()
        {
            dic.Clear();
            SetAttacking(true);
        }

        void OnCollisionEnter(Collision col)
        {
            if (!Attacking)
            {
                return;
            }

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
                Debug.Log("Enemy Atttack!  finalDamage:" + finalDamage);

                Vector3 direction = new Vector3(1, 1, 1);
                Vector3 displaceDestination = (direction).normalized * 5f + actor.gameObject.transform.position;
                DisplaceAction da = new DisplaceAction(actor.gameObject, displaceDestination, Time.time, 1f, 5f);
                displaceActionsManager.addAction(da);

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
                damageable.InflictDamage(finalDamage, false, Owner);
            }
        }
    }
}
