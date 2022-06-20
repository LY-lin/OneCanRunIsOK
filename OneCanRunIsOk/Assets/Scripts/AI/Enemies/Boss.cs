using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using UnityEngine.AI;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(ActorBuffManager))]
    public class Boss : MonoBehaviour
    {
        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        public enum AIState
        {
            Idle,
            Patrol,
            Follow,
            Attack,
        }

        public AIState state;
        
        public Health health;

        DetectionModule detectionModule;

        // public EnemyAttackController enemyAttackController;

        NavMeshAgent NavMeshAgent;

        public Actor actor;

        Collider[] colliders;

        public PatrolPath PatrolPath;

        public List<GameObject> LootPrefabList;

        Animator animator;
        int pathDestinationNodeIndex;
        float lastHitTime = Mathf.Infinity;
        float lastPlayTime = Mathf.Infinity;
        float Duration = 0;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            DebugUtility.HandleErrorIfNullGetComponent<Animator, Boss>(animator, this, gameObject);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Boss>(health, this, gameObject);
            health.OnDamaged = OnDamaged;
            health.OnDie = OnDie;

            detectionModule = GetComponentInChildren<DetectionModule>();
            DebugUtility.HandleErrorIfNullGetComponent<DetectionModule, Boss>(detectionModule, this, gameObject);
            detectionModule.onDetectedTarget = OnDetectTarget;
            detectionModule.onLostTarget = OnLostTarget;

            // enemyAttackController = GetComponent<EnemyAttackController>();
            // DebugUtility.HandleErrorIfNullGetComponent<EnemyAttackController, Boss>(enemyAttackController, this, gameObject);

            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, Boss>(NavMeshAgent, this, gameObject);
            NavMeshAgent.enabled = true;

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, Boss>(actor, this, gameObject);
            
            colliders = GetComponentsInChildren<Collider>();
            DebugUtility.HandleErrorIfNoComponentFound<Collider, Boss>(colliders.Length, this, gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            detectionModule.HandleDetection(actor, colliders);
        }

        void OnLostTarget()
        {
            // 调用绑定的敌人丢失事件
            state = AIState.Follow;
        }

        // 处理检测到目标
        void OnDetectTarget()
        {
            
        }

        // 处理视角，追踪玩家
        public void OrientTowards(Vector3 lookPosition)
        {
            // 计算射线方向
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        // 判断巡检路径是否有效
        bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        // 重置路径目的地
        public void resetPathDestination()
        {
            pathDestinationNodeIndex = 0;
        }

        // 设置目的地最近的导航路径
        public void SetPathDestinationToClosestNode()
        {
            // 路径有效
            if (IsPathValid())
            {
                int closestPathNodeIndex = 0;
                for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
                {
                    float distanceToPathNode = PatrolPath.GetDistanceToNode(transform.position, i);
                    if (distanceToPathNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                    {
                        closestPathNodeIndex = i;
                    }
                }

                pathDestinationNodeIndex = closestPathNodeIndex;
            }
            else
            {
                pathDestinationNodeIndex = 0;
            }
        }

        // 获取路径的目的地
        public Vector3 GetDestinationOnPath()
        {
            if (IsPathValid())
            {
                return PatrolPath.GetPositionOfPathNode(pathDestinationNodeIndex);
            }
            else
            {
                return transform.position;
            }
        }

        // 设置导航目的地
        public void SetNavDestination(Vector3 destination)
        {
            if (NavMeshAgent && NavMeshAgent.isActiveAndEnabled)
            {
                animator.SetBool("Run", true);
                NavMeshAgent.SetDestination(destination);
            }
        }

        // 更新巡检节点
        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
                {
                    // increment path destination index
                    pathDestinationNodeIndex =
                        inverseOrder ? (pathDestinationNodeIndex - 1) : (pathDestinationNodeIndex + 1);
                    if (pathDestinationNodeIndex < 0)
                    {
                        pathDestinationNodeIndex += PatrolPath.PathNodes.Count;
                    }

                    if (pathDestinationNodeIndex >= PatrolPath.PathNodes.Count)
                    {
                        pathDestinationNodeIndex -= PatrolPath.PathNodes.Count;
                    }
                }
            }
        }

        public void SetAnimationBool(string Animation, bool flag)
        {
            animator.SetBool(Animation, flag);
        }

        public bool TryPlayAnimation(string Animation, float duration, bool flag)
        {
            if (flag)
            {
                return lastPlayTime + duration > Time.time;
            }
            else
            {
                animator.SetBool(Animation, true);
                lastPlayTime = Time.time;
                return true;
            }
        }

        public bool TryFollow()
        {
            if(detectionModule.KnownDetectedTarget == null)
            {
                return true;
            }
            SetNavDestination(detectionModule.KnownDetectedTarget.transform.position);
            if (detectionModule.IsSeeingTarget && detectionModule.IsTargetInAttackRange)
            {
                state = AIState.Attack;
                return false;
            }
            return true;
        }

        public bool TryAttack(string Attack, float duration)
        {
            return true;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            if (lastHitTime == Mathf.Infinity || lastHitTime + Duration < Time.time)
            {
                lastHitTime = Time.time;
                animator.SetTrigger("isHit");
            }
            // test if the damage source is the player
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // pursue the player
                detectionModule.OnDamaged(damageSource);
            }
        }

        void OnDie()
        {
            Debug.Log("Enemy Die");

            animator.SetTrigger("isDie");

            NavMeshAgent.enabled = false;

        }

        public void EnterDeathQueue()
        {
            Debug.Log(this.gameObject.name);
            // loot an object
            foreach(GameObject LootPrefab in LootPrefabList)
            {
                if (TryDropItem())
                {
                    Instantiate(LootPrefab, transform.position, Quaternion.identity);
                }
            }

            // this will call the OnDestroy function
            //Destroy(gameObject, DeathDuration);
            Game.Share.MonsterPoolManager temp = Game.Share.MonsterPoolManager.getInstance();
            if (temp == null)
            {
                Destroy(gameObject);
            }
            else
            {
                temp.release(this.gameObject);
            }
        }

        public bool TryDropItem()
        {
            // TODO:
            return true;
        }

        public float GetDistance()
        {
            return Vector3.Magnitude(transform.position - detectionModule.KnownDetectedTarget.transform.position);
        }
    }
}
