using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class DroneController : MonoBehaviour
    {
        // Start is called before the first frame update

        //������Χ
        public float attackRange = 30f;
        //ת���ٶ�
        public float orientationSpeed = 30f;
        //�������
        public float attackDelay = 1.0f;
        private float lastAttack = Mathf.NegativeInfinity;

        public GameObject targetObject;
        public WeaponController weaponController;


        //public GameObject Owner;


        //void Start()
        //{
        
        //}

        // Update is called once per frame
        void Update()
        {
            if (targetObject)
            {
                if (Vector3.Distance(transform.position, targetObject.transform.position) > attackRange)
                {
                    targetObject = null;
                    Detection();
                }
                else
                {
                    OrientTowards(targetObject.transform.position);
                    HandleAttack();
                }
            }
            else
            {
                Detection();
            }
        }

        void OrientTowards(Vector3 lookPosition)
        {
           //�������߷���
           Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * orientationSpeed);
            }
        }

        void Detection()
        {
            //�������
            Collider[] affectedColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach(var col in affectedColliders)
            {
                Actor actor = col.GetComponent<Actor>();
                if (actor && actor.Affiliation != affiliationType.allies)
                {
                    targetObject = actor.gameObject;
                    return;
                }
            }
        }

        void HandleAttack()
        {
            if (lastAttack + attackDelay > Time.time)
            {
                return;
            }
            lastAttack = Time.time;

            Vector3 weaponForward = (targetObject.transform.position - weaponController.WeaponRoot.transform.position).normalized;
            weaponController.transform.forward = weaponForward;

            if (weaponController.ShootType == WeaponShootType.Manual)
            {
                weaponController.HandleShootInputs(true, false, false);
            }
        }
    }
}
