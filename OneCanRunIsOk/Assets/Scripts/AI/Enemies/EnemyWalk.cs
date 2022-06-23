using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyWalk : MonoBehaviour
    {
        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        public enum AIState
        {
            Patrol,
            Follow,
            Attack,
        }

        public AIState state;

        public EnemyController controller;

        Animator anim;

        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyWalk>(controller, this, gameObject);
            
            anim = GetComponent<Animator>();
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyWalk>(anim, this, gameObject);

            state = AIState.Patrol;

            controller.onDetectedTarget += OnDetectedTarget;
            controller.onLostTarget += OnLostTarget;
            controller.SetPathDestinationToClosestNode();
            controller.onDamaged += OnDamaged;

            anim.SetBool("isRun", true);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAiStateTransitions();

            UpdateCurrentAIState();
        }

        public virtual void UpdateAiStateTransitions()
        {
            switch (state)
            {
                case AIState.Follow:
                    if (controller.IsSeeingTarget && controller.IsTargetInAttackRange)
                    {
                        state = AIState.Attack;
                        controller.SetNavDestination(transform.position);
                    }
                    break;
                case AIState.Attack:
                    if (!controller.IsTargetInAttackRange)
                    {
                        anim.SetBool("isRun", true);
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
                    if (controller.attackController.TryFinishAttack())
                    {
                        controller.UpdatePathDestination();
                        controller.SetNavDestination(controller.GetDestinationOnPath());
                    }
                    else
                    {
                        controller.SetNavDestination(transform.position);
                        controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    }
                    break;
                case AIState.Follow:
                    if (controller.attackController.TryFinishAttack())
                    {
                        controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                        controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        controller.SetNavDestination(transform.position);
                        controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    }
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.DetectionModule.AttackRange) && controller.attackController.TryFinishAttack())
                    {
                        controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        controller.SetNavDestination(transform.position);
                    }

                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    controller.TryAttack();
                    break;
            }
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
            // GOTO: OnDamaged
        }
    }
}
