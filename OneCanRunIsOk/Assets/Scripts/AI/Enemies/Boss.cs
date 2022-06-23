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
        public float PathReachingRadius = 50f;

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

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
        CharacterController characterController;
        int pathDestinationNodeIndex;
        float lastHitTime = Mathf.Infinity;
        float lastAttackTime = Mathf.Infinity;
        float lastPlayTime = Mathf.Infinity;
        float Duration = 0;
        bool CG = true;

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

            characterController = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, Boss>(characterController, this, gameObject);

            // enemyAttackController = GetComponent<EnemyAttackController>();
            // DebugUtility.HandleErrorIfNullGetComponent<EnemyAttackController, Boss>(enemyAttackController, this, gameObject);

            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, Boss>(NavMeshAgent, this, gameObject);
            NavMeshAgent.enabled = true;

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, Boss>(actor, this, gameObject);
            
            colliders = GetComponentsInChildren<Collider>();
            DebugUtility.HandleErrorIfNoComponentFound<Collider, Boss>(colliders.Length, this, gameObject);

            

            lastAttackTime = Time.time;
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
            Debug.Log("Lost");
        }

        // 处理检测到目标
        void OnDetectTarget()
        {
            Debug.Log("Detect");
        }

        public bool GetCG()
        {
            return CG;
        }

        public void SetCG(bool cg)
        {
            CG = cg;
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
        public int SetPathDestinationToClosestNode()
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
            return pathDestinationNodeIndex;
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

        public void Fly(Vector3 destination)
        {
            NavMeshAgent.enabled = false;
            Vector3 direction = (destination - transform.position).normalized;
            Vector3 fly = direction * 20f;
            OrientTowards(destination);
            characterController.Move(fly * Time.deltaTime);
        }

        // 更新巡检节点
        public int UpdatePathDestination(bool inverseOrder = false)
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
            return pathDestinationNodeIndex;
        }

        public bool Arrive(Vector3 position)
        {
            return (transform.position - position).magnitude <= PathReachingRadius;
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
            NavMeshAgent.enabled = true;
            if(detectionModule.KnownDetectedTarget == null)
            {
                return true;
            }
            OrientTowards(detectionModule.KnownDetectedTarget.transform.position);
            if (Vector3.Distance(detectionModule.KnownDetectedTarget.transform.position,
                detectionModule.DetectionSourcePoint.position) >= (AttackStopDistanceRatio * detectionModule.AttackRange))
            {
                SetNavDestination(detectionModule.KnownDetectedTarget.transform.position);
            }
            else
            {
                SetNavDestination(transform.position);
            }
            
            if (detectionModule.IsSeeingTarget && detectionModule.IsTargetInAttackRange)
            {
                state = AIState.Attack;
                return false;
            }
            return true;
        }

        public bool TryAttack(string Attack, float duration)
        {
            if (Vector3.Distance(detectionModule.KnownDetectedTarget.transform.position,
                detectionModule.DetectionSourcePoint.position) > detectionModule.AttackRange)
            {
                return false;
            }
            if(lastAttackTime <= Time.time)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger(Attack);
                return true;
            }
            else if(lastAttackTime + duration > Time.time)
            {
                return true;
            }
            else
            {
                lastAttackTime += duration;
                return false;
            }
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
