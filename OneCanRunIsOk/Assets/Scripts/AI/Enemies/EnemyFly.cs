using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyFly : MonoBehaviour
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

        Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyFly>(controller, this, gameObject);

            anim = GetComponent<Animator>();
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyFly>(anim, this, gameObject);

            rb = GetComponent<Rigidbody>();
            DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, EnemyFly>(rb, this, gameObject);

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
                        Fly(transform.position);
                    }
                    break;
                case AIState.Attack:
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
                    Fly(controller.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    Fly(controller.KnownDetectedTarget.transform.position);
                    // controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.DetectionModule.AttackRange))
                    {
                        Fly(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        Fly(transform.position);
                    }
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    controller.TryAttack();
                    break;
            }
        }

        void Fly(Vector3 destination)
        {
            Vector3 direction = (destination - transform.position).normalized;
            Vector3 fly = direction * 2f;
            controller.OrientTowards(destination);
            // characterController.Move(fly * Time.deltaTime);
            rb.AddForce(fly*Time.deltaTime);
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
