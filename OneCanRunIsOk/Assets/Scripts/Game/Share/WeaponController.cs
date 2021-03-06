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

    [System.Serializable]
    public struct CrosshairData //准心的信息
    {
        [Tooltip("The image that will be used for this weapon's crosshair")]
        public Sprite CrosshairSprite;



        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color CrosshairColor;
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


        [Tooltip("Default data for the crosshair")]
        public CrosshairData CrosshairDataDefault;//默认的准心数据

        /*
        [Tooltip("Data for the crosshair when targeting an enemy")]
        public CrosshairData CrosshairDataTargetInSight;//瞄准时的准信数据
        */

        [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
        public GameObject WeaponRoot;   //武器使用的模型

        [Header("Melee Weapons' Internal Reference(Only active when RemoteWeapons is false)")]
        //[Tooltip("Damagable Box")]
        //public GameObject DamagableBox;

        [Tooltip("Delay between two attacks")]
        public float DelayBetweenAttacks;

        [Header("武器伤害，参与最终的伤害计算")]
        [Tooltip("damage")]
        public float damage;

        float OrginDamage;

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

        /* [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
         [Range(0f, 1f)]
         public float AimZoomRatio = 1f; //瞄准时候的FOV比例
         [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
         public Vector3 AimOffset;*/
        //当用这种武器瞄准时，平移过来适用于武器臂

        [Header("GunAmmo Parameters")]//枪支武器
        [Tooltip("Should the player manually reload")]//玩家手动装弹
        public bool AutomaticReload = true;//默认自动换弹

        [Tooltip("Has physical clip on the weapon and ammo shells are ejected when firing")]
        public bool HasPhysicalBullets = false;//武器上有物理弹夹吗?发射时弹壳会弹出吗

        GameObject muzzleChargeInstance;

        public float speed = 100f;

        [Tooltip("Number of bullets in a clip")]
        public int ClipSize = 30;//单个弹匣的子弹数量

        [Tooltip("Bullet Shell Casing")]
        public GameObject ShellCasing;//枪管物件

        [Tooltip("Weapon Ejection Port for physical ammo")]
        public Transform EjectionPort;//武器弹射端口用于物理弹药

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

        [Tooltip("Sound played when changing to this weapon")]
        public AudioClip ChangeWeaponSfx;//充能武器时的音频片段


        [Tooltip("Prefab of the charge flash")]
        public GameObject MuzzleChargePrefab;//预制的枪口闪光，枪口的焰火

        [Tooltip("Continuous Shooting Sound")]
        public bool UseContinuousShootSound = false;//是否时许产生持续的音效

        /*  //持续音频的设计
        public AudioClip ContinuousShootStartSfx;
        public AudioClip ContinuousShootLoopSfx;
        public AudioClip ContinuousShootEndSfx;
        AudioSource m_ContinuousShootAudioSource = null;*/
        bool m_WantsToShoot = false;


        //射击时的Action
        public UnityAction OnShoot;
        public event Action OnShootProcessed;

        int m_CarriedPhysicalBullets;//携带的物理子弹量
        float m_CurrentAmmo;    //现在的弹药
        float m_LastTimeShot = Mathf.NegativeInfinity;  //上次射击的实现 = 负无穷大
        public float LastChargeTriggerTimestamp { get; private set; }   //上次触发充能的时间戳
        Vector3 m_LastMuzzlePosition;   //上次枪口的位置


        public CachePool cachePool;
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
                if (ShootType != WeaponShootType.Laser)
                {
                    this.bulletPoolManager = new BulletPoolManager(this.bullet);
                    //CacheItem cacheBullet = new CacheBullet(bullet);
                    //this.cachePool = new CachePool(128, cacheBullet);
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
                //m_CarriedPhysicalBullets = HasPhysicalBullets ? ClipSize : 0;//?��????????????????????????0
                m_LastMuzzlePosition = WeaponMuzzle.position;


                m_ShootAudioSource = GetComponent<AudioSource>();
                DebugUtility.HandleErrorIfNullGetComponent<AudioSource, WeaponController>(m_ShootAudioSource, this,
                    gameObject);
                CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;

                OrginDamage = damage;
            }


            /*
            if (UseContinuousShootSound)
            {
                m_ContinuousShootAudioSource = gameObject.AddComponent<AudioSource>();
                m_ContinuousShootAudioSource.playOnAwake = false;
                m_ContinuousShootAudioSource.clip = ContinuousShootLoopSfx;
                m_ContinuousShootAudioSource.outputAudioMixerGroup =
                    AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponShoot);
                m_ContinuousShootAudioSource.loop = true;
            }*/


        }
        public void InitMelee()
        {
            //DamagableBox.GetComponent<MeleeController>().Init(this);
            //DamagableBox.gameObject.SetActive(false);
            GetComponent<MeleeController>().Init(this);
        }
        //PickUp????????
        //public void AddCarriablePhysicalBullets(int count) => m_CarriedPhysicalBullets = Mathf.Max(m_CarriedPhysicalBullets + count, MaxAmmo);

        void ShootShell()//????????
        {
            /*
            Rigidbody nextShell = m_PhysicalAmmoPool.Dequeue();
            nextShell.transform.position = EjectionPort.transform.position;
            nextShell.transform.rotation = EjectionPort.transform.rotation;
            nextShell.gameObject.SetActive(true);
            nextShell.transform.SetParent(null);
            nextShell.collisionDetectionMode = CollisionDetectionMode.Continuous;
            nextShell.AddForce(nextShell.transform.up * ShellCasingEjectionForce, ForceMode.Impulse);
            m_PhysicalAmmoPool.Enqueue(nextShell);
            */

            //Bullet.trigger();
        }

        //void PlaySFX(AudioClip sfx) => AudioUtility.CreateSFX(sfx, transform.position, AudioUtility.AudioGroups.WeaponShoot, 0.0f);


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
                UpdateContinuousShootSound();

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
                UseAmmo(LaseringCost * Time.deltaTime);
            }
        }

        void UpdateContinuousShootSound()
        {/*
            if (UseContinuousShootSound)
            {
                if (m_WantsToShoot && m_CurrentAmmo >= 1f)
                {
                    if (!m_ContinuousShootAudioSource.isPlaying)
                    {
                        m_ShootAudioSource.PlayOneShot(ShootSfx);
                        m_ShootAudioSource.PlayOneShot(ContinuousShootStartSfx);
                        m_ContinuousShootAudioSource.Play();
                    }
                }
                else if (m_ContinuousShootAudioSource.isPlaying)
                {
                    m_ShootAudioSource.PlayOneShot(ContinuousShootEndSfx);
                    m_ContinuousShootAudioSource.Stop();
                }
            }*/
        }

        public void ShowWeapon(bool show)
        {
            WeaponRoot.SetActive(show);

            if (show && ChangeWeaponSfx)
            {
                // m_ShootAudioSource.PlayOneShot(ChangeWeaponSfx);
            }

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
            //if ((inputDown || inputHeld)&&!DamagableBox.gameObject.activeSelf)
            //{
            //    return TryAttack();
            //}
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
                        //Debug.Log("CurrentCharge:" + CurrentCharge);
                        //Debug.Log("weapon damage:" + damage);
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
                    else if (inputUp && isLasering)
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
                muzzleChargeInstance = Instantiate(MuzzleChargePrefab, WeaponMuzzle.position,
                    WeaponMuzzle.rotation, WeaponMuzzle.transform);
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

            //DamagableBox.GetComponent<MeleeController>().ReleaseDic();
            //DamagableBox.gameObject.SetActive(false);
            m_LastTimeAttack = Time.time;
            IsAttacking = false;

        }
        void HandleAttack()
        {
            //DamagableBox.gameObject.SetActive(true);
            //DamagableBox.GetComponent<MeleeController>().Init(this);
            GetComponent<Animator>().SetTrigger("Attack");
            IsAttacking = true;
            //m_ShootAudioSource.PlayOneShot(WeaponAttackSfx);

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
                //?????��?????
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
                //ShootShell();
                //m_CarriedPhysicalBullets--;
                m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
            }

            m_LastTimeShot = Time.time;

            // play shoot SFX
            if (ShootSfx && !UseContinuousShootSound)
            {
                m_ShootAudioSource.PlayOneShot(ShootSfx);
            }

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
            //GameObject tempBullet = cachePool.getObject().cacheObject;
            tempBullet.transform.position = WeaponMuzzle.position;
            tempBullet.transform.rotation = Quaternion.LookRotation(shootDirection);
            tempBullet.transform.position = WeaponMuzzle.position;
            tempBullet.GetComponent<BulletController>().damageType = this.damageType;
            Vector3 testForRotation = new Vector3(0, 0, 0);
            tempBullet.transform.rotation = new Quaternion();


            tempBullet.transform.forward = shootDirection;
            // Debug.Log(shootDirection);
            tempBullet.GetComponent<BulletController>().Shoot(this);
            //tempBullet.GetComponent<BulletController>().OnShoot?.Invoke();



        }

        public void UpdateOwner(GameObject owner)
        {
            this.Owner = owner;
            if (ShootType == WeaponShootType.Laser)
            {
                lc.Owner = owner;
            }
        }
    }
}