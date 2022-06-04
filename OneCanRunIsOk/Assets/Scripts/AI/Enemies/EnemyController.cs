using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    // 敌人控制器类，敌人定义自己的行为并将其控制权交给控制器
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
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

        // 对应敌人的四个事件：攻击、发现、丢失、受伤和死亡
        public UnityAction onAttack;
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;
        public UnityAction onDie;

        // 最新受伤时间，用于仇恨丢失
        float lastTimeDamaged = float.NegativeInfinity;

        // 引入敌人的检测模块
        public EnemyDetectionModule EnemyDetectionModule { get; private set; }
        // 引入自带的AI寻路模块
        public NavMeshAgent NavMeshAgent { get; private set; }
        // 引入自定义的巡检路径
        public PatrolPath PatrolPath { get; set; }
        // 引入敌人的血量系统
        public Health health;
        // 引入角色
        public Actor actor;
        // 绑定状态，用于敌人的状态处理
        public GameObject KnownDetectedTarget => EnemyDetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => EnemyDetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => EnemyDetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => EnemyDetectionModule.HadKnownTarget;

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
        int pathDestinationNodeIndex;

        // Start is called before the first frame update
        void Start()
        {
            // 初始化
            enemyManager = FindObjectOfType<EnemyManager>();
            DebugUtility.HandleErrorIfNullFindObject<EnemyManager, EnemyController>(enemyManager, this);

            actorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyController>(actorsManager, this);

            gameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyController>(gameFlowManager, this);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(health, this, gameObject);

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyController>(actor, this, gameObject);

            colliders = GetComponentsInChildren<Collider>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, EnemyController>(NavMeshAgent, this, gameObject);

            // 注册敌人
            enemyManager.RegisterEnemy(this);

            // 订阅事件
            health.OnDie += OnDie;
            health.OnDamaged += OnDamaged;

            // 初始化检测模块
            EnemyDetectionModule[] enemyDetectionModules = GetComponentsInChildren<EnemyDetectionModule>();
            DebugUtility.HandleErrorIfNoComponentFound<EnemyDetectionModule, EnemyController>(enemyDetectionModules.Length, this,
                gameObject);
            DebugUtility.HandleWarningIfDuplicateObjects<EnemyDetectionModule, EnemyController>(enemyDetectionModules.Length,
                this, gameObject);
            EnemyDetectionModule = enemyDetectionModules[0];
            EnemyDetectionModule.onDetectedTarget += OnDetectTarget;
            EnemyDetectionModule.onLostTarget += OnLostTarget;
            onAttack += EnemyDetectionModule.OnAttack;

            NavigationModule[] navigationModules = GetComponentsInChildren<NavigationModule>();
            DebugUtility.HandleWarningIfDuplicateObjects<EnemyDetectionModule, EnemyController>(enemyDetectionModules.Length,
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

        // Update is called once per frame
        void Update()
        {
            // 确保所有敌人都在一定高度
            EnsureIsWithinLevelBounds();
            // 每帧都要检测
            EnemyDetectionModule.HandleDetection(actor, colliders);
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
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        // 判断巡检路径是否有效
        bool IsPathValid()
        {
            return true;
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
                Debug.Log(pathDestinationNodeIndex);
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
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        // 
        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                Debug.Log((transform.position - GetDestinationOnPath()).magnitude);
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

        // 处理受伤
        void OnDamaged(float damage, GameObject damageSource)
        {
            // 由伤害来源，且伤害来源不是敌人
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // 敌人受伤后要更新检测状态（无视检测范围）
                EnemyDetectionModule.OnDamaged(damageSource);

                // 敌人自定义受伤事件
                onDamaged?.Invoke();

                // 更新受伤时间
                lastTimeDamaged = Time.time;

                // 处理受伤音效

            }
        }

        void OnDie()
        {
            // tells the game flow manager to handle the enemy destuction
            enemyManager.UnregisterEnemy(this);

            // loot an object
            if (TryDropItem())
            {
                Instantiate(LootPrefab, transform.position, Quaternion.identity);
            }

            // this will call the OnDestroy function
            Destroy(gameObject, DeathDuration);
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