using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class MovableEnemy : MonoBehaviour
    {
        // 敌人在达到攻击距离后，再缩小一定攻击距离才停止移动
        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("Maximum amount of health")]
        public float maxHealth = 100f;

        // 武器参数，决定是否可以切换武器
        [Header("Weapons Parameters")]
        [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        // 换枪后开枪的冷却时间，允许换枪动画播放完毕
        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        // 状态枚举类型，枚举的每个变量代表怪物的一个状态，分别为：巡检，追击，攻击
        public enum AIState
        {
            Patrol,
            Follow,
            Attack,
        }

        // 怪物的AI状态，根据当前状态调用怪物对应的执行动作
        public AIState state;

        // 敌人控制器
        public EnemyController controller;
        // 血量系统
        public Health health;
        // 武器系统
        // 记录最近一次射击时间
        float lastTimeWeaponSwapped = Mathf.NegativeInfinity;
        // 当前武器索引，如果是存在多个武器
        int currentWeaponIndex;
        // 一个武器对应一个武器控制器
        WeaponController currentWeapon;
        // 获取敌人身上所有绑定的武器
        WeaponController[] weapons;

        // Start is called before the first frame update
        void Start()
        {
            // 鍒濆鍩烘湰灞炴�?
            maxHealth = 10f;
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, MovableEnemy>(controller, this, gameObject);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, MovableEnemy>(health, this, gameObject);
            health.MaxHealth = maxHealth;

            // 初始化武器
            FindAndInitializeAllWeapons();
            currentWeapon = GetCurrentWeapon();
            currentWeapon.ShowWeapon(true);

            // 初始化为巡检状态
            state = AIState.Patrol;

            //订阅事件
            controller.onDetectedTarget += OnDetectedTarget;
            controller.onLostTarget += OnLostTarget;
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;

        }

        // Update is called once per frame
        void Update()
        {
            UpdateAiStateTransitions();

            UpdateCurrentAIState();
        }

        // 状态变换
        public virtual void UpdateAiStateTransitions()
        {
            switch (state)
            {
                case AIState.Follow:
                    if (controller.IsSeeingTarget && controller.IsTargetInAttackRange)
                    {
                        state = AIState.Attack;
                        controller.SetNavDestination(transform.position);
                    }
                    break;
                case AIState.Attack:
                    if (!controller.IsTargetInAttackRange)
                    {
                        state = AIState.Follow;
                    }
                    break;
            }
        }

        // 处理当前状态需要做的事
        public virtual void UpdateCurrentAIState()
        {
            switch (state)
            {
                case AIState.Patrol:
                    controller.UpdatePathDestination();
                    controller.SetNavDestination(controller.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    OrientWeaponsTowards(controller.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.EnemyDetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.EnemyDetectionModule.AttackRange))
                    {
                        controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        controller.SetNavDestination(transform.position);
                    }

                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    if((lastTimeWeaponSwapped + DelayAfterWeaponSwap) < Time.time && controller.TryAttack())
                    {
                        Attack(controller.KnownDetectedTarget.transform.position);
                    };
                    break;
            }
        }

        // 处理攻击事件
        void Attack(Vector3 enemyPosition)
        {
            // 敌人，武器根部两点连线即武器朝向
            OrientWeaponsTowards(enemyPosition);

            // 射击
            bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

            if (didFire)
            {
                // 攻击特效

                // 如果存在多种武器，并且可以切换武器，切换武器
                if (SwapToNextWeapon && weapons.Length > 1)
                {
                    int nextWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
                    SetCurrentWeapon(nextWeaponIndex);
                }
            }
        }

        // 处理检测到目标事件
        void OnDetectedTarget()
        {
            if (state == AIState.Patrol)
            {
                state = AIState.Follow;
            }
        }

        // 处理丢失目标事件
        void OnLostTarget()
        {
            if (state == AIState.Follow || state == AIState.Attack)
            {
                state = AIState.Patrol;
            }
        }

        // 处理受伤事件
        void OnDamaged(float damage, GameObject damageSource)
        {
            controller.EnemyDamaged(damage, damageSource);

            // 受伤特效
        }

        void OnDie()
        {
            Debug.Log(gameObject.name + " Die");
            controller.EnemyDie();
        }

        // 找到并初始化所有武器
        void FindAndInitializeAllWeapons()
        {
            if (weapons == null)
            {
                weapons = GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyController>(weapons.Length, this, gameObject);

                // 初始化所有武器的拥有者
                foreach (WeaponController weapon in weapons)
                {
                    weapon.Owner = gameObject;
                }
            }
        }

        // 返回当前武器
        WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            if (currentWeapon == null)
            {
                SetCurrentWeapon(0);
            }
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyController>(currentWeapon, this, gameObject);
            return currentWeapon;
        }

        // 设置当前武器
        void SetCurrentWeapon(int index)
        {
            currentWeaponIndex = index;
            currentWeapon = weapons[currentWeaponIndex];
            lastTimeWeaponSwapped = SwapToNextWeapon ? Time.time : Mathf.NegativeInfinity;
        }

        // 调整武器朝向
        // 调整武器方向
        public void OrientWeaponsTowards(Vector3 lookPostion)
        {
            // 计算所有武器的朝向，并更新
            foreach (WeaponController weapon in weapons)
            {
                Vector3 weaponForward = (lookPostion - weapon.WeaponRoot.transform.position).normalized;
                weapon.transform.forward = weaponForward;
            }
        }
    }
}
