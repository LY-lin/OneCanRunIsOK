using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class WeaponController : MonoBehaviour
    {
        public GameObject bullet;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update(){


        }

        private void trigger(){
            GameObject tmp = new GameObject();


<<<<<<< Updated upstream
=======
            float currentSpread = Mathf.Lerp(0.0f, 10, 2 / 1);
            
            fireRotation = Quaternion.RotateTowards(fireRotation, UnityEngine.Random.rotation, UnityEngine.Random.Range(0.0f, currentSpread));

            Physics.Raycast(transform.position, fireRotation * Vector3.forward, out hit, Mathf.Infinity);

            {
                GameObject tempBullet = Instantiate(bullet, WeaponMuzzle.position, fireRotation);
                tempBullet.GetComponent<BulletController>().hitPoint = hit.point;
                
                tempBullet.GetComponent<Rigidbody>().AddForce(  5f*WeaponMuzzle.forward.normalized);
            }
>>>>>>> Stashed changes
        }
    }

}