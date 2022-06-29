using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class AnimationAction : Action
    {
        [SerializeField]
        private string animation;

        [SerializeField]
        private float duration;

        [SerializeField]
        private bool endCG;

        private Boss boss;

        private bool flag;

        public override void Awake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, AnimationAction>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);

            flag = false;
        }

        protected override Status OnUpdate()
        {
            if (boss.TryPlayAnimation(animation, duration, flag))
            {
                flag = true;
                return Status.Running;
            }
            else
            {
                boss.SetAnimationBool(animation, false);
                if (endCG)
                {
                    boss.SetCG(false);
                    boss.SetAwake(true);
                }
                return Status.Success;
            }
        }
    }
}
