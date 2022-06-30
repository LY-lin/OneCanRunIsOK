using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class DragonFlame : MonoBehaviour
    {
        [Tooltip("attack length of the DragonFlame")]
        public float length = 9f;
        [Tooltip("attack width of the DragonFlame")]
        public float width = 2f;
        [Tooltip("impact height of the DragonFlame")]
        public float height = 2f;
        [Tooltip("damage per second of the DragonFlame")]
        public float damage = 10f;
        [Tooltip("how many frames to calculate per attack")]
        public int deltaCount = 25;

        public GameObject Owner;

        public DamageType damageType;

        float totalDeltaTime = 0;
        int curDeltaCount = 0;

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if (curDeltaCount == deltaCount)
            {
                Collider[] affectedColliders = Physics.OverlapBox(this.gameObject.transform.position + transform.forward * (length / 2), new Vector3(width, height, length));
                foreach (var col in affectedColliders)
                {
                    //damage
                    Damageable damageable = col.GetComponent<Damageable>();
                    if (damageable)
                    {
                        Actor actor = col.gameObject.GetComponentInParent<Actor>();
                        ActorProperties colliderProperty = actor.GetActorProperties();
                        float finalDamage = calculateDamage(colliderProperty, damage * totalDeltaTime, damageType);
                        Debug.Log("龙炎伤害：" + finalDamage);
                        if (Owner != actor.gameObject)
                            damageable.InflictDamage(finalDamage, false, Owner, col.gameObject, damageType);
                    }
                }
                curDeltaCount = 0;
                totalDeltaTime = 0;
            }
            curDeltaCount++;
            totalDeltaTime += Time.deltaTime;

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

            //GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            //if (hurtNumber && hurtNumberParent)
            //{
            //    //Debug.Log("count!");
            //    GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
            //    hurt.transform.position = damagePoint;
            //    hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            //}

            return finalDamage;
        }
    }
}
