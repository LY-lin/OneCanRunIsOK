using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        [Tooltip("上下抖动频率")]
        public float VerticalBobFrequency = 1f;

        [Tooltip("上下抖动距离")]
        public float BobbingAmount = 1f;

        [Tooltip("旋转速度")] public float RotatingSpeed = 360f;

        [Tooltip("拾取音效")] public AudioClip PickupSfx;
        [Tooltip("拾取特效")] public GameObject PickupVfxPrefab;

        //刚体
        public Rigidbody PickupRigidbody { get; private set; }
        //碰撞体
        Collider m_Collider;
        //初始位置
        Vector3 m_StartPosition;
        //是否已播放拾取反馈
        bool m_HasPlayedFeedback;

        protected virtual void Start()
        {
            PickupRigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, Pickup>(PickupRigidbody, this, gameObject);
            m_Collider = GetComponent<Collider>();
            DebugUtility.HandleErrorIfNullGetComponent<Collider, Pickup>(m_Collider, this, gameObject);

            // ensure the physics setup is a kinematic rigidbody trigger
            //是否运动
            PickupRigidbody.isKinematic = true;
            //触发设置
            m_Collider.isTrigger = true;

            // Remember start position for animation
            //记录初始位置
            m_StartPosition = transform.position;
        }

        protected virtual void Update()
        {
            // Handle bobbing
            //上下抖动
            float bobbingAnimationPhase = ((Mathf.Sin(Time.time * VerticalBobFrequency) * 0.5f) + 0.5f) * BobbingAmount;
            transform.position = m_StartPosition + Vector3.up * bobbingAnimationPhase;

            // Handle rotating
            //旋转
            transform.Rotate(Vector3.up, RotatingSpeed * Time.deltaTime, Space.Self);
        }

        //碰撞触发
        void OnTriggerEnter(Collider other)
        {
            PlayerCharacterController pickingPlayer = other.GetComponent<PlayerCharacterController>();

            if (pickingPlayer != null)
            {
                OnPicked(pickingPlayer);

                PickupEvent evt = Events.PickupEvent;
                evt.Pickup = gameObject;
                EventManager.broadcast(evt);
            }
        }

        //具体拾取函数
        protected virtual void OnPicked(PlayerCharacterController playerController)
        {
            PlayPickupFeedback();
        }

        public void PlayPickupFeedback()
        {
            if (m_HasPlayedFeedback)
                return;

            if (PickupSfx)
            {
                //AudioUtility.CreateSFX(PickupSfx, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
            }

            if (PickupVfxPrefab)
            {
                var pickupVfxInstance = Instantiate(PickupVfxPrefab, transform.position, Quaternion.identity);
            }

            m_HasPlayedFeedback = true;
        }
    }
}
