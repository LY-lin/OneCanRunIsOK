using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OneCanRun.Game.Share
{
    public class AoeCalculator : MonoBehaviour
    {
        [Tooltip("the radius of the Aoe")]
        public float AoeRadius = 2.0f;

        [Tooltip("if the collider can be displaced")]
        public bool canDisplace = false;

        [Tooltip("the distance for displacement")]
        public float displacementDistance = 1.0f;

        [Tooltip("the force for displacement")]
        public float displacementForce = 1.0f;

        public GameObject hurtNumber;

        public void AoeCalculating(Vector3 aimingPoint,float damage, DamageType damageType,GameObject Owner)
        {
            Collider[] affectedColliders = Physics.OverlapSphere(aimingPoint, AoeRadius);
            foreach(var col in affectedColliders)
            {
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    Actor actor = col.gameObject.GetComponent<Actor>();
                    ActorProperties colliderProperty = actor.GetActorProperties();
                    float finalDamage = calculateDamage(colliderProperty, damage, damageType);
                    damageable.InflictDamage(finalDamage, false, Owner);

                    if (canDisplace)
                    {
                        //强制位移 可能击出地图
                        Vector3 displacementVector = (col.transform.position - aimingPoint).normalized;
                        col.gameObject.transform.Translate(displacementVector);

                        //DisplacementCalculating(aimingPoint, col);

                    }
                }
            }
        }

        //void DisplacementCalculating(Vector3 aimingPoint, Collider col)
        //{
        //    Rigidbody curRigidbody = col.GetComponentInChildren<Rigidbody>();
        //    Vector3 forceDirection = curRigidbody.transform.position - aimingPoint;
        //    Vector3 forceVector = forceDirection.normalized * displacementForce;
        //    curRigidbody.AddForce(forceVector, ForceMode.Impulse);


        private float calculateDamage(ActorProperties colliderProperty,float damage,DamageType damageType)
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

            GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            if (hurtNumber && hurtNumberParent)
            {
                Debug.Log("count!");
                GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
                hurt.transform.position = this.transform.position;
                hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            }

            return finalDamage;
        }
    }
}
