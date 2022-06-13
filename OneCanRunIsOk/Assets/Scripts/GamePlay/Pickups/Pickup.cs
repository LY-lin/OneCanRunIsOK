using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        [Tooltip("���¶���Ƶ��")]
        public float VerticalBobFrequency = 1f;

        [Tooltip("���¶�������")]
        public float BobbingAmount = 1f;

        [Tooltip("��ת�ٶ�")] public float RotatingSpeed = 360f;

        [Tooltip("ʰȡ��Ч")] public AudioClip PickupSfx;
        [Tooltip("ʰȡ��Ч")] public GameObject PickupVfxPrefab;

        //����
        public Rigidbody PickupRigidbody { get; private set; }
        //��ײ��
        Collider m_Collider;
        //��ʼλ��
        Vector3 m_StartPosition;
        //�Ƿ��Ѳ���ʰȡ����
        bool m_HasPlayedFeedback;

        protected virtual void Start()
        {
            PickupRigidbody = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, Pickup>(PickupRigidbody, this, gameObject);
            m_Collider = GetComponent<Collider>();
            DebugUtility.HandleErrorIfNullGetComponent<Collider, Pickup>(m_Collider, this, gameObject);

            // ensure the physics setup is a kinematic rigidbody trigger
            //�Ƿ��˶�
            PickupRigidbody.isKinematic = true;
            //��������
            m_Collider.isTrigger = true;

            // Remember start position for animation
            //��¼��ʼλ��
            m_StartPosition = transform.position;
        }

        protected virtual void Update()
        {
            // Handle bobbing
            //���¶���
            float bobbingAnimationPhase = ((Mathf.Sin(Time.time * VerticalBobFrequency) * 0.5f) + 0.5f) * BobbingAmount;
            transform.position = m_StartPosition + Vector3.up * bobbingAnimationPhase;

            // Handle rotating
            //��ת
            transform.Rotate(Vector3.up, RotatingSpeed * Time.deltaTime, Space.Self);
        }

        //��ײ����
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

        //����ʰȡ����
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
