using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game;
namespace OneCanRun.Game.Share
{
    public class BulletController : MonoBehaviour
    {
        public enum BulletType{
            Common,
            Ice,
            Napalm,
        }
        [Tooltip("击中特效")]
        public GameObject ImpactVfx;
        [Tooltip("击中特效持续时间")]
        public float ImpactVfxLifetime = 5f;
        public GameObject hurtNumber;
        public DamageType damageType;
        [Tooltip("if is Aoe")]
        public bool isAoe = false;

        [Tooltip("if is Aoe")]
        public BulletType bulletType = BulletType.Common;


        //[Tooltip("Speed")]
        public float speed;
        private float mDamage;
        private affiliationType shooterType;
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

        //public UnityAction OnShoot;

        // weapon should provide the speed, but...not implement
        public void Shoot(WeaponController controller)
        {
            //此处可以加入根据人物身上的buff情况调整子弹类型。

            speed = (float)controller.speed;
            // calculate damage

            ActorProperties tmp = controller.Owner.GetComponent<Actor>().GetActorProperties();
            this.shooterType = controller.Owner.GetComponent<Actor>().Affiliation;
            float tmpDamage = tmp.getMagicAttack() + tmp.getPhysicalAttack();

            // calculate damage
            this.mDamage = tmpDamage + controller.damage;
            WeaponController = controller;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;
            //Debug.Log("shoot"+ this.mDamage);
            //OnShoot?.Invoke();
        }

        // if collision happens, it will be called
        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("weapon") ||
                col.gameObject.layer == LayerMask.NameToLayer("physical dectect nut not collision"))
            {
                return;
            }


            Actor target = col.gameObject.GetComponent<Actor>();
            //if (target == null)
            //return;
            if (target)
            {
                if (target.Affiliation == this.shooterType)
                    return;
            }

            //特效
            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, this.gameObject.transform.position,
                    Quaternion.LookRotation(col.gameObject.transform.up));
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
                }
            }


            if (isAoe)
            {
                AoeCalculator ac = GetComponent<AoeCalculator>();
                Collider bulletCol = GetComponent<Collider>();
                ac.AoeCalculating(bulletCol.transform.position, mDamage, damageType, Owner);
                this.WeaponController.bulletPoolManager.release(this.gameObject);
            }
            else
            {
                Damageable damageable = col.collider.GetComponentInParent<Damageable>();
                if (damageable)
                {
                    Actor actor = col.gameObject.GetComponentInParent<Actor>();
                    
                    if(bulletType == BulletType.Ice)
                    {
                        Buff buff = GameObject.Find("IceBuff").GetComponent<Buff>();
                        BuffController iceBuff= new BuffController(buff);
                        ActorBuffManager actorBuffManager = col.gameObject.GetComponentInParent<ActorBuffManager>();
                        actorBuffManager.buffGain(iceBuff);
                    }


                    float finalDamage = calculateDamage(actor.GetActorProperties());
                    damageable.InflictDamage(finalDamage, false, Owner);

                    this.WeaponController.bulletPoolManager.release(this.gameObject);
                    GameObject hurtNumberParent = GameObject.Find("HurtNumberCollector");
                    if (hurtNumber && hurtNumberParent)
                    {
                        GameObject hurt = GameObject.Instantiate(hurtNumber, hurtNumberParent.transform);
                        hurt.transform.position = this.transform.position;
                        hurt.GetComponent<HurtNumber>().init(finalDamage, this.damageType);


                    }

                }
                this.WeaponController.bulletPoolManager.release(this.gameObject);
            }
        }


        private float calculateDamage(ActorProperties colliderProperty)
        {
            if (colliderProperty == null)
                return 0;
            float finalDamage = 0;
            if (this.damageType == DamageType.magic)
            {
                finalDamage = this.mDamage - colliderProperty.getMagicDefence();
            }
            else
            {
                finalDamage = this.mDamage - colliderProperty.getPhysicalDefence();
            }
            if (finalDamage < 0f)
                finalDamage = 0f;

            return finalDamage;
        }
    }


}