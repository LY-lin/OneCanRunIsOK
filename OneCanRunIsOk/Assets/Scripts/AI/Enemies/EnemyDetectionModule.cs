using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    public class EnemyDetectionModule : MonoBehaviour
    {
        [Tooltip("The point representing the source of target-detection raycasts for the enemy AI")]
        public Transform DetectionSourcePoint;

        [Tooltip("The max distance at which the enemy can see targets")]
        public float DetectionRange = 20f;

        [Tooltip("The max distance at which the enemy can attack its target")]
        public float AttackRange = 10f;

        [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
        public float KnownTargetTimeout = 4f;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;

        ActorsManager manager;

        public GameObject KnownDetectedTarget { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsSeeingTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }

        protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

        // Start is called before the first frame update
        void Start()
        {
            manager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyDetectionModule>(manager, this);
        }

        public virtual void HandleDetection(Actor self, Collider[] selfColliders)
        {
            float sqrDetectRange = DetectionRange * DetectionRange;
            IsSeeingTarget = false;
            float minDistance = Mathf.Infinity;
            // 遍历场景中所有角色
            foreach (Actor actor in manager.Actors)
            {
                // 属于不同阵营
                if(actor.Affiliation != self.Affiliation)
                {
                    float sqrDistance = (actor.transform.position - DetectionSourcePoint.position).sqrMagnitude;
                    if (sqrDistance < sqrDetectRange && sqrDistance < minDistance)
                    {
                        // 获取视线上一定距离的所有对象
                        RaycastHit[] hits = Physics.RaycastAll(DetectionSourcePoint.position,
                            (actor.AimPoint.position - DetectionSourcePoint.position).normalized, DetectionRange,
                            -1, QueryTriggerInteraction.Ignore);
                        // 最终目标是最近的对象，新建对象保存该目标
                        RaycastHit target = new RaycastHit();
                        // 初始化，距离无穷远
                        target.distance = Mathf.Infinity;
                        // 不一定找得到
                        bool found = false;
                        foreach (RaycastHit hit in hits)
                        {
                            // 对象不包含自己的组件并且距离更小时，更新目标并设置found
                            if(!selfColliders.Contains(hit.collider) && hit.distance < target.distance)
                            {
                                target = hit;
                                found = true;
                            }
                        }

                        // 找到了
                        if (found)
                        {
                            // 获取对象角色
                            Actor hitActor = target.collider.GetComponent<Actor>();
                            // 角色是同一个
                            if (hitActor == actor)
                            {
                                // 设置看到目标状态，并更新当前最短距离
                                IsSeeingTarget = true;
                                minDistance = sqrDistance;

                                // 记录看到目标的最新事件以及监测到的对象
                                TimeLastSeenTarget = Time.time;
                                KnownDetectedTarget = actor.AimPoint.gameObject;
                            }
                        }
                    }
                }
            }

            // 判定是否在攻击范围内部
            IsTargetInAttackRange = KnownDetectedTarget != null &&
                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= AttackRange;

            // 如果不知道目标但知道检测到目标，就要启动检测到事件
            if (!HadKnownTarget && KnownDetectedTarget != null)
            {
                OnDetect();
            }

            if (HadKnownTarget && KnownDetectedTarget == null)
            {
                OnLostTarget();
            }
        }

        // 丢失目标事件
        public virtual void OnLostTarget() => onLostTarget?.Invoke();

        // 检测到玩家时间
        public virtual void OnDetect() => onDetectedTarget?.Invoke();

        // 收到伤害事件（无视检测范围）
        public virtual void OnDamaged(GameObject damageSource)
        {
            TimeLastSeenTarget = Time.time;
            KnownDetectedTarget = damageSource;

        }

        // 攻击事件
        public virtual void OnAttack()
        {

        }
    }
}
