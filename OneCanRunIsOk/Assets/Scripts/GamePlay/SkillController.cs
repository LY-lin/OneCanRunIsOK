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
        //Ͷ�乥��-������
        Cast,
        //����״̬-��ָ�HP����߹�������
        Buff,
        //�ٻ���-��̨�������ˡ��ٻ����
        Summon,
    }

    public class SkillController : MonoBehaviour
    {
        [Header("ͨ������")]
        [Tooltip("��������")]
        public string SkillName;

        [Tooltip("��������")]
        public string SkillDescription;

        [Tooltip("����ͼ��")]
        public Sprite SkillIcon;

        [Tooltip("��������-Ͷ��/����/�ٻ�")]
        public SkillType m_SkillType;

        [Tooltip("������ȴʱ��")]
        public float CoolingTime = 10f;

        /*
        [Header("Ͷ����������")]

        [Header("������������")]

        [Header("�ٻ���������")]
        */
        //�ϴ�ʹ�ü���ʱ��
        public float m_LastTimeUse { get; private set; }
        //����ɼ��ܵ�����������
        WeaponController m_SkillWeapon;
        //�����ߵ�Actor����Ž�ɫ������ֵ
        Actor m_Actor;
        //ԴԤ�Ƽ�
        public GameObject SourcePrefab { get; set; }
        //������
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
        }

        //void Update()
        //{

        //}
        public void setWeaponOwner(GameObject gameObject)
        {
            m_SkillWeapon.Owner = gameObject;
        }

        //ʹ�øü���
        public bool UseSkill()
        {
            //������ȴ��
            if (m_LastTimeUse + CoolingTime > Time.time)
            {
                Debug.Log("Cooling!");
                return false;
            }

            m_LastTimeUse = Time.time;

            //�жϼ�������
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
            
            return true;
        }
        
        //Ͷ�似��ʵ��
        void UseCastSkill()
        {
            m_SkillWeapon.HandleShootInputs(true, false, false);
            Debug.Log("Cast!");
        }

        //���漼��ʵ��
        void UseBuffSkill()
        {
            SkillBuffGiver m_SkillBuffGiver = GetComponent<SkillBuffGiver>();
            DebugUtility.HandleErrorIfNullGetComponent<SkillBuffGiver, SkillController>(m_SkillBuffGiver,
                this, gameObject);
            m_SkillBuffGiver.buffGive();
        
        //Debug.Log("Buff!");

        }

        //�ٻ�����ʵ��
        void UseSummonSkill()
        {
            Debug.Log("Summon!");
        }
        
        //���¸ü��ܵĳ�����
        public void UpdateOwner()
        {
            m_Actor = Owner.GetComponent<Actor>();
            //DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerSkillsManager>(m_Actor,
            //    Owner, Owner);
        }

    }
}
