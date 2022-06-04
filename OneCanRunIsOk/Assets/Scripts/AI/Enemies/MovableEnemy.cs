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
            // 初始基本属性
            maxHealth = 10f;
            attckDistance = 10f;
            detectDistance = 20f;
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, MovableEnemy>(controller, this, gameObject);

            // 初始化为巡检状态
            state = AIState.Patrol;

            //订阅事件
            controller.onAttack += OnAttack;
            controller.onDamaged += OnDamaged;
            controller.onDetectedTarget += OnDetectedTarget;
            controller.onLostTarget += OnLostTarget;
        }

        // Update is called once per frame
        void Update()
        {
            // 先更新当前的状态变更，得到最新状态
            UpdateAiStateTransitions();

            // 根据状态执行相应的动作
            UpdateCurrentAIState();
        }

        public virtual void UpdateAiStateTransitions()
        {
            switch (state)
            {
                case AIState.Follow:
                    // 看到目标且目标在攻击范围内部
                    if (controller.IsSeeingTarget && controller.IsTargetInAttackRange)
                    {
                        state = AIState.Attack;
                        controller.SetNavDestination(transform.position);
                    }
                    break;
                case AIState.Attack:
                    // 不在攻击范围内部
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
            // 攻击特效
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
            // 受伤特效
        }
    }
}