using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class OnlyOneCondition : Conditional
    {
        private Boss boss;

        protected override void OnAwake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, OnlyOneCondition>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);
        }

        protected override bool IsUpdatable()
        {
            return boss.GetCG();
        }
    }
}
