using OneCanRun.Game;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class MovableEnemy : MonoBehaviour
    {
        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("Maximum distance of detection")]
        public float detectDistance;

        [Tooltip("Maximum amount of health")]
        public float maxHealth;

        [Tooltip("Maximum amount of attck distance")]
        public float attckDistance;

        public enum AIState
        {
            Patrol,
            Follow,
            Attack,
        }

        public AIState state;

        public EnemyController controller;

        // Start is called before the first frame update
        void Start()
        {
            // ��ʼ��������
            maxHealth = 10f;
            attckDistance = 10f;
            detectDistance = 20f;
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, MovableEnemy>(controller, this, gameObject);

            // ��ʼ��ΪѲ��״̬
            state = AIState.Patrol;

            //�����¼�
            controller.onAttack += OnAttack;
            controller.onDamaged += OnDamaged;
            controller.onDetectedTarget += OnDetectedTarget;
            controller.onLostTarget += OnLostTarget;
        }

        // Update is called once per frame
        void Update()
        {
            // �ȸ��µ�ǰ��״̬������õ�����״̬
            UpdateAiStateTransitions();

            // ����״ִ̬����Ӧ�Ķ���
            UpdateCurrentAIState();
        }

        public virtual void UpdateAiStateTransitions()
        {
            switch (state)
            {
                case AIState.Follow:
                    // ����Ŀ����Ŀ���ڹ�����Χ�ڲ�
                    if (controller.IsSeeingTarget && controller.IsTargetInAttackRange)
                    {
                        state = AIState.Attack;
                        controller.SetNavDestination(transform.position);
                    }
                    break;
                case AIState.Attack:
                    // ���ڹ�����Χ�ڲ�
                    if (!controller.IsTargetInAttackRange)
                    {
                        state = AIState.Follow;
                    }
                    break;
            }
        }

        public virtual void UpdateCurrentAIState()
        {
            switch (state)
            {
                case AIState.Patrol:
                    controller.UpdatePathDestination();
                    controller.SetNavDestination(controller.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    controller.OrientWeaponsTowards(controller.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.EnemyDetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.EnemyDetectionModule.AttackRange))
                    {
                        controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        controller.SetNavDestination(transform.position);
                    }

                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    controller.TryAttack(controller.KnownDetectedTarget.transform.position);
                    break;
            }
        }

        void OnAttack()
        {
            // ������Ч
        }

        void OnDetectedTarget()
        {
            if (state == AIState.Patrol)
            {
                state = AIState.Follow;
            }
        }

        void OnLostTarget()
        {
            if (state == AIState.Follow || state == AIState.Attack)
            {
                state = AIState.Patrol;
            }
        }

        void OnDamaged()
        {
            // ������Ч
        }
    }
}
