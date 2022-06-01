using System;
using System.Collections;
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
    }

    [System.Serializable]
    public struct CrosshairData //׼�ĵ���Ϣ
    {
        [Tooltip("The image that will be used for this weapon's crosshair")]
        public Sprite CrosshairSprite;

        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color CrosshairColor;
    }

    // [RequireComponent(typeof(AudioSource))]  //������Դ
    public class WeaponController : MonoBehaviour
    {
        

        [Header("Information")]
        [Tooltip("The name that will be displayed in the UI for this weapon")]
        public string WeaponName;   //������

        [Tooltip("The image that will be displayed in the UI for this weapon")]
        public Sprite WeaponIcon;   //������ʾ��UI��Сͼ��

        [Tooltip("Bullet profebs")]
        public GameObject bullet;

        [Tooltip("Default data for the crosshair")]
        public CrosshairData CrosshairDataDefault;//Ĭ�ϵ�׼������

        /*
        [Tooltip("Data for the crosshair when targeting an enemy")]
        public CrosshairData CrosshairDataTargetInSight;//��׼ʱ��׼������
        */

        [Header("Internal References")]
        [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
        public GameObject WeaponRoot;   //����ʹ�õ�ģ��

        [Tooltip("Tip of the weapon, where the projectiles are shot")]
        public Transform WeaponMuzzle;  //�������䵯��ĵط�

        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon wil affect how it shoots")]
        public WeaponShootType ShootType;

        /*
        [Tooltip("The projectile prefab")] 
        public ProjectileBase ProjectilePrefab;//�ӵ���Ԥ�Ƽ�*/

        [Tooltip("Minimum duration between two shots")]
        public float DelayBetweenShots = 0.5f;  //�������֮�����̼��

        [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
        public float BulletSpreadAngle = 0f;    //���ӵ���������Բ׶��Ƕ�, ��ɢ(0��ʾ��ȫû����ɢ)

        [Tooltip("Amount of bullets per shot")]
        public int BulletsPerShot = 1;  //ÿ��������ӵ���

        [Tooltip("Force that will push back the weapon after each shot")]
        [Range(0f, 2f)]
        public float RecoilForce = 1;//������

        /* [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
         [Range(0f, 1f)]
         public float AimZoomRatio = 1f; //��׼ʱ���FOV����

         [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
         public Vector3 AimOffset;*/
        //��������������׼ʱ��ƽ�ƹ���������������

        [Header("GunAmmo Parameters")]//ǹ֧����
        [Tooltip("Should the player manually reload")]//����ֶ�װ��
        public bool AutomaticReload = true;//Ĭ���Զ�����

        [Tooltip("Has physical clip on the weapon and ammo shells are ejected when firing")]
        public bool HasPhysicalBullets = false;//����������������?����ʱ���ǻᵯ����


        [Tooltip("Number of bullets in a clip")]
        public int ClipSize = 30;//������ϻ���ӵ�����

        [Tooltip("Bullet Shell Casing")]
        public GameObject ShellCasing;//ǹ�����

        [Tooltip("Weapon Ejection Port for physical ammo")]
        public Transform EjectionPort;//��������˿���������ҩ

        [Tooltip("Force applied on the shell")]
        [Range(0.0f, 5.0f)] public float ShellCasingEjectionForce = 2.0f;
        //ʩ���ڿ��ϵ���

        [Tooltip("Maximum number of shell that can be spawned before reuse")]
        [Range(1, 30)] public int ShellPoolSize = 1;//��װ��֮ǰ�������ɵ����shell��

        [Tooltip("Amount of ammo reloaded per second")]
        public float AmmoReloadRate = 1f;//ÿ��װ��������

        [Tooltip("Delay after the last shot before starting to reload")]
        public float AmmoReloadDelay = 2f;//�����һǹ���ӳٿ�ʼװ��

        [Tooltip("Maximum amount of ammo in the gun")]
        public int MaxAmmo = 8;//ǹ�������ӵ���������󱸵���


        //��������������
        [Header("Charging parameters (charging weapons only)")]
        [Tooltip("Trigger a shot when maximum charge is reached")]
        public bool AutomaticReleaseOnCharged;//�Ƿ�ﵽ������ʱ����һ�����

        [Tooltip("Duration to reach maximum charge")]
        public float MaxChargeDuration = 2f;//����������ʱ���;�

        [Tooltip("Initial ammo used when starting to charge")]
        public float AmmoUsedOnStartCharge = 1f;//��������ʱʹ�õ��ӵ���

        [Tooltip("Additional ammo used when charge reaches its maximum")]
        public float AmmoUsageRateWhileCharging = 1f;//����������ʹ�õ��ӵ�



        [Header("Audio & Visual")]
        [Tooltip("Optional weapon animator for OnShoot animations")]
        public Animator WeaponAnimator;//��������

        [Tooltip("Prefab of the muzzle flash")]
        public GameObject MuzzleFlashPrefab;//Ԥ�Ƶ�ǹ�����⣬ǹ�ڵ����

        [Tooltip("Unparent the muzzle flash instance on spawn")]
        public bool UnparentMuzzleFlash;//�Ƿ����ӵ���ȥʱȥ���ڿ�����ʵ��

        [Tooltip("sound played when shooting")]
        public AudioClip ShootSfx;//���ʱ����ƵƬ��

        [Tooltip("Sound played when changing to this weapon")]
        public AudioClip ChangeWeaponSfx;//��������ʱ����ƵƬ��

        [Tooltip("Continuous Shooting Sound")]
        public bool UseContinuousShootSound = false;//�Ƿ�ʱ�������������Ч

        /*  //������Ƶ�����
        public AudioClip ContinuousShootStartSfx;
        public AudioClip ContinuousShootLoopSfx;
        public AudioClip ContinuousShootEndSfx;
        AudioSource m_ContinuousShootAudioSource = null;*/
        bool m_WantsToShoot = false;
        

        //���ʱ��Action
        public UnityAction OnShoot;
        public event Action OnShootProcessed;

        int m_CarriedPhysicalBullets;//Я���������ӵ���
        float m_CurrentAmmo;    //���ڵĵ�ҩ
        float m_LastTimeShot = Mathf.NegativeInfinity;  //�ϴ������ʵ�� = �������
        public float LastChargeTriggerTimestamp { get; private set; }   //�ϴδ������ܵ�ʱ���
        Vector3 m_LastMuzzlePosition;   //�ϴ�ǹ�ڵ�λ��
        // Update is called once per frame

        public GameObject Owner { get; set; }           //ӵ����
        public GameObject SourcePrefab { get; set; }    //ԴԤ�Ƽ�  
        public bool IsCharging { get; private set; }    //�Ƿ��ڳ��ܣ�����������
        public float CurrentAmmoRatio { get; private set; } //���ڵ�ҩ�ı���
        public bool IsWeaponActive { get; private set; }    //�����Ƿ�ɶ�
        public bool IsCooling { get; private set; }         //�Ƿ�����ȴ��
        public float CurrentCharge { get; private set; }    //��
        public Vector3 MuzzleWorldVelocity { get; private set; }    //ǹ����ʱ�����е�����

        public float GetAmmoNeededToShoot() =>
            (ShootType != WeaponShootType.Charge ?
            1f : Mathf.Max(1f, AmmoUsedOnStartCharge)) /
            (MaxAmmo * BulletsPerShot);//��õ��������Ҫ���ӵ���

        public int GetCarriedPhysicalBullets() => m_CarriedPhysicalBullets;
        public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);

        AudioSource m_ShootAudioSource;

        public bool IsReloading { get; private set; }

        const string k_AnimAttackParameter = "Attack";

        private Queue<Rigidbody> m_PhysicalAmmoPool;//ǹ���ӵ���

        void Awake()
        {
            m_CurrentAmmo = HasPhysicalBullets ? ClipSize : MaxAmmo;
            //m_CarriedPhysicalBullets = HasPhysicalBullets ? ClipSize : 0;//�е�ϻʱ������ϻ���ӵ���������Ϊ0
            m_LastMuzzlePosition = WeaponMuzzle.position;

            /* 
             * m_ShootAudioSource = GetComponent<AudioSource>();
             DebugUtility.HandleErrorIfNullGetComponent<AudioSource, WeaponController>(m_ShootAudioSource, this,
                 gameObject);
            */

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

            /*
            if (HasPhysicalBullets)//����������ӵ�
            {
                m_PhysicalAmmoPool = new Queue<Rigidbody>(ShellPoolSize);//�����ӵ���

                for (int i = 0; i < ShellPoolSize; i++)
                {
                    GameObject shell = Instantiate(ShellCasing, transform);
                    shell.SetActive(false);//Ĭ�Ͻ����ӵ�
                    m_PhysicalAmmoPool.Enqueue(shell.GetComponent<Rigidbody>());
                }
            }*/
        }

        //PickUp����Ҫɾ��
        //public void AddCarriablePhysicalBullets(int count) => m_CarriedPhysicalBullets = Mathf.Max(m_CarriedPhysicalBullets + count, MaxAmmo);

        void ShootShell()//����ǹ���
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


        public void Reload()   //�����ӵ�װ��
        {
            m_CurrentAmmo = ClipSize;
            m_CarriedPhysicalBullets = Mathf.RoundToInt(m_CurrentAmmo);
            IsReloading = false;
        }

        public void StartReloadAnimation()  //װ������
        {
            /*
                GetComponent<Animator>().SetTrigger("Reload");
                IsReloading = true;
            */
        }

        void Update()
        {
            UpdateAmmo();
            UpdateCharge();
            UpdateContinuousShootSound();

            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
                m_LastMuzzlePosition = WeaponMuzzle.position;
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
                        return TryReleaseCharge();
                    }

                    return false;

                default:
                    return false;
            }
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
            if (!IsCharging
                && m_CurrentAmmo >= AmmoUsedOnStartCharge
                && Mathf.FloorToInt((m_CurrentAmmo - AmmoUsedOnStartCharge) * BulletsPerShot) > 0
                && m_LastTimeShot + DelayBetweenShots < Time.time)
            {
                UseAmmo(AmmoUsedOnStartCharge);

                LastChargeTriggerTimestamp = Time.time;
                IsCharging = true;

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

                return true;
            }

            return false;
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
                /*
                ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this*/
                
                trigger(Quaternion.LookRotation(shotDirection));
            }

            // muzzle flash
            if (MuzzleFlashPrefab != null)
            {
                //�̻���Ч�Ĳ���
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

        public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = BulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }

        private void trigger(Quaternion fireRotation)
        {
            //Debug.Log(111);
            RaycastHit hit;

            //float currentSpread = Mathf.Lerp(0.0f, maxSpreadAngle, accuracy / timeTillMaxSpread);

            //fireRotation = Quaternion.RotateTowards(fireRotation, Random.rotation, Random.Range(0.0f, currentSpread));

            Physics.Raycast(transform.position, fireRotation * Vector3.forward, out hit, Mathf.Infinity);

            {
                GameObject tempBullet = Instantiate(bullet, WeaponMuzzle.position, fireRotation);
                tempBullet.GetComponent<BulletController>().hitPoint = hit.point;
            }
        }
    }
}