using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        public Vector3 hitPoint;
        //[Tooltip("Speed")]
        public static int speed = 50;

        //≤‚ ‘…À∫¶ ±µƒø’…À∫¶¿¥‘¥
        public GameObject Owner { get; set; }
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {

        }


        void OnCollisionEnter(Collision col)
        {
            //if (col.gameObject.tag == "Enemy")
            //{
            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable)
            {
                //ProjectileBase m_ProjectileBase = new ProjectileBase();
                damageable.InflictDamage(10f, false, Owner);
            }
            //Destroy(this.gameObject);
            //}
            //else
            //{
            //    Destroy(this.gameObject);
            //}

            //Destroy(this.gameObject);
        }


    }

}