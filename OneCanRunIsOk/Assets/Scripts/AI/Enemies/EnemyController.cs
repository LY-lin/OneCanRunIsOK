using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.AI.Enemies
{
    // 敌人控制器类，敌人定义自己的行为并将其控制权交给控制器
    [RequireComponent(typeof(Actor), typeof(NavMeshAgent), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        // 最低高度，超出死亡，怪物掉落消失
        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float minHeight = -20f;

        // 转动速度，敌人转动视角的速度/旋转速度
        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        // 死亡持续时间，动画
        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;

        // 掉落战利品
        [Header("Loot")]
        [Tooltip("The object this enemy can drop when dying")]
        public GameObject LootPrefab;

        // 掉落几率
        [Tooltip("The chance the object has to drop")]
        [Range(0, 1)]
        public float DropRate = 1f;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;

        // 最新受伤时间，用于仇恨丢失
        float lastTimeDamaged = float.NegativeInfinity;

        // 引入敌人的检测模块
        public DetectionModule DetectionModule { get; private set; }
        // 引入自带的AI寻路模块
        public NavMeshAgent NavMeshAgent { get; private set; }
        // 引入自定义的巡检路径
        public PatrolPath PatrolPath { get; set; }

        public EnemyAttackController attackController;

        public Health health;

        public Actor actor;
        // 绑定状态，用于敌人的状态处理
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;

        // 敌人管理器
        EnemyManager enemyManager;
        // 角色管理器
        ActorsManager actorsManager;
        // 游戏进程管理器
        GameFlowManager gameFlowManager;
        // 保存自己的组件
        Collider[] colliders;
        // 导航数据模块
        NavigationModule navigationModule;

        Animator animator;
        
        int pathDestinationNodeIndex;

        float lastHitTime = Mathf.Infinity;

        // Start is called before the first frame update
        void Awake()
        {
            // 初始化
            enemyManager = FindObjectOfType<EnemyManager>();
            DebugUtility.HandleErrorIfNullFindObject<EnemyManager, EnemyController>(enemyManager, this);

            actorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyController>(actorsManager, this);

            gameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyController>(gameFlowManager, this);

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyController>(actor, this, gameObject);

            colliders = GetComponentsInChildren<Collider>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, EnemyController>(NavMeshAgent, this, gameObject);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(health, this, gameObject);

            attackController = GetComponent<EnemyAttackController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyAttackController, EnemyController>(attackController, this, gameObject);

            animator = GetComponent<Animator>();
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyController>(animator, this, gameObject);

            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;

            DetectionModule[] enemyDetectionModules = GetComponentsInChildren<DetectionModule>();
            DebugUtility.HandleErrorIfNoComponentFound<DetectionModule, EnemyController>(enemyDetectionModules.Length, this,
                gameObject);
            DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, EnemyController>(enemyDetectionModules.Length,
                this, gameObject);
            DetectionModule = enemyDetectionModules[0];
            DetectionModule.onDetectedTarget += OnDetectTarget;
            DetectionModule.onLostTarget += OnLostTarget;

            NavigationModule[] navigationModules = GetComponentsInChildren<NavigationModule>();
            DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, EnemyController>(enemyDetectionModules.Length,
                this, gameObject);
            // Override navmesh agent data
            if (navigationModules.Length > 0)
            {
                navigationModule = navigationModules[0];
                NavMeshAgent.speed = navigationModule.MoveSpeed;
                NavMeshAgent.angularSpeed = navigationModule.AngularSpeed;
                NavMeshAgent.acceleration = navigationModule.Acceleration;
            }
        }

        void Start()
        {
            // 注册敌人
            enemyManager.RegisterEnemy(this);
        }

        private void OnEnable()
        {
            NavMeshAgent.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            // 确保所有敌人都在一定高度
            EnsureIsWithinLevelBounds();
            // 每帧都要检测
            DetectionModule.HandleDetection(actor, colliders);
        }

        // 确保在限制范围内部
        void EnsureIsWithinLevelBounds()
        {
            if (transform.position.y < minHeight)
            {
                Destroy(gameObject);
                return;
            }
        }

        // 处理丢失目标
        void OnLostTarget()
        {
            // 调用绑定的敌人丢失事件
            onLostTarget.Invoke();
        }

        // 处理检测到目标
        void OnDetectTarget()
        {
            onDetectedTarget.Invoke();
        }

        // 处理视角，追踪玩家
        public void OrientTowards(Vector3 lookPosition)
        {
            // 计算射线方向
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if(lookDirection.sqrMagnitude != 0f)
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
                animator.SetFloat("Speed", NavMeshAgent.speed);
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

        public bool TryAttack()
        {
            if (gameFlowManager.GameIsEnding)
            {
                return false;
            }

            attackController.UpdateAttackState(KnownDetectedTarget.transform.position);
            attackController.Attack(KnownDetectedTarget.transform.position);

            return true;
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            if(lastHitTime == Mathf.Infinity || lastHitTime + 2f < Time.time)
            {
                lastHitTime = Time.time;
                animator.SetTrigger("isHit");
            }
            // test if the damage source is the player
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // pursue the player
                DetectionModule.OnDamaged(damageSource);

                onDamaged?.Invoke();
            }
        }

        void OnDie()
        {
            Debug.Log("Enemy Die");

            animator.SetTrigger("isDie");

            enemyManager.UnregisterEnemy(this);

            NavMeshAgent.enabled = false;

        }

        public void EnterDeathQueue()
        {
            Debug.Log(this.gameObject.name);
            // loot an object
            if (TryDropItem())
            {
                Instantiate(LootPrefab, transform.position, Quaternion.identity);
            }

            // this will call the OnDestroy function
            //Destroy(gameObject, DeathDuration);
            Game.Share.MonsterPoolManager temp = Game.Share.MonsterPoolManager.getInstance();
            if (temp == null)
            {
                Destroy(gameObject, DeathDuration);
            }
            else
            {
                temp.release(this.gameObject);
            }
        }

        public bool TryDropItem()
        {
            if (DropRate == 0 || LootPrefab == null)
                return false;
            else if (DropRate == 1)
                return true;
            else
                return (Random.value <= DropRate);
        }
    }
}