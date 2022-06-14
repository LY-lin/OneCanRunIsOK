using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.GamePlay
{
    //可交互组件只作为结构体，主要的判断逻辑位于PlayCharacterController.HandleInteract()中
    public class Interactive : MonoBehaviour
    {
        [Tooltip("交互内容描述")]
        public string description;

        //是否展示UI
        public bool showInteractiveUI = false;
        //是否交互
        public bool hasInteracted = false;
        //交互的玩家控制器
        public PlayerCharacterController m_PlayerCharacterController;

        public UnityAction beInteracted;
        
    }
}
