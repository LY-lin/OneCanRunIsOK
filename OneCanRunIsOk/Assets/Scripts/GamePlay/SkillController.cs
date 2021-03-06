using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    //��������
    public enum SkillType
    {
        //cast skill which be like bullet
        Cast,
        //give a buff
        Buff,
        //create some objects
        Summon,
        //special skill, such as flying, creating a storm
        Special,
    }

    public enum SpSkillType
    {
        //there are 3 special skills
        Tonado,
        FireRain,
        Flash,
    }

    public class SkillController : MonoBehaviour
    {
        [Header("Skill information")]
        //[Tooltip("��������")]
        public string SkillName;

        //[Tooltip("��������")]
        public string SkillDescription;

        //[Tooltip("����ͼ��")]
        public Sprite SkillIcon;

        //[Tooltip("��������-Ͷ��/����/�ٻ�")]
        public SkillType m_SkillType;
        public SpSkillType m_SpSkillType;

        //[Tooltip("������ȴʱ��")]
        public float CoolingTime = 10f;

        public float UsingRange = 10f;

        //exist time of the tonado
        public float ExistingTime = 10f;

        [Header("Skill VFX")]
        public GameObject AimingVfx;
        public GameObject UsingVfx;

        [Header("Skill SFX")]
        public AudioClip UsingSfx;
        AudioSource m_ShootAudioSource;

        //last time of using skill
        public float m_LastTimeUse { get; private set; }
        //cast use
        WeaponController m_SkillWeapon;
        //get the properity
        Actor m_Actor;
        //prefab
        public GameObject SourcePrefab { get; set; }
        //owner
        public GameObject Owner { get; set; }

        void Awake()
        {
            
            if (m_SkillType == SkillType.Cast)
            {
                m_SkillWeapon = GetComponent<WeaponController>();
                DebugUtility.HandleErrorIfNullGetComponent<WeaponController, SkillController>(m_SkillWeapon,
                    this, gameObject);
            }
            m_LastTimeUse = Mathf.NegativeInfinity;
            m_ShootAudioSource = GetComponent<AudioSource>();
        }

        //void Update()
        //{

        //}
        public void setWeaponOwner(GameObject gameObject)
        {
            m_SkillWeapon.Owner = gameObject;
        }

        public bool isCooling()
        {
            if (m_LastTimeUse + CoolingTime > Time.time)
            {
                return true;
            }
            return false;
        }

        //interface
        public bool UseSkill()
        {
            //CD?
            if (isCooling())
            {
                return false;
            }

            m_LastTimeUse = Time.time;

            //switch type
            switch (m_SkillType)
            {
                case SkillType.Cast:
                    UseCastSkill();
                    break;

                case SkillType.Buff:
                    UseBuffSkill();
                    break;

                case SkillType.Summon:
                    UseSummonSkill();
                    break;

                default:
                    break;
            }
            if(UsingSfx)
            m_ShootAudioSource.PlayOneShot(UsingSfx);
            return true;
        }
        
        //cast
        void UseCastSkill()
        {
            //m_SkillWeapon.damageType = Game.Share.DamageType.magic;
            m_SkillWeapon.HandleShootInputs(true, false, false);
            //Debug.Log("Cast!");
        }

        //buff
        void UseBuffSkill()
        {
            SkillBuffGiver m_SkillBuffGiver = GetComponent<SkillBuffGiver>();
            DebugUtility.HandleErrorIfNullGetComponent<SkillBuffGiver, SkillController>(m_SkillBuffGiver,
                this, gameObject);
            m_SkillBuffGiver.buffGive();

            GameObject VfxInstance = Instantiate(UsingVfx,Owner.transform);

            Destroy(VfxInstance.gameObject, 2f);
            //Debug.Log("Buff!");

        }

        //summon
        void UseSummonSkill()
        {
            GameObject SummonInstance = Instantiate(UsingVfx, transform.position + transform.up, Quaternion.identity);
            WeaponController wc = SummonInstance.GetComponentInChildren<WeaponController>();
            //SummonInstance.Owner = this.Owner;
            wc.Owner = this.Owner;
            Destroy(wc.bulletPoolManager.getCollector(), ExistingTime + 3);
            Destroy(SummonInstance, ExistingTime);
            //Debug.Log("Summon!");
        }

        //Special - get the using point
        public bool UseSpSkill(Vector3 aimingPoint)
        {

            m_LastTimeUse = Time.time;

            //switch type
            switch (m_SpSkillType)
            {
                case SpSkillType.Tonado:
                    Tonado(aimingPoint);
                    break;

                case SpSkillType.FireRain:
                    FireRain(aimingPoint);
                    break;

                case SpSkillType.Flash:
                    Flash(aimingPoint);
                    break;

                default:
                    break;
            }

            return true;
        }

        //
        void Tonado(Vector3 aimingPoint)
        {
            GameObject VfxInstance = Instantiate(UsingVfx, aimingPoint, Quaternion.identity);
            TonadoIce Tonado = VfxInstance.GetComponent<TonadoIce>();
            Tonado.Owner = this.Owner;
            Destroy(VfxInstance.gameObject, ExistingTime);
        }

        void FireRain(Vector3 aimingPoint)
        {
            GameObject VfxInstance = Instantiate(UsingVfx, aimingPoint + Vector3.up * 15, Quaternion.AngleAxis(-90f,Vector3.right));
            WeaponController FireSource = VfxInstance.GetComponent<WeaponController>();
            FireSource.Owner = this.Owner;
            //销毁子弹池与Vfx实例
            Destroy(FireSource.bulletPoolManager.getCollector(), ExistingTime + 3);
            Destroy(VfxInstance.gameObject, ExistingTime);
        }

        void Flash(Vector3 aimingPoint)
        {
            //PlayerCharacterController m_playerCharacterController = Owner.GetComponent<PlayerCharacterController>();
            //m_playerCharacterController.transform.position = aimingPoint;
            //Physics.autoSyncTransforms = true;
            GameObject VfxInstance = Instantiate(UsingVfx, Owner.transform.position, Quaternion.AngleAxis(-90f, Vector3.right));
            Destroy(VfxInstance.gameObject, ExistingTime);
            Owner.transform.position = aimingPoint;
            //Debug.Log(Owner.transform.position);
        }

        //get the property
        public void UpdateOwner()
        {
            m_Actor = Owner.GetComponent<Actor>();
            //DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerSkillsManager>(m_Actor,
            //    Owner, Owner);
        }

    }
}
