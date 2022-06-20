using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class TonadoIce : MonoBehaviour
    {
        [Tooltip("impact radius of the tonado")]
        public float radius= 3f;
        [Tooltip("impact height of the tonado")]
        public float height = 6f;
        [Tooltip("damage per second of the tonado")]
        public float damage = 1f;
        [Tooltip("speed of the displacement")]
        public float displacementSpeed = 1f;
        [Tooltip("how many frames to calculate per attack")]
        public int deltaCount = 50;

        public GameObject Owner;

        public GameObject hurtNumber;

        public DamageType damageType;

        float totalDeltaTime = 0;
        int curDeltaCount = 0;

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            Collider[] affectedColliders = Physics.OverlapBox(this.gameObject.transform.position, new Vector3(radius, height, radius), Quaternion.identity);
            foreach (var col in affectedColliders)
            {
                //damage
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    //displacement of affectedColliders
                    col.gameObject.transform.position = Vector3.MoveTowards(col.gameObject.transform.position, this.gameObject.transform.position, displacementSpeed * Time.deltaTime);

                    if (curDeltaCount == deltaCount)
                    {
                        Actor actor = col.gameObject.GetComponent<Actor>();
                        ActorProperties colliderProperty = actor.GetActorProperties();
                        float finalDamage = calculateDamage(colliderProperty, damage * totalDeltaTime, damageType, col.gameObject.transform.position + transform.up);
                        damageable.InflictDamage(finalDamage, false, Owner);
                    }
                }
            }
            if (curDeltaCount == deltaCount)
            {
                curDeltaCount = 0;
                totalDeltaTime = 0;
            }
            curDeltaCount++;
            totalDeltaTime += Time.deltaTime;

        }
        private float calculateDamage(ActorProperties colliderProperty, float damage, DamageType damageType,Vector3 damagePoint)
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
                //Debug.Log("count!");
                GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
                hurt.transform.position = damagePoint;
                hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            }

            return finalDamage;
        }
    }
}
