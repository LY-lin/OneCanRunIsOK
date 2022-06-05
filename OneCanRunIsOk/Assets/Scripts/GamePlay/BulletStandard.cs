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
        [Tooltip("Radius of this projectile's collision detection")]
        public float Radius = 0.01f;

       // [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
       // public Transform Root;

       // [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
       // public Transform Tip;

        [Tooltip("LifeTime of the projectile")]
        public float MaxLifeTime = 5f;

        [Tooltip("VFX prefab to spawn upon impact")]
        public GameObject ImpactVfx;

        [Tooltip("LifeTime of the VFX before being destroyed")]
        public float ImpactVfxLifetime = 5f;

        [Tooltip("Offset along the hit normal where the VFX will be spawned")]
        public float ImpactVfxSpawnOffset = 0.1f;

        [Tooltip("Clip to play on impact")]
        public AudioClip ImpactSfxClip;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Header("Movement")]
        [Tooltip("Speed of the projectile")]
        public float Speed = 1f;

        [Tooltip("Downward acceleration from gravity")]
        public float GravityDownAcceleration = 0f;

        [Tooltip(
            "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
        public float TrajectoryCorrectionDistance = -1;

        [Tooltip("Determines if the projectile inherits the velocity that the weapon's muzzle had when firing")]
        public bool InheritWeaponVelocity = false;
        

        [Header("Damage")]
        [Tooltip("Damage of the projectile")]
        public float Damage = 40f;

        /*[Tooltip("Area of damage. Keep empty if you don<t want area damage")]
        public DamageArea AreaOfDamage;*/

        [Header("Debug")]
        [Tooltip("Color of the projectile radius debug view")]
        public Color RadiusColor = Color.cyan * 0.2f;

        BulletController m_ProjectileBase;
        Vector3 m_LastRootPosition;
        Vector3 m_Velocity;
        bool m_HasTrajectoryOverride;
        //float m_ShootTime;
        Vector3 m_TrajectoryCorrectionVector;
        Vector3 m_ConsumedTrajectoryCorrectionVector;
        List<Collider> m_IgnoredColliders;

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        public void restart()
        {
            Start();
            m_ProjectileBase.restart = false;
        }

        public void Start()
        {
            m_ProjectileBase = this.gameObject.GetComponent<BulletController>();
            gadia_test();
        }

        public void gadia_test()
        {
            m_LastRootPosition = m_ProjectileBase.transform.position;
            m_Velocity = transform.forward * Speed;
            transform.position += m_ProjectileBase.InheritedMuzzleVelocity * Time.deltaTime;
            

            // get weaponController
            WeaponController wp = m_ProjectileBase.weaponController;

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
                restart();


            }
            if( Time.time - this.m_ProjectileBase.m_ShootTime > this.MaxLifeTime){
                this.m_ProjectileBase.weaponController.bulletPoolManager.release(this.gameObject);
            }
            // Move
            m_LastRootPosition = transform.position;
            transform.position += m_Velocity * Time.deltaTime;

            /*
            if (InheritWeaponVelocity)
            {
                transform.position += m_ProjectileBase.InheritedMuzzleVelocity * Time.deltaTime;
            }*/

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
            transform.forward = m_Velocity.normalized;

            // Gravity
            if (GravityDownAcceleration > 0)
            {
                // add gravity to the projectile velocity for ballistic effect
                m_Velocity += Vector3.down * GravityDownAcceleration * Time.deltaTime;
            }
            

            //m_LastRootPosition = Root.position;
        }

        bool IsHitValid(RaycastHit hit)
        {
            // ignore hits with an ignore component
            /*if (hit.collider.GetComponent<IgnoreHitDetection>())
            {
                return false;
            }*/

            // ignore hits with triggers that don't have a Damageable component
            if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

 
        void OnDrawGizmosSelected()
        {
            Gizmos.color = RadiusColor;
            Gizmos.DrawSphere(transform.position, Radius);
        }

        void OnDestroy()
        {
            int a = 0;
        }
    }
}
