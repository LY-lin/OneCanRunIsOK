using System;

using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{

    public enum WeaponShootType
    {
        Manual,
        Automatic,
        Charge,
        Laser,
    }


    // [RequireComponent(typeof(AudioSource))]  //加入声源
    public class WeaponController : MonoBehaviour
    {
        public bool isPlayer = true;
        public Share.DamageType damageType;

        [Header("Information")]
        [Tooltip("if it is RemoteWeapons")]
        public bool RemoteWeapons;

        [Tooltip("The name that will be displayed in the UI for this weapon")]
        public string WeaponName;   //武器名


        [Tooltip("The image that will be displayed in the UI for this weapon")]
        public Sprite WeaponIcon;   //武器显示在UI的小图标

        [Tooltip("Image that show the weapon root")]
        public Sprite WeaponImg;

        [Tooltip("Weapon's Description")]
        public string description;


        [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
        public GameObject WeaponRoot;   //武器使用的模型

        [Header("Melee Weapons' Internal Reference(Only active when RemoteWeapons is false)")]

        [Tooltip("Delay between two attacks")]
        public float DelayBetweenAttacks;

        [Header("武器伤害，参与最终的伤害计算")]
        [Tooltip("damage")]
        public float damage;

        

        [Header("Remote Weapons' Internal References")]
        [Tooltip("Tip of the weapon, where the projectiles are shot")]
        public Transform WeaponMuzzle;  //武器发射弹丸的地方

        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon wil affect how it shoots")]
        public WeaponShootType ShootType;

        [Tooltip("Bullet profebs")]
        public GameObject bullet;


        [Tooltip("Minimum duration between two shots")]
        public float DelayBetweenShots = 0.5f;  //两次射击之间的最短间隔

        [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
        public float BulletSpreadAngle = 0f;    //“子弹随机射击的圆锥体角度, 扩散(0表示完全没有扩散)

        [Tooltip("Amount of bullets per shot")]
        public int BulletsPerShot = 1;  //每次射击的子弹数

        [Tooltip("Force that will push back the weapon after each shot")]
        [Range(0f, 2f)]
        public float RecoilForce = 1;//后坐力


        [Header("GunAmmo Parameters")]//枪支武器
        [Tooltip("Should the player manually reload")]//玩家手动装弹
        public bool AutomaticReload = true;//默认自动换弹

        [Tooltip("Has physical clip on the weapon and ammo shells are ejected when firing")]
        public bool HasPhysicalBullets = false;//武器上有物理弹夹吗?发射时弹壳会弹出吗

        
        [Tooltip("the speed of shooting out")]
        public float speed = 100f;

        [Tooltip("Number of bullets in a clip")]
        public int ClipSize = 30;//单个弹匣的子弹数量


        [Tooltip("Force applied on the shell")]
        [Range(0.0f, 5.0f)] public float ShellCasingEjectionForce = 2.0f;
        //施加在壳上的力

        [Tooltip("Maximum number of shell that can be spawned before reuse")]
        [Range(1, 30)] public int ShellPoolSize = 1;//在装弹之前可以生成的最大shell数

        [Tooltip("Amount of ammo reloaded per second")]
        public float AmmoReloadRate = 1f;//每秒装弹的数量

        [Tooltip("Delay after the last shot before starting to reload")]
        public float AmmoReloadDelay = 2f;//在最后一枪后延迟开始装填

        [Tooltip("Maximum amount of ammo in the gun")]
        public int MaxAmmo = 8;//枪中最大的子弹数量（最大备弹）


        //充能类武器参数
        [Header("Charging parameters (charging weapons only)")]
        [Tooltip("Trigger a shot when maximum charge is reached")]
        public bool AutomaticReleaseOnCharged;//是否达到最大充能时触发一次射击

        [Tooltip("Duration to reach maximum charge")]
        public float MaxChargeDuration = 2f;//到达最大充能时的耐久

        [Tooltip("Initial ammo used when starting to charge")]
        public float AmmoUsedOnStartCharge = 1f;//启动武器时使用的子弹量

        [Tooltip("Additional ammo used when charge reaches its maximum")]
        public float AmmoUsageRateWhileCharging = 1f;//到达最大充能使用的子弹

        [Header("Laser parameters (laser weapons only)")]
        [Tooltip("Lasering cost per second")]
        public float LaseringCost = 2f;

        [Header("About Discard")]
        [Tooltip("Discard prefab")]
        public GameObject DiscardPrefab;

        [Header("Audio & Visual")]
        [Tooltip("Optional weapon animator for OnShoot animations")]
        public Animator WeaponAnimator;//武器动画

        [Tooltip("Prefab of the muzzle flash")]
        public GameObject MuzzleFlashPrefab;//预制的枪口闪光，枪口的焰火

        [Tooltip("Unparent the muzzle flash instance on spawn")]
        public bool UnparentMuzzleFlash;//是否在子弹出去时去掉炮口闪光实例

        [Tooltip("sound played when shooting")]
        public AudioClip ShootSfx;//射击时的音频片段



        bool m_WantsToShoot = false;

        float OrginDamage;

        GameObject muzzleChargeInstance;
        //射击时的Action
        public UnityAction OnShoot;
        public event Action OnShootProcessed;

        int m_CarriedPhysicalBullets;//携带的物理子弹量
        float m_CurrentAmmo;    //现在的弹药
        float m_LastTimeShot = Mathf.NegativeInfinity;  //上次射击的实现 = 负无穷大
        public float LastChargeTriggerTimestamp { get; private set; }   //上次触发充能的时间戳
        Vector3 m_LastMuzzlePosition;   //上次枪口的位置

        public BulletPoolManager bulletPoolManager;
        // Update is called once per frame

        float m_LastTimeAttack = Mathf.NegativeInfinity;    //  上次攻击结束时间



        public GameObject Owner { get; set; }           //拥有者
        public GameObject SourcePrefab { get; set; }    //源预制件  
        public bool IsCharging { get; private set; }    //是否在充能（充能武器）
        public float CurrentAmmoRatio { get; private set; } //现在弹药的比例
        public bool IsWeaponActive { get; private set; }    //武器是否可动
        public bool IsCooling { get; private set; }         //是否在冷却中
        public float CurrentCharge { get; private set; }    //？
        public Vector3 MuzzleWorldVelocity { get; private set; }    //枪口在时世界中的速率

        public float GetAmmoNeededToShoot() =>
            (ShootType != WeaponShootType.Charge ?
            1f : Mathf.Max(1f, AmmoUsedOnStartCharge)) /
            (MaxAmmo * BulletsPerShot);//获得单次射击需要的子弹数

        public int GetCarriedPhysicalBullets() => m_CarriedPhysicalBullets;
        public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);

        AudioSource m_ShootAudioSource;

        public bool IsReloading { get; private set; }

        public bool IsAttacking { get; private set; }

        const string k_AnimAttackParameter = "Attack";

        private bool isLasering = false;
        private LaserController lc;



        void Awake()
        {
            Owner = this.transform.gameObject;
            if (RemoteWeapons)
            {
                if (ShootType!=WeaponShootType.Laser)
                {
                    this.bulletPoolManager = new BulletPoolManager(this.bullet);
                }
                else
                {
                    lc = bullet.GetComponent<LaserController>();
                    lc.Owner = this.Owner;
                    lc.LaserSocket = WeaponMuzzle;
                }

                m_CurrentAmmo = HasPhysicalBullets ? ClipSize : MaxAmmo;
                MaxAmmo = HasPhysicalBullets ? ClipSize : MaxAmmo;
                m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
                m_LastMuzzlePosition = WeaponMuzzle.position;


                m_ShootAudioSource = GetComponent<AudioSource>();
                DebugUtility.HandleErrorIfNullGetComponent<AudioSource, WeaponController>(m_ShootAudioSource, this,
                    gameObject);
                CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;

                OrginDamage = damage;
            }



        }
        public void InitMelee()
        {
            GetComponent<MeleeController>().Init(this);
        }




        public void Reload()   //??????????
        {

            m_CurrentAmmo = ClipSize;
            m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
            IsReloading = false;

        }

        public void StartReloadAnimation()  //???????
        {

            GetComponent<Animator>().SetTrigger("Reload");
            IsReloading = true;

        }

        void Update()
        {
            if (RemoteWeapons)
            {
                UpdateAmmo();
                UpdateCharge();
                UpdateLaser();

                if (Time.deltaTime > 0)
                {
                    MuzzleWorldVelocity = (WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
                    m_LastMuzzlePosition = WeaponMuzzle.position;
                }
            }

        }

        void UpdateAmmo()
        {


            if (!HasPhysicalBullets && AutomaticReload && m_LastTimeShot + AmmoReloadDelay < Time.time && m_CurrentAmmo < MaxAmmo && !IsCharging)
            {

                // reloads weapon over time
                m_CurrentAmmo += AmmoReloadRate * Time.deltaTime;

                // limits ammo to max value
                m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo, 0, MaxAmmo);

                IsCooling = true;
            }
            else
            {
                IsCooling = false;
            }

            if (MaxAmmo == Mathf.Infinity)
            {
                CurrentAmmoRatio = 1f;
            }
            else
            {
                CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;
            }
        }

        void UpdateCharge()
        {
            if (IsCharging)
            {
                if (CurrentCharge < 1f)
                {
                    float chargeLeft = 1f - CurrentCharge;

                    // Calculate how much charge ratio to add this frame
                    float chargeAdded = 0f;
                    if (MaxChargeDuration <= 0f)
                    {
                        chargeAdded = chargeLeft;
                    }
                    else
                    {
                        chargeAdded = (1f / MaxChargeDuration) * Time.deltaTime;
                    }

                    chargeAdded = Mathf.Clamp(chargeAdded, 0f, chargeLeft);

                    // See if we can actually add this charge
                    float ammoThisChargeWouldRequire = chargeAdded * AmmoUsageRateWhileCharging;
                    if (ammoThisChargeWouldRequire <= m_CurrentAmmo)
                    {
                        // Use ammo based on charge added
                        UseAmmo(ammoThisChargeWouldRequire);

                        // set current charge ratio
                        CurrentCharge = Mathf.Clamp01(CurrentCharge + chargeAdded);
                    }
                }
            }
        }

        void UpdateLaser()
        {
            if (isLasering)
            {
                if (m_CurrentAmmo <= 0)
                {
                    isLasering = false;
                    lc.StopLaser();
                }
                UseAmmo(LaseringCost*Time.deltaTime);
            }
        }


        public void ShowWeapon(bool show)
        {
            WeaponRoot.SetActive(show);


            IsWeaponActive = show;
        }

        public void UseAmmo(float amount)
        {
            m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo - amount, 0f, MaxAmmo);
            //m_CarriedPhysicalBullets -= Mathf.RoundToInt(amount);
            m_CarriedPhysicalBullets = Mathf.Clamp(m_CarriedPhysicalBullets, 0, MaxAmmo);
            m_LastTimeShot = Time.time;
        }

        public bool HandleAttackInputs(bool inputDown, bool inputHeld)
        {
            
            if ((inputDown || inputHeld) && !IsAttacking)
            {
                return TryAttack();
            }
            return false;
        }
        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            m_WantsToShoot = inputDown || inputHeld;
            switch (ShootType)
            {
                case WeaponShootType.Manual:
                    if (inputDown)
                    {
                        return TryShoot();
                    }

                    return false;

                case WeaponShootType.Automatic:
                    if (inputHeld)
                    {

                        return TryShoot();
                    }

                    return false;

                case WeaponShootType.Charge:
                    if (inputHeld)
                    {
                        TryBeginCharge();
                    }

                    // Check if we released charge or if the weapon shoot autmatically when it's fully charged
                    if (inputUp || (AutomaticReleaseOnCharged && CurrentCharge >= 1f))
                    {
                        damage = (int)(damage * CurrentCharge);
                        return TryReleaseCharge();
                    }

                    return false;

                case WeaponShootType.Laser:
                    if (inputDown && !isLasering)
                    {
                        //获取玩家的摄像头
                        if (isPlayer)
                        {
                            lc.StartLaser(this.Owner.GetComponentInChildren<AudioListener>().transform);
                        }
                        else
                        {
                            lc.StartLaser();
                        }
                        isLasering = true;
                        return true;
                    }
                    else if(inputUp && isLasering)
                    {
                        lc.StopLaser();
                        isLasering = false;
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }

        bool TryAttack()
        {
            if (m_LastTimeShot + DelayBetweenAttacks < Time.time)
            {
                HandleAttack();
                return true;
            }
            return false;
        }
        bool TryShoot()
        {

            if (m_CurrentAmmo >= 1f
                && m_LastTimeShot + DelayBetweenShots < Time.time)
            {
                HandleShoot();
                m_CurrentAmmo -= 1f;
                m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
                return true;
            }

            return false;
        }


        bool TryBeginCharge()
        {
            damage = OrginDamage;
            if (!IsCharging
                && m_CurrentAmmo >= AmmoUsedOnStartCharge
                && Mathf.FloorToInt((m_CurrentAmmo - AmmoUsedOnStartCharge) * BulletsPerShot) > 0
                && m_LastTimeShot + DelayBetweenShots < Time.time)
            {
                UseAmmo(AmmoUsedOnStartCharge);

                LastChargeTriggerTimestamp = Time.time;
                IsCharging = true;
                GetComponent<Animator>().SetTrigger("Attack");
                // Unparent the muzzleFlashInstance
                if (UnparentMuzzleFlash)
                {
                    muzzleChargeInstance.transform.SetParent(null);
                }
                return true;
            }

            return false;
        }

        bool TryReleaseCharge()
        {
            if (IsCharging)
            {
                HandleShoot();

                CurrentCharge = 0f;
                IsCharging = false;
                GetComponent<Animator>().SetTrigger("EndAttack");
                Destroy(muzzleChargeInstance);
                return true;
            }

            return false;
        }
        void EndAttack()
        {
            m_LastTimeAttack = Time.time;
            IsAttacking = false;

        }
        void HandleAttack()
        {
            GetComponent<Animator>().SetTrigger("Attack");
            IsAttacking = true;

        }
        void HandleShoot()
        {
            int bulletsPerShotFinal = ShootType == WeaponShootType.Charge
                ? Mathf.CeilToInt(CurrentCharge * BulletsPerShot)
                : BulletsPerShot;

            // spawn all bullets with random direction
            for (int i = 0; i < bulletsPerShotFinal; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);

                trigger(shotDirection);
            }

            // muzzle flash
            if (MuzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position,
                    WeaponMuzzle.rotation, WeaponMuzzle.transform);
                // Unparent the muzzleFlashInstance
                if (UnparentMuzzleFlash)
                {
                    muzzleFlashInstance.transform.SetParent(null);
                }

                Destroy(muzzleFlashInstance, 2f);
            }

            if (HasPhysicalBullets)
            {
                m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
            }

            m_LastTimeShot = Time.time;

            // Trigger attack animation if there is any
            if (WeaponAnimator)
            {
                WeaponAnimator.SetTrigger(k_AnimAttackParameter);
            }

            OnShoot?.Invoke();
            OnShootProcessed?.Invoke();
        }
        public float GetMyCurrentAmmo()
        {
            return m_CurrentAmmo;
        }
        public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = BulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }



        private void trigger(Vector3 shootDirection)
        {

            // initialization
            GameObject tempBullet = bulletPoolManager.getObject(WeaponMuzzle.position, Quaternion.LookRotation(shootDirection));
            tempBullet.transform.position = WeaponMuzzle.position;
            tempBullet.transform.rotation = Quaternion.LookRotation(shootDirection);
            tempBullet.transform.position = WeaponMuzzle.position;
            tempBullet.GetComponent<BulletController>().damageType = this.damageType;
            Vector3 testForRotation = new Vector3(0, 0, 0);
            tempBullet.transform.rotation = new Quaternion();


            tempBullet.transform.forward = shootDirection;
            tempBullet.GetComponent<BulletController>().Shoot(this);



        }

        public void UpdateOwner(GameObject owner)
        {
            this.Owner = owner;
            if (ShootType == WeaponShootType.Laser)
            {
                lc.Owner=owner;
            }
        }
    }
}