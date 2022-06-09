using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine.Events;

namespace OneCanRun.GamePlay
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerSkillsManager : MonoBehaviour
    {
        [Tooltip("��ʼЯ������")]
        public SkillController StartSkill;

        [Tooltip("�����ͷ�λ��")]
        public Transform SkillSocket;

        PlayerInputHandler m_InputHandler;
        SkillController CurrentSkillInstance;

        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerSkillsManager>(m_InputHandler, this,
                gameObject);

            ChangeCurrentSkill(StartSkill);
        }


        void Update()
        {
            //ʹ�ü���
            if (m_InputHandler.GetUseSkillButtonDown())
            {
                CurrentSkillInstance.UseSkill();
            }
        }

        //�ı䵱ǰ����
        void ChangeCurrentSkill(SkillController skillPrefab)
        {
            //ɾ��ԭ����ʵ��
            if (CurrentSkillInstance != null)
            {
                Destroy(CurrentSkillInstance.gameObject);
            }
            CurrentSkillInstance = Instantiate(skillPrefab, SkillSocket);
            CurrentSkillInstance.transform.localPosition = Vector3.zero;
            CurrentSkillInstance.transform.localRotation = Quaternion.identity;

            CurrentSkillInstance.SourcePrefab = skillPrefab.gameObject;
            CurrentSkillInstance.Owner = gameObject;
            CurrentSkillInstance.UpdateOwner();

            //��ΪCast����
            if(CurrentSkillInstance.m_SkillType== SkillType.Cast)
            {
                WeaponController m_SkillWeapon= CurrentSkillInstance.GetComponent<WeaponController>();
                m_SkillWeapon.Owner = gameObject;
            }
        }
    }
}
