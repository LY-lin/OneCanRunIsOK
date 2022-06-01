using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {
        public Vector3 hitPoint;
        public int speed;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "Enemy")
            {

                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

}