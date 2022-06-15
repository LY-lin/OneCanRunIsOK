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
        [Tooltip("the initial skill")]
        public SkillController StartSkill;

        [Tooltip("the initial Special skill")]
        public SkillController StartSpSkill;

        [Tooltip("where the skill use")]
        public Transform SkillSocket;

        PlayerInputHandler m_InputHandler;
        public SkillController CurrentSkillInstance { get; private set; }
        public SkillController CurrentSpSkillInstance { get; private set; }

        //if is aiming the special skill
        bool isAiming = false;

        GameObject activeAimingVfx;

        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerSkillsManager>(m_InputHandler, this,
                gameObject);

            ChangeCurrentSkill(StartSkill);
            ChangeCurrentSpSkill(StartSpSkill);
        }


        void Update()
        {
            //aiming
            if (!isAiming && m_InputHandler.GetUseSPSkillButtonDown())
            {
                isAiming = true;
            }
            else if(isAiming)
            {
                //aiming ray
                if (Physics.Raycast(SkillSocket.position, SkillSocket.forward, out RaycastHit hit,
                CurrentSpSkillInstance.UsingRange, -1))
                {
                    if (activeAimingVfx)
                    {
                        activeAimingVfx.transform.position = hit.point;
                    }
                    else
                    {
                        activeAimingVfx = Instantiate(CurrentSpSkillInstance.AimingVfx, SkillSocket);
                        activeAimingVfx.transform.position = hit.point;
                        activeAimingVfx.transform.rotation = Quaternion.identity;
                    }
                }
                if (m_InputHandler.GetUseSPSkillButtonDown())
                {
                    Transform aimTransform = activeAimingVfx.transform;
                    CurrentSpSkillInstance.UseSpSkill(aimTransform);
                    isAiming = false;
                    Destroy(activeAimingVfx);
                }
            }
            //get input for skill using
            else if (m_InputHandler.GetUseSkillButtonDown())
            {
                CurrentSkillInstance.UseSkill();
            }
        }

        //change the instance of skill
        void ChangeCurrentSkill(SkillController skillPrefab)
        {
            //destroy the old one before updating
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

            //if cast, set the owner
            if(CurrentSkillInstance.m_SkillType== SkillType.Cast)
            {
                WeaponController m_SkillWeapon= CurrentSkillInstance.GetComponent<WeaponController>();
                m_SkillWeapon.Owner = gameObject;
            }
        }

        void ChangeCurrentSpSkill(SkillController skillPrefab)
        {
            //destroy the old one before updating
            if (CurrentSpSkillInstance != null)
            {
                Destroy(CurrentSpSkillInstance.gameObject);
            }
            CurrentSpSkillInstance = Instantiate(skillPrefab, SkillSocket);
            CurrentSpSkillInstance.transform.localPosition = Vector3.zero;
            CurrentSpSkillInstance.transform.localRotation = Quaternion.identity;

            CurrentSpSkillInstance.SourcePrefab = skillPrefab.gameObject;
            CurrentSpSkillInstance.Owner = gameObject;
            CurrentSpSkillInstance.UpdateOwner();

           }
        
    }
}
