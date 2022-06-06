
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {

        public Vector3 hitPoint;
        //[Tooltip("Speed")]
        public static int speed = 50;

        //�����˺�ʱ�Ŀ��˺���Դ
        public GameObject Owner { get; private set; }
        public WeaponController WeaponController;
        // Start is called before the first frame update

        public bool restart = false;
        public float m_ShootTime;
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitialCharge { get; private set; }

        public UnityAction OnShoot;

        public void Shoot(WeaponController controller)
        {

            WeaponController = controller;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            OnShoot?.Invoke();
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("weapon"))
            {
                return;
            }
            //if (col.gameObject.tag == "Enemy")
            //{
            Debug.Log("boom!");
            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable)
            {
                //ProjectileBase m_ProjectileBase = new ProjectileBase();
                damageable.InflictDamage(10f, false, Owner);
                this.WeaponController.bulletPoolManager.release(this.gameObject);
            }
            //Destroy(this.gameObject);
            //}
            //else
            //{
            //    Destroy(this.gameObject);
            //}

            //
        }


    }

}