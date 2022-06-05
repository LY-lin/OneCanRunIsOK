using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {
        public Vector3 hitPoint;
<<<<<<< Updated upstream
        public int speed;
=======
        //[Tooltip("Speed")]
        public static int speed = 50;

        //�����˺�ʱ�Ŀ��˺���Դ
        public GameObject Owner { get; private set; }
        public WeaponController weaponController;
>>>>>>> Stashed changes
        // Start is called before the first frame update
        void Start()
        {

<<<<<<< Updated upstream
        }
=======
        public float m_ShootTime;
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitialCharge { get; private set; }
        public bool restart = false;

        public UnityAction OnShoot;
>>>>>>> Stashed changes

        // Update is called once per frame
        void Update()
        {
            m_ShootTime = Time.time;
            
            weaponController = controller;
            

<<<<<<< Updated upstream
=======
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            OnShoot?.Invoke();
>>>>>>> Stashed changes
        }


        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "Enemy")
            {

                Destroy(this.gameObject);
            }
<<<<<<< Updated upstream
            else
            {
                Destroy(this.gameObject);
=======
            //if (col.gameObject.tag == "Enemy")
            //{
            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable)
            {
                //ProjectileBase m_ProjectileBase = new ProjectileBase();
                damageable.InflictDamage(10f, false, Owner);

                weaponController.bulletPoolManager.release(this.gameObject);
                //Destroy(this.gameObject);
>>>>>>> Stashed changes
            }

<<<<<<< Updated upstream
            Destroy(this.gameObject);
=======
            //
            weaponController.bulletPoolManager.release(this.gameObject);
            //Destroy(this.gameObject);
>>>>>>> Stashed changes
        }
    }

}