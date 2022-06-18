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

        public void AoeCalculating(Vector3 aimingPoint,float damage,GameObject Owner)
        {
            Collider[] affectedColliders = Physics.OverlapSphere(aimingPoint, AoeRadius);
            foreach(var col in affectedColliders)
            {
                Damageable damageable = col.GetComponent<Damageable>();
                if (damageable)
                {
                    Actor actor = col.gameObject.GetComponent<Actor>();
                    ActorProperties colliderProperty = actor.GetActorProperties();
                    float finalDamage = damage - colliderProperty.getPhysicalDefence() - colliderProperty.getMagicDefence();
                    if (finalDamage < 0f)
                        finalDamage = 0f;
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

        //}
    }
}
