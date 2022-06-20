using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class PatrolAction : Action
    {
        [SerializeField]
        private string animation;

        [SerializeField]
        private Transform end;

        private Boss boss;

        public override void Awake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, AnimationAction>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);
        }

        protected override Status OnUpdate()
        {
            if (!boss.Arrive(end.position))
            {
                boss.Fly(end.position);
                boss.SetAnimationBool("Fly", true);
                return Status.Running;
            }
            boss.SetAnimationBool("Fly", false);
            return Status.Success;
        }
    }
}
