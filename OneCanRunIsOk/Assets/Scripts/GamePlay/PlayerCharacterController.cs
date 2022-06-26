using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game;

namespace OneCanRun.GamePlay
{   
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
    //[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the main camera used for the player")]
        //玩家摄像机
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        //音源组件
        public AudioSource AudioSource;

        [Header("General")]
        [Tooltip("Force applied downward when in the air")]
        //重力数值
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        //物理层
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        //测试着地距离
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")]
        [Tooltip("Max movement speed when grounded (when not sprinting)")]
        //着地最大移动速度
        public float MaxSpeedOnGround = 10f;

        [Tooltip(
            "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        //类似于加速度，插值时使用的锐度因数
        public float MovementSharpnessOnGround = 15;

        [Tooltip("Max movement speed when crouching")]
        [Range(0, 1)]
        //下蹲速度因数
        public float MaxSpeedCrouchedRatio = 0.5f;

        [Tooltip("Max movement speed when not grounded")]
        //空中最大移动速度
        public float MaxSpeedInAir = 10f;

        [Tooltip("Acceleration speed when in the air")]
        //空中加速度
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        //冲刺速度因数
        public float SprintSpeedModifier = 2f;

        [Tooltip("Height at which the player dies instantly when falling off the map")]
        //死亡判定高度
        public float KillHeight = -50f;

        [Tooltip("Range of Interact")]
        //可交互的范围
        public float InteractiveRange = 10f;

        [Header("Rotation")]
        [Tooltip("Rotation speed for moving the camera")]
        //摄像头旋转速度 灵敏度
        public float RotationSpeed = 200f;

        [Range(0.1f, 1f)]
        [Tooltip("Rotation speed multiplier when aiming")]
        //瞄准时摄像头旋转速度
        public float AimingRotationMultiplier = 0.4f;

        [Header("Jump")]
        [Tooltip("Force applied upward when jumping")]
        //跳跃力
        public float JumpForce = 9f;

        [Header("Stance")]
        [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        //摄像头高度比 相对角色整体
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Height of character when standing")]
        //角色胶囊体高度
        public float CapsuleHeightStanding = 1.8f;

        [Tooltip("Height of character when crouching")]
        //角色
        public float CapsuleHeightCrouching = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        //下蹲加速度
        public float CrouchingSharpness = 10f;

        [Header("Audio")]
        [Tooltip("Amount of footstep sounds played when moving one meter")]
        //脚步频率
        public float FootstepSfxFrequency = 0.25f;

        [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        //冲刺脚步频率
        public float FootstepSfxFrequencyWhileSprinting = 0.25f;

        [Tooltip("Sound played for footsteps")]
        //脚步声效
        public AudioClip FootstepSfx;

        //跳跃声效
        [Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
        //着地声效
        [Tooltip("Sound played when landing")] public AudioClip LandSfx;

        [Tooltip("Sound played when taking damage froma fall")]
        public AudioClip FallDamageSfx;

        [Header("Fall Damage")]
        [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
        //是否开启跌落伤害
        public bool RecievesFallDamage;

        [Tooltip("Minimun fall speed for recieving fall damage")]
        //收到跌落伤害最小速度
        public float MinSpeedForFallDamage = 10f;

        [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
        //收到满额跌落伤害的速度
        public float MaxSpeedForFallDamage = 30f;

        [Tooltip("Damage recieved when falling at the mimimum speed")]
        //最小跌落伤害
        public float FallDamageAtMinSpeed = 10f;

        [Tooltip("Damage recieved when falling at the maximum speed")]
        //最大跌落伤害
        public float FallDamageAtMaxSpeed = 50f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsCrouching { get; private set; }

        //上次看向的可交互物体
        Interactive lastInteractive;

        public float RotationMultiplier
        {
            get
            {
                //if (m_WeaponsManager.IsAiming)
                //{
                //    return AimingRotationMultiplier;
                //}

                return 1f;
            }
        }

        Health m_Health;
        PlayerInputHandler m_InputHandler;
        CharacterController m_Controller;
        PlayerWeaponsManager m_WeaponsManager;
        Actor m_Actor;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        Vector3 m_LatestImpactSpeed;
        float m_LastTimeJumped = 0f;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        void Awake()
        {
            ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
            if (actorsManager != null)
                actorsManager.SetPlayer(gameObject);
        }

        void Start()
        {
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, PlayerCharacterController>(m_Controller,
                this, gameObject);

            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerCharacterController>(m_InputHandler,
                this, gameObject);

            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, PlayerCharacterController>(
                m_WeaponsManager, this, gameObject);

            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerCharacterController>(m_Health, this, gameObject);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerCharacterController>(m_Actor, this, gameObject);

            m_Controller.enableOverlapRecovery = true;

            m_Health.OnDie += OnDie;

            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);

            //allow flash
            Physics.autoSyncTransforms = true;
        }

        void Update()
        {
            // check for Y kill
            if (!IsDead && transform.position.y < KillHeight)
            {
                m_Health.Kill();
            }

            HasJumpedThisFrame = false;

            //着地判定
            bool wasGrounded = IsGrounded;
            GroundCheck();

            // landing
            //该帧着陆，判定着陆与伤害
            if (IsGrounded && !wasGrounded)
            {
                // Fall damage
                float fallSpeed = -Mathf.Min(CharacterVelocity.y, m_LatestImpactSpeed.y);
                float fallSpeedRatio = (fallSpeed - MinSpeedForFallDamage) /
                                       (MaxSpeedForFallDamage - MinSpeedForFallDamage);
                if (RecievesFallDamage && fallSpeedRatio > 0f)
                {
                    float dmgFromFall = Mathf.Lerp(FallDamageAtMinSpeed, FallDamageAtMaxSpeed, fallSpeedRatio);
                    m_Health.TakeDamage(dmgFromFall, null); 

                    // fall damage SFX
                    AudioSource.PlayOneShot(FallDamageSfx);
                }
                else
                {
                    // land SFX
                    AudioSource.PlayOneShot(LandSfx);
                }
            }

            // crouching
            //下蹲
            if (m_InputHandler.GetCrouchInputDown())
            {
                SetCrouchingState(!IsCrouching, false);
            }

            UpdateCharacterHeight(false);

            //处理角色移动
            HandleCharacterMovement();
            //处理交互
            HandleInteract();
        }

        //死亡
        void OnDie()
        {
            IsDead = true;

            // Tell the weapons manager to switch to a non-existing weapon in order to lower the weapon
            m_WeaponsManager.SwitchToWeaponIndex(-1, true);

            EventManager.broadcast(Events.PlayerDeathEvent);
        }

        //着地判定
        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            //确保已在空中的地面检查距离非常小，以防止突然弹到地面
            float chosenGroundCheckDistance =
                IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            m_GroundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            //只有在距离上次跳跃一段时间的情况下，才尝试探测地面；否则，我们可能会在尝试跳跃后立即跳到地上
            if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                //胶囊体判定
                //如果我们着地，收集有关地面法线的信息，使用向下的胶囊投射代表我们的角色胶囊
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                    m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_GroundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_GroundNormal))
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_Controller.skinWidth)
                        {
                            m_Controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        void HandleCharacterMovement()
        {
            MaxSpeedOnGround = GetComponent<Actor>().GetActorProperties().getMaxSpeed();
            // horizontal character rotation
            // 修改角色Rotate.y以修改左右朝向
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier),
                        0f), Space.Self);
            }

            // vertical camera rotation
            //修改角色摄像机Rotate.x以修改上下朝向
            {
                // add vertical inputs to the camera's vertical angle
                m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

                // limit the camera's vertical angle to min/max
                //限制90度
                m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                //将垂直角度作为局部旋转沿其右轴应用于摄影机变换（使其上下旋转）
                PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }

            // character movement handling
            //冲刺判定
            bool isSprinting = m_InputHandler.GetSprintInputHeld();
            {
                if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }

                float speedModifier = isSprinting ? SprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

                // handle grounded movement
                if (IsGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier;
                    // reduce speed if crouching by crouch speed ratio
                    if (IsCrouching)
                        targetVelocity *= MaxSpeedCrouchedRatio;
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                                     targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    //插值计算当前角色速度，保证平滑的角色速度变化
                    CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                        MovementSharpnessOnGround * Time.deltaTime);

                    // jumping
                    if (IsGrounded && m_InputHandler.GetJumpInputDown())
                    {
                        // force the crouch state to false
                        //取消下蹲
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            //play sound
                            AudioSource.PlayOneShot(JumpSfx);

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            m_LastTimeJumped = Time.time;
                            HasJumpedThisFrame = true;

                            // Force grounding to false
                            IsGrounded = false;
                            m_GroundNormal = Vector3.up;
                        }
                    }

