
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {

        //[Tooltip("Speed")]
        public float speed;
        private float mDamage;
        //weapon Controller
        public GameObject Owner { get; private set; }
        public WeaponController WeaponController;
        // Start is called before the first frame update


        // for reuse
        public bool restart = false;

        // for shoot and auto destory
        public float m_ShootTime;
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }


        public float InitialCharge { get; private set; }

        public UnityAction OnShoot;

        // weapon should provide the speed, but...not implement
        public void Shoot(WeaponController controller)
        {

            speed = (float)controller.speed;
            // calculate damage

            ActorProperties tmp = controller.Owner.GetComponent<Actor>().GetActorProperties();

            float tmpDamage = tmp.getMagicAttack() + tmp.getPhysicalAttack();

            // calculate damage
            this.mDamage = tmpDamage + controller.damage;
            WeaponController = controller;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            OnShoot?.Invoke();
        }

        // if collision happens, it will be called
        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("weapon"))
            {
                return;
            }

            Damageable damageable = col.collider.GetComponent<Damageable>();
            if (damageable)
            {
                damageable.InflictDamage(this.mDamage, false, Owner);
                this.WeaponController.bulletPoolManager.release(this.gameObject);
            }
            this.WeaponController.bulletPoolManager.release(this.gameObject);
        }


    }

}