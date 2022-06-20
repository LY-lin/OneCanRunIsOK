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

        [Tooltip("the speed for displacement")]
        public float displacementSpeed = 1.0f;

        [Tooltip("the exist time for displacement")]
        public float displacementTime = 1.0f;

        [Tooltip("the max distance for displacement")]
        public float displacementDistance = 1.0f;

        //public GameObject hurtNumber;
        private DisplaceActionsManager displaceActionsManager;

        public void Awake()
        {
            displaceActionsManager= GameObject.FindObjectOfType<DisplaceActionsManager>();
            DebugUtility.HandleErrorIfNullFindObject<DisplaceActionsManager, AoeCalculator>(displaceActionsManager, this);
        }

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
                    damageable.InflictDamage(finalDamage, false, Owner, col.gameObject, damageType);

                    if (canDisplace && Owner != col.gameObject)
                    {
                        //强制位移 可能击出地图
                        //Vector3 displacementVector = (col.transform.position - aimingPoint).normalized;
                        //col.gameObject.transform.Translate(displacementVector);

                        //DisplacementCalculating(aimingPoint, col);
                        Vector3 displaceDestination = (col.transform.position - aimingPoint).normalized * displacementDistance + col.gameObject.transform.position;
                        DisplaceAction da = new DisplaceAction(col.gameObject, displaceDestination, Time.time, displacementTime, displacementSpeed);
                        displaceActionsManager.addAction(da);

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

            //GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
            //if (hurtNumber && hurtNumberParent)
            //{
            //    Debug.Log("count!");
            //    GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
            //    hurt.transform.position = this.transform.position;
            //    hurt.GetComponent<HurtNumber>().init(finalDamage, damageType);


            //}

            return finalDamage;
        }
    }
}
