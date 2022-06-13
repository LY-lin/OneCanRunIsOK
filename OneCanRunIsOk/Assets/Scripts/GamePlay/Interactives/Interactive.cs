using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    //�ɽ������ֻ��Ϊ�ṹ�壬��Ҫ���ж��߼�λ��PlayCharacterController.HandleInteract()��
    public class Interactive : MonoBehaviour
    {
        [Tooltip("������������")]
        public string description;

        //�Ƿ�չʾUI
        public bool showInteractiveUI = false;
        //�Ƿ񽻻�
        public bool hasInteracted = false;
        //��������ҿ�����
        public PlayerCharacterController m_PlayerCharacterController;

    }
}