                    //footsteps sound
                    float chosenFootstepSfxFrequency =
                        (isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency);
                    if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
                    {
                        m_FootstepDistanceCounter = 0f;
                        AudioSource.PlayOneShot(FootstepSfx);
                    }

                    // keep track of distance traveled for footsteps sound
                    m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
                }
            }

            // apply the final calculated velocity value as a character movement
            //应用胶囊体移动
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
            m_Controller.Move(CharacterVelocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            //检测障碍物以相应地调整速度
            m_LatestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                //记录撞击速度
                m_LatestImpactSpeed = CharacterVelocity;

                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }
        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        //如果给定法线表示的倾斜角度低于角色控制器的倾斜角度限制，则返回true
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        //获取角色控制器胶囊的下半球的中心点
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * m_Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        //获取角色控制器胶囊的上半球的中心点
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - m_Controller.radius));
        }

        // Gets a reoriented direction that is tangent to a given slope
        //获取与给定坡度相切的重定向方向
        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                m_Controller.height = m_TargetCharacterHeight;
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
            // Update smooth height
            else if (m_Controller.height != m_TargetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                    CrouchingSharpness * Time.deltaTime);
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                    Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
        }

        // returns false if there was an obstruction
        bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                m_TargetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    Collider[] standingOverlaps = Physics.OverlapCapsule(
                        GetCapsuleBottomHemisphere(),
                        GetCapsuleTopHemisphere(CapsuleHeightStanding),
                        m_Controller.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);
                    foreach (Collider c in standingOverlaps)
                    {
                        if (c != m_Controller)
                        {
                            return false;
                        }
                    }
                }

                m_TargetCharacterHeight = CapsuleHeightStanding;
            }
            

            if (OnStanceChanged != null)
            {
                OnStanceChanged.Invoke(crouched);
            }

            IsCrouching = crouched;
            return true;
        }

        void HandleInteract()
        {
            if (lastInteractive)
            {
                lastInteractive.showInteractiveUI = false;
            }
            //检测看向
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit,
                1000, -1))
            {
                //Debug.Log(hit.point);
                lastInteractive = hit.collider.GetComponentInParent<Interactive>();
                if (lastInteractive != null)
                {
                    // Debug.Log("Watching!");
                    //判断距离
                    if (Vector3.Distance(PlayerCamera.transform.position, hit.transform.position) <= InteractiveRange)
                    {
                        //Debug.Log(lastInteractive.description);
                        lastInteractive.showInteractiveUI = true;
                        if (m_InputHandler.GetInteractButtonDown())
                        {
                            lastInteractive.showInteractiveUI = false;
                            lastInteractive.hasInteracted = true;
                            
                            lastInteractive.m_PlayerCharacterController = this;
                            lastInteractive.beInteracted.Invoke();
                        }
                    }

                }
            }
        }
    }
}
