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
        [Tooltip("Ͷ����ģ��")]
        public GameObject Projectile;

        [Tooltip("Ͷ���")]
        public Transform CastMuzzle;

        [Header("������������")]

        [Header("�ٻ���������")]

        //�ϴ�ʹ�ü���ʱ��
        float m_LastTimeUse = Mathf.NegativeInfinity;

        //void Awake()
        //{

        //}

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
                    doCastSkill();
                    break;

                case SkillType.Buff:
                    doBuffSkill();
                    break;

                case SkillType.Summon:
                    doSummonSkill();
                    break;

                default:
                    break;
            }
            
            return true;
        }
        
        //Ͷ�似��ʵ��
        void doCastSkill()
        {
            Debug.Log("Cast!");
        }

        //���漼��ʵ��
        void doBuffSkill()
        {
            Debug.Log("Buff!");
        }

        //�ٻ�����ʵ��
        void doSummonSkill()
        {
            Debug.Log("Summon!");
        }
    }
}
