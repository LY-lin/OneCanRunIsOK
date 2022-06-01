using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        
=======
        public Vector3 hitPoint;
        public int speed;

        //²âÊÔÉËº¦Ê±µÄ¿ÕÉËº¦À´Ô´
        public GameObject Owner { get; set; }
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
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
