using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class AttackAction : Action
    {
        [SerializeField]
        private string animation;

        [SerializeField]
        private float duration;

        [SerializeField]
        private string colliderName; 

        private Boss boss;

        public override void Awake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, AnimationAction>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);
        }

        protected override Status OnUpdate()
        {
            if (boss.TryAttack(animation, duration, colliderName))
            {
                return Status.Running;
            }
            return Status.Success;
        }
    }
}
