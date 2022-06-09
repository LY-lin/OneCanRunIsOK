using UnityEngine;
using System;
using OneCanRun.Game.Share;
using OneCanRun.Game;
using System.Collections.Generic;

namespace OneCanRun.GamePlay
{
    public class BulletStandard : MonoBehaviour
    {
        [Header("General")]

        [Tooltip("LifeTime of the projectile")]
        public float MaxLifeTime = 5f;

        [Header("Movement")]

        [Tooltip("Downward acceleration from gravity")]
        public float GravityDownAcceleration = 0f;

        [Tooltip(
            "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
        public float TrajectoryCorrectionDistance = 5;

        [Tooltip("Determines if the projectile inherits the velocity that the weapon's muzzle had when firing")]
        public bool InheritWeaponVelocity = false;


        [Header("Damage")]
        [Tooltip("Damage of the projectile")]
        public float Damage = 40f;


        BulletController m_ProjectileBase;
        Vector3 m_LastRootPosition;
        Vector3 m_Velocity;
        bool m_HasTrajectoryOverride;
        Vector3 m_TrajectoryCorrectionVector;
        Vector3 m_ConsumedTrajectoryCorrectionVector;


        private void Start()
        {
            
            init();
        }
        void OnEnable()
        {
           
        }

        public void init()
        {
            m_ProjectileBase = this.gameObject.GetComponent<BulletController>();
            m_LastRootPosition = m_ProjectileBase.transform.position;
            m_Velocity = transform.forward * m_ProjectileBase.speed;
            transform.position += m_ProjectileBase.InheritedMuzzleVelocity * Time.deltaTime;
            

            // get weaponController
            WeaponController wp = m_ProjectileBase.WeaponController;

            // Handle case of player shooting (make projectiles not go through walls, and remember center-of-screen trajectory)
            PlayerWeaponsManager playerWeaponsManager = wp.Owner.GetComponent<PlayerWeaponsManager>();

            if (playerWeaponsManager)
            {
                m_HasTrajectoryOverride = true;

                Vector3 cameraToMuzzle = (m_ProjectileBase.InitialPosition -
                                          playerWeaponsManager.WeaponCamera.transform.position);

                m_TrajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                    playerWeaponsManager.WeaponCamera.transform.forward);
                m_ConsumedTrajectoryCorrectionVector = new Vector3(0,0,0);
            }
        }

        void Update()
        {

            if (m_ProjectileBase.restart){
                this.Start();
                m_ProjectileBase.restart = false;

            }
            if(Time.time - m_ProjectileBase.m_ShootTime >= MaxLifeTime){
                m_ProjectileBase.WeaponController.bulletPoolManager.release(this.gameObject);
            }
            // Move
            m_LastRootPosition = transform.position;
            transform.position += m_Velocity * Time.deltaTime;
            

            // Drift towards trajectory override (this is so that projectiles can be centered 
            // with the camera center even though the actual weapon is offset)
            if (m_HasTrajectoryOverride && m_ConsumedTrajectoryCorrectionVector.sqrMagnitude <
                m_TrajectoryCorrectionVector.sqrMagnitude)
            {
                Vector3 correctionLeft = m_TrajectoryCorrectionVector - m_ConsumedTrajectoryCorrectionVector;
                float distanceThisFrame = (transform.position - m_LastRootPosition).magnitude;
                Vector3 correctionThisFrame =
                    (distanceThisFrame / TrajectoryCorrectionDistance) * m_TrajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                m_ConsumedTrajectoryCorrectionVector += correctionThisFrame;

                // Detect end of correction
                if (m_ConsumedTrajectoryCorrectionVector.sqrMagnitude == m_TrajectoryCorrectionVector.sqrMagnitude)
                {
                    m_HasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }

            // Orient towards velocity
            //transform.forward = m_Velocity.normalized;

            
        }

      

       
    }
}
