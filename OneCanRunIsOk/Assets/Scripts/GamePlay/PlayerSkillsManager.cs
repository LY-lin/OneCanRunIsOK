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
        // B
        public SkillController StartSkill_B;

        [Tooltip("the initial Special skill")]
        public SkillController StartSpSkill_B;

        // H
        public SkillController StartSkill_H;
        public SkillController StartSpSkill_H;


        // A
        public SkillController StartSkill_A;
        public SkillController StartSpSkill_A;


        [Tooltip("where the skill use")]
        public Transform SkillSocket;

        PlayerInputHandler m_InputHandler;
        public SkillController CurrentSkillInstance { get; private set; }
        public SkillController CurrentSpSkillInstance { get; private set; }

        //if is aiming the special skill
        bool isAiming = false;

        GameObject activeAimingVfx;

        //test Laser
        //public GameObject Laser;
        //LaserController lc;

        //test Flame
        //public SpitFlame spitFlame;

        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerSkillsManager>(m_InputHandler, this,
                gameObject);

            switch (this.gameObject.GetComponent<Actor>().campType){
                case ActorConfig.CampType.Azeyma:
                    ChangeCurrentSkill(StartSkill_A);
                    ChangeCurrentSpSkill(StartSpSkill_A);
                    break;
                case ActorConfig.CampType.Byregot:
                    ChangeCurrentSkill(StartSkill_B);
                    ChangeCurrentSpSkill(StartSpSkill_B);
                    break;
                case ActorConfig.CampType.Halone:
                    ChangeCurrentSkill(StartSkill_H);
                    ChangeCurrentSpSkill(StartSpSkill_H);
                    break;
                default:
                    break;

            }

            //test Laser
            //lc = Laser.GetComponent<LaserController>();
            //lc.Owner = gameObject;
            //lc.LaserSocket = SkillSocket;

            //test Flame
            //spitFlame = GetComponent<SpitFlame>();
        }


        void Update()
        {
            //test Laser
            //if (m_InputHandler.GetUseSPSkillButtonDown())
            //{
            //    lc.StartLaser();  
            //}
            //else if (m_InputHandler.GetUseSkillButtonDown())
            //{
            //    lc.StopLaser();
            //}

            //test Flame
            //if(m_InputHandler.GetUseSPSkillButtonDown())
            //{
            //    if (!isAiming)
            //    {
            //        spitFlame.StartSpitting();
            //        isAiming = true;
            //    }
            //    else
            //    {
            //        spitFlame.StopSpitting();
            //        isAiming = false;
            //    }
            //}

            //aiming
            if (!isAiming && m_InputHandler.GetUseSPSkillButtonDown() && !CurrentSpSkillInstance.isCooling())
            {
                isAiming = true;
            }
            else if (isAiming)
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
                        activeAimingVfx = Instantiate(CurrentSpSkillInstance.AimingVfx, hit.point, Quaternion.identity);
                        //activeAimingVfx.transform.position = hit.point;
                        //activeAimingVfx.transform.rotation = Quaternion.identity;
                    }
                }
                if (m_InputHandler.GetUseSPSkillButtonDown())
                {
                    //Transform aimingPoint = activeAimingVfx.transform.position;
                    CurrentSpSkillInstance.UseSpSkill(activeAimingVfx.transform.position);
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

        public SkillController GetCurrentSkill()
        {
            return CurrentSkillInstance;
        }

        public SkillController GetCurrentSpSkill()
        {
            return CurrentSpSkillInstance;
        }

    }
}
