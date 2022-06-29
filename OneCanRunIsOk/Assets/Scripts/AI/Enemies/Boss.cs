using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using UnityEngine.AI;
using OneCanRun.Game.Share;
using OneCanRun.GamePlay;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(ActorBuffManager))]
    public class Boss : MonoBehaviour
    {
        [Tooltip("Boss Name")]
        public string BossName;

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
        BodyMeleeController[] bodyMeleeControllers;

        public PatrolPath PatrolPath;

        public List<GameObject> LootPrefabList;

        Animator animator;
        CharacterController characterController;
        SpitFlame spitFlame;

        int pathDestinationNodeIndex;
        int hitNumber = 3;
        float lastAttackTime = Mathf.Infinity;
        float lastPlayTime = Mathf.Infinity;
        float Duration = 2.5f;
        bool CG = false;

        bool Awake = false;
        bool Attacking = false;
        Dictionary<string, BodyMeleeController> map;
        Collider HitBox;

        BossAwake trap;

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

            trap = GameObject.Find("ChestForBossAwake").GetComponent<BossAwake>();
            trap.bossAwake += BossAwake;
            // enemyAttackController = GetComponent<EnemyAttackController>();
            // DebugUtility.HandleErrorIfNullGetComponent<EnemyAttackController, Boss>(enemyAttackController, this, gameObject);

            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, Boss>(NavMeshAgent, this, gameObject);
            NavMeshAgent.enabled = true;

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, Boss>(actor, this, gameObject);
            
            colliders = GetComponentsInChildren<Collider>();
            DebugUtility.HandleErrorIfNoComponentFound<Collider, Boss>(colliders.Length, this, gameObject);

            bodyMeleeControllers = GetComponentsInChildren<BodyMeleeController>();
            DebugUtility.HandleErrorIfNoComponentFound<Collider, Boss>(bodyMeleeControllers.Length, this, gameObject);

            spitFlame = GetComponent<SpitFlame>();
            DebugUtility.HandleErrorIfNullGetComponent<SpitFlame, Boss>(spitFlame, this, gameObject);

            map = new Dictionary<string, BodyMeleeController>();

            foreach(Collider c in colliders)
            {
                if ( c.gameObject.name == "HitBox")
                {
                    HitBox = c;
                    HitBox.enabled = false;
                }
            }

            foreach(BodyMeleeController bodyMeleeController in bodyMeleeControllers)
            {
                bodyMeleeController.init(actor);
                Debug.Log(bodyMeleeController.gameObject.name);
                map.Add(bodyMeleeController.gameObject.name, bodyMeleeController);
            }

            lastAttackTime = Time.time;

            animator.SetBool("Sleep", true);
        }

        // Update is called once per frame
        void Update()
        {
            if (Awake)
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

        public bool GetAwake()
        {
            return Awake;
        }

        private void BossAwake()
        {
            Debug.Log("boss awake");

            SetCG(true);
            
        }

        public void SetCG(bool cg)
        {
            characterController.detectCollisions = !cg;
            HitBox.enabled = !cg;
            CG = cg;
        }

        public void SetAwake(bool awake)
        {
            Awake = awake;
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
            Vector3 fly = direction * 40f;
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

        public void SetAnimationTrigger(string Animation)
        {
            animator.SetTrigger(Animation);
        }

        public bool TryPlayAnimation(string Animation, float duration, bool flag)
        {
            animator.SetBool("Sleep", false);
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

        public bool TryAttack(string Attack, float duration, string colliderName)
        {
            if (detectionModule.KnownDetectedTarget && Vector3.Distance(detectionModule.KnownDetectedTarget.transform.position,
                detectionModule.DetectionSourcePoint.position) > detectionModule.AttackRange)
            {
                return false;
            }
            if(lastAttackTime <= Time.time)
            {
                if (Attacking)
                {
                    Attacking = false;
                    if (colliderName.Length != 0)
                    {
                        BodyMeleeController bodyMeleeController;
                        map.TryGetValue(colliderName, out bodyMeleeController);
                        bodyMeleeController.SetAttacking(false);
                    }
                    return false;
                }
                else
                {
                    lastAttackTime = Time.time + duration;
                    Attacking = true;
                    if (colliderName.Length != 0)
                    {
                        BodyMeleeController bodyMeleeController;
                        map.TryGetValue(colliderName, out bodyMeleeController);
                        bodyMeleeController.preOneAttack();
                    }
                    animator.SetTrigger(Attack);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        void OnDamaged(float damage, GameObject damageSource)
        {
            if (hitNumber > 0 && Mathf.FloorToInt((health.CurrentHealth * 4) / health.MaxHealth) == hitNumber)
            {
                hitNumber--;
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
            characterController.enabled = false;
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }

            //BossDie?.Invoke();
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
            Destroy(gameObject);
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
