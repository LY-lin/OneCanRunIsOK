using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
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

        [Header("Ͷ����������")]
        
        [Header("������������")]

        [Header("�ٻ���������")]

        //�ϴ�ʹ�ü���ʱ��
        float m_LastTimeUse = Mathf.NegativeInfinity;
        //����ɼ��ܵ�����������
        WeaponController m_SkillWeapon;
        //ԴԤ�Ƽ�
        public GameObject SourcePrefab { get; set; }

        void Awake()
        {
            m_SkillWeapon = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, SkillController>(m_SkillWeapon,
                this, gameObject);
        }

        //void Update()
        //{

        //}

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
            Debug.Log("Buff!");
        }

        //�ٻ�����ʵ��
        void UseSummonSkill()
        {
            Debug.Log("Summon!");
        }

    }
}