using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using OneCanRun.Game;

namespace OneCanRun.AI.Enemies
{
    // ���˿������࣬���˶����Լ�����Ϊ���������Ȩ����������
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        // ��͸߶ȣ��������������������ʧ
        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float minHeight = -20f;

        // ת���ٶȣ�����ת���ӽǵ��ٶ�/��ת�ٶ�
        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        // ��������ʱ�䣬����
        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;

        // ����ս��Ʒ
        [Header("Loot")]
        [Tooltip("The object this enemy can drop when dying")]
        public GameObject LootPrefab;

        // ���伸��
        [Tooltip("The chance the object has to drop")]
        [Range(0, 1)]
        public float DropRate = 1f;

        // ��Ӧ���˵��ĸ��¼������������֡���ʧ�����˺�����
        public UnityAction onAttack;
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;
        public UnityAction onDie;

        // ��������ʱ�䣬���ڳ�޶�ʧ
        float lastTimeDamaged = float.NegativeInfinity;

        // ������˵ļ��ģ��
        public EnemyDetectionModule EnemyDetectionModule { get; private set; }
        // �����Դ���AIѰ·ģ��
        public NavMeshAgent NavMeshAgent { get; private set; }
        // �����Զ����Ѳ��·��
        public PatrolPath PatrolPath { get; set; }
        // ������˵�Ѫ��ϵͳ
        public Health health;
        // �����ɫ
        public Actor actor;
        // ��״̬�����ڵ��˵�״̬����
        public GameObject KnownDetectedTarget => EnemyDetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => EnemyDetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => EnemyDetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => EnemyDetectionModule.HadKnownTarget;

        // ���˹�����
        EnemyManager enemyManager;
        // ��ɫ������
        ActorsManager actorsManager;
        // ��Ϸ���̹�����
        GameFlowManager gameFlowManager;
        // �����Լ������
        Collider[] colliders;
        // ��������ģ��
        NavigationModule navigationModule;
        int pathDestinationNodeIndex;

        // Start is called before the first frame update
        void Start()
        {
            // ��ʼ��
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

            // ע�����
            enemyManager.RegisterEnemy(this);

            // �����¼�
            health.OnDie += OnDie;
            health.OnDamaged += OnDamaged;

            // ��ʼ�����ģ��
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
            // ȷ�����е��˶���һ���߶�
            EnsureIsWithinLevelBounds();
            // ÿ֡��Ҫ���
            EnemyDetectionModule.HandleDetection(actor, colliders);
        }

        // ȷ�������Ʒ�Χ�ڲ�
        void EnsureIsWithinLevelBounds()
        {
            if (transform.position.y < minHeight)
            {
                Destroy(gameObject);
                return;
            }
        }

        // �����ʧĿ��
        void OnLostTarget()
        {
            // ���ð󶨵ĵ��˶�ʧ�¼�
            onLostTarget.Invoke();
        }

        // �����⵽Ŀ��
        void OnDetectTarget()
        {
            onDetectedTarget.Invoke();
        }

        // �����ӽǣ�׷�����
        public void OrientTowards(Vector3 lookPosition)
        {
            // �������߷���
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        // �ж�Ѳ��·���Ƿ���Ч
        bool IsPathValid()
        {
            return true;
        }

        // ����·��Ŀ�ĵ�
        public void resetPathDestination()
        {
            pathDestinationNodeIndex = 0;
        }

        // ����Ŀ�ĵ�����ĵ���·��
        public void SetPathDestinationToClosestNode()
        {
            // ·����Ч
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

        // ��ȡ·����Ŀ�ĵ�
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

        // ���õ���Ŀ�ĵ�
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

        // ��������
        void OnDamaged(float damage, GameObject damageSource)
        {
            // ���˺���Դ�����˺���Դ���ǵ���
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // �������˺�Ҫ���¼��״̬�����Ӽ�ⷶΧ��
                EnemyDetectionModule.OnDamaged(damageSource);

                // �����Զ��������¼�
                onDamaged?.Invoke();

                // ��������ʱ��
                lastTimeDamaged = Time.time;

                // ����������Ч

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
