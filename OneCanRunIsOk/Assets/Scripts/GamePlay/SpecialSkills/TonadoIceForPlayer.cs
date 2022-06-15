using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class TonadoIceForPlayer : MonoBehaviour
    {
        [Tooltip("impact radius of the tonado")]
        public float radius= 3f;
        [Tooltip("impact height of the tonado")]
        public float height = 6f;
        [Tooltip("damage per second of the tonado")]
        public float damage = 1f;
        [Tooltip("speed of the displacement")]
        public float displacementSpeed = 1f;

        public GameObject Owner;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Collider[] affectedColliders = Physics.OverlapBox(this.gameObject.transform.position, new Vector3(radius, height, radius), Quaternion.identity);
            foreach (var col in affectedColliders)
            {
                //damage
                //InParent?
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    Actor actor = col.gameObject.GetComponent<Actor>();
                    ActorProperties colliderProperty = actor.GetActorProperties();
                    float finalDamage = damage * Time.deltaTime - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                    if (finalDamage < 0f)
                        finalDamage = 0f;
                    damageable.InflictDamage(finalDamage, false, Owner);

                    //displacement of affectedColliders
                    col.gameObject.transform.position = Vector3.MoveTowards(col.gameObject.transform.position, this.gameObject.transform.position, displacementSpeed * Time.deltaTime);
                }

            }
        }
    }
}
