using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniBT;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class AttackCondition : Conditional
    {
        [SerializeField]
        private float threshold = 10;

        private Boss boss;

        protected override void OnAwake()
        {
            boss = gameObject.GetComponent<Boss>();
            DebugUtility.HandleErrorIfNullGetComponent<Boss, AnimationAction>(boss, gameObject.GetComponent<BehaviorTree>(), gameObject);
        }

        protected override bool IsUpdatable()
        {
            return boss.GetDistance() < threshold;
        }
    }
}
