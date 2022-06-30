using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{
    //暂不加载游戏流管理
    //该input输入流(如Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal)类函数)与编辑器中的input设置相关 方便修改·调试
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        //镜头灵敏度
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        //WebGL额外镜头灵敏度
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        //输入限制
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        //反转X轴
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        //反转Y轴
        public bool InvertXAxis = false;

        GameFlowManager m_GameFlowManager;
        PlayerCharacterController m_PlayerCharacterController;
        bool m_FireInputWasHeld;

        public bool lockInput=false;

        void Start()
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerInputHandler>(
                m_PlayerCharacterController, this, gameObject);
            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, PlayerInputHandler>(m_GameFlowManager, this);

            //进入第一人称 隐藏鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //后更新-在update()全调用后再调用
        void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld();
        }

        //通过鼠标是否锁定判断是否能处理输入
        public bool CanProcessInput()
        {
            //return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.GameIsEnding;
            if (lockInput)
            {
                return false;
            }
            return Cursor.lockState == CursorLockMode.Locked;
        }

        //判断移动输入wasd
        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                //GetAxisRaw 根据input设置的key键返回1或者-1
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                //约束最大值为1
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        //镜头移动-x轴
        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal);
        }

        //镜头输入-y轴
        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical);
        }

        //跳跃按下输入
        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        //跳跃持续输入
        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        //开火按下输入
        public bool GetFireInputDown()
        {
            return GetFireInputHeld() && !m_FireInputWasHeld;
        }

        //开火弹起输入
        public bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && m_FireInputWasHeld;
        }

        //开火持续输入
        public bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                //bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) != 0f;
                //if (isGamepad)
                //{
                //    return Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) >= TriggerAxisThreshold;
                //}
                //else
                //{
                //    return Input.GetButton(GameConstants.k_ButtonNameFire);
                //}
                return Input.GetButton(GameConstants.k_ButtonNameFire);
            }

            return false;
        }

        //瞄准持续输入
        public bool GetAimInputHeld()
        {
            if (CanProcessInput())
            {
                //bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) != 0f;
                //bool i = isGamepad
                //    ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) > 0f)
                //    : Input.GetButton(GameConstants.k_ButtonNameAim);
                //return i;
                return Input.GetButton(GameConstants.k_ButtonNameAim);
            }

            return false;
        }

        //冲刺持续输入
        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameSprint);
            }

            return false;
        }

        //下蹲按下输入
        public bool GetCrouchInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        //下蹲弹起输入
        public bool GetCrouchInputReleased()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        //重新装填按下输入
        public bool GetReloadButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonReload);
            }

            return false;
        }

        //使用技能按下输入
        public bool GetUseSkillButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonUseSkill);
            }

            return false;
        }

        //使用特殊技能按下输入
        public bool GetUseSPSkillButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonUseSPSkill);
            }

            return false;
        }

        //进行交互按下输入
        public bool GetInteractButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonInteract);
            }

            return false;
        }

        //丢弃按下输入
        public bool GetDiscardButtonDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonDiscard);
            }

            return false;
        }

        //切换武器输入-滚轴
        public int GetSwitchWeaponInput()
        {
            if (CanProcessInput())
            {

                //bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadSwitchWeapon) != 0f;
                //string axisName = isGamepad
                //    ? GameConstants.k_ButtonNameGamepadSwitchWeapon
                //    : GameConstants.k_ButtonNameSwitchWeapon;

                string axisName = GameConstants.k_ButtonNameSwitchWeapon;

                if (Input.GetAxis(axisName) > 0f)
                    return -1;
                else if (Input.GetAxis(axisName) < 0f)
                    return 1;
                //else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) > 0f)
                //    return -1;
                //else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) < 0f)
                //    return 1;
            }

            return 0;
        }

        //切换武器输入-按键（1-9）
        public int GetSelectWeaponInput()
        {
            if (CanProcessInput())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    return 1;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    return 2;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    return 3;
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    return 4;
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                    return 5;
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                    return 6;
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                    return 7;
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                    return 8;
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                    return 9;
                else
                    return 0;
            }

            return 0;
        }

        //        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
        //        {
        //            if (CanProcessInput())
        //            {
        //                // Check if this look input is coming from the mouse
        //                bool isGamepad = Input.GetAxis(stickInputName) != 0f;
        //                float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

        //                // handle inverting vertical input
        //                if (InvertYAxis)
        //                    i *= -1f;

        //                // apply sensitivity multiplier
        //                i *= LookSensitivity;

        //                if (isGamepad)
        //                {
        //                    // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
        //                    i *= Time.deltaTime;
        //                }
        //                else
        //                {
        //                    // reduce mouse input amount to be equivalent to stick movement
        //                    i *= 0.01f;
        //#if UNITY_WEBGL
        //                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
        //                    i *= WebglLookSensitivityMultiplier;
        //#endif
        //                }

        //                return i;
        //            }

        //            return 0f;
        //        }

        //鼠标输入-镜头输入（包括x轴 y轴）
        float GetMouseLookAxis(string mouseInputName, string stickInputName)
        {
            if (CanProcessInput())
            {
                // Check if this look input is coming from the mouse
                //bool isGamepad = Input.GetAxis(stickInputName) != 0f;
                //float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);
                float i = Input.GetAxisRaw(mouseInputName);

                // handle inverting vertical input
                if (InvertYAxis)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;

                //                if (isGamepad)
                //                {
                //                    // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
                //                    i *= Time.deltaTime;
                //                }
                //                else
                //                {
                //                    // reduce mouse input amount to be equivalent to stick movement
                //                    i *= 0.01f;
                //#if UNITY_WEBGL
                //                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                //                    i *= WebglLookSensitivityMultiplier;
                //#endif
                //                }

                i *= 0.01f;
#if UNITY_WEBGL
                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                    i *= WebglLookSensitivityMultiplier;
#endif

                return i;
            }

            return 0f;
        }
    }
}