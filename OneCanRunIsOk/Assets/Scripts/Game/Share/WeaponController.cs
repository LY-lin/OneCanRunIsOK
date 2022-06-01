using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class WeaponController : MonoBehaviour
    {
        public GameObject bullet;

        public float cooldownSpeed;

        public float fireRate;

        public float recoilCooldown;

        private float accuracy;

        public float maxSpreadAngle;

        public float timeTillMaxSpread;


        public GameObject shootPoint;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update(){

            if (Input.GetKeyDown(KeyCode.Space)){
                trigger();
            }

        }



        private void trigger()
        {
            RaycastHit hit;

            Quaternion fireRotation = Quaternion.LookRotation(transform.forward);

            float currentSpread = Mathf.Lerp(0.0f, maxSpreadAngle, accuracy / timeTillMaxSpread);

            fireRotation = Quaternion.RotateTowards(fireRotation, Random.rotation, Random.Range(0.0f, currentSpread));

            Physics.Raycast(transform.position, fireRotation * Vector3.forward, out hit, Mathf.Infinity);

            {
                GameObject tempBullet = Instantiate(bullet, shootPoint.transform.position, fireRotation);
                tempBullet.GetComponent<BulletController>().hitPoint = hit.point;
            }
        }
    }

}