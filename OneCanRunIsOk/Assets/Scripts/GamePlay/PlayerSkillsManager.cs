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
        [Tooltip("初始携带技能")]
        public SkillController StartSkill;

        [Tooltip("技能释放位置")]
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
            //使用技能
            if (m_InputHandler.GetUseSkillButtonDown())
            {
                CurrentSkillInstance.UseSkill();
            }
        }

        //改变当前技能
        void ChangeCurrentSkill(SkillController skillPrefab)
        {
            //删除原技能实例
            if (CurrentSkillInstance != null)
            {
                Destroy(CurrentSkillInstance.gameObject);
            }
            CurrentSkillInstance = Instantiate(skillPrefab, SkillSocket);
            CurrentSkillInstance.transform.localPosition = Vector3.zero;
            CurrentSkillInstance.transform.localRotation = Quaternion.identity;

            CurrentSkillInstance.SourcePrefab = skillPrefab.gameObject;

            //若为Cast类型
            if(CurrentSkillInstance.m_SkillType== SkillType.Cast)
            {
                WeaponController m_SkillWeapon= CurrentSkillInstance.GetComponent<WeaponController>();
                m_SkillWeapon.Owner = gameObject;
            }
        }
    }
}
