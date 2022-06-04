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
        [Tooltip("��ǰ����")]
        public SkillController CurrentSkill;


        PlayerInputHandler m_InputHandler;

        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerSkillsManager>(m_InputHandler, this,
                gameObject);
        }


        void Update()
        {
            //ʹ�ü���
            if (m_InputHandler.GetUseSkillButtonDown())
            {
                CurrentSkill.UseSkill();
            }
        }
    }
}
