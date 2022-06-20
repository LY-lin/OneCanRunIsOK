using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class FollowAction : Action
    {
        [SerializeField]
        private string animation;

        private Boss boss;

        private bool flag;

        public override void Awake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, AnimationAction>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);

            flag = true;
        }

        // Start is called before the first frame update
        protected override Status OnUpdate()
        {
            if (flag)
            {
                boss.SetAnimationBool("Idle", true);
                boss.SetAnimationBool(animation, flag);
                flag = false;
                return Status.Running;
            }
            if (boss.TryFollow())
            {
                return Status.Running;
            }
            return Status.Success;
        }
    }
}
