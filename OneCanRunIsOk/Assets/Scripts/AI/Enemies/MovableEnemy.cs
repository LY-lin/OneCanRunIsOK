using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public class MovableEnemy : MonoBehaviour
    {
        // �����ڴﵽ�������������Сһ�����������ֹͣ�ƶ�
        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("Maximum amount of health")]
        public float maxHealth = 100f;

        // ���������������Ƿ�����л�����
        [Header("Weapons Parameters")]
        [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        // ��ǹ��ǹ����ȴʱ�䣬������ǹ�����������
        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        // ״̬ö�����ͣ�ö�ٵ�ÿ���������������һ��״̬���ֱ�Ϊ��Ѳ�죬׷��������
        public enum AIState
        {
            Patrol,
            Follow,
            Attack,
        }

        // �����AI״̬�����ݵ�ǰ״̬���ù����Ӧ��ִ�ж���
        public AIState state;

        // ���˿�����
        public EnemyController controller;
        // Ѫ��ϵͳ
        public Health health;
        // ����ϵͳ
        // ��¼���һ�����ʱ��
        float lastTimeWeaponSwapped = Mathf.NegativeInfinity;
        // ��ǰ��������������Ǵ��ڶ������
        int currentWeaponIndex;
        // һ��������Ӧһ������������
        WeaponController currentWeapon;
        // ��ȡ�����������а󶨵�����
        WeaponController[] weapons;

        // Start is called before the first frame update
        void Start()
        {
            // 初始基本属�?
            maxHealth = 10f;
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, MovableEnemy>(controller, this, gameObject);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, MovableEnemy>(health, this, gameObject);
            health.MaxHealth = maxHealth;

            // ��ʼ������
            FindAndInitializeAllWeapons();
            currentWeapon = GetCurrentWeapon();
            currentWeapon.ShowWeapon(true);

            // ��ʼ��ΪѲ��״̬
            state = AIState.Patrol;

            //�����¼�
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

        // ״̬�任
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

        // ������ǰ״̬��Ҫ������
        public virtual void UpdateCurrentAIState()
        {
            switch (state)
            {
                case AIState.Patrol:
                    controller.UpdatePathDestination();
                    //Debug.Log(controller.GetDestinationOnPath());
                    controller.SetNavDestination(controller.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    OrientWeaponsTowards(controller.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.DetectionModule.AttackRange))
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

        // ���������¼�
        void Attack(Vector3 enemyPosition)
        {
            // ���ˣ����������������߼���������
            OrientWeaponsTowards(enemyPosition);

            // ���
            bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

            if (didFire)
            {
                // ������Ч

                // ������ڶ������������ҿ����л��������л�����
                if (SwapToNextWeapon && weapons.Length > 1)
                {
                    int nextWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
                    SetCurrentWeapon(nextWeaponIndex);
                }
            }
        }

        // ������⵽Ŀ���¼�
        void OnDetectedTarget()
        {
            if (state == AIState.Patrol)
            {
                state = AIState.Follow;
            }
        }

        // ������ʧĿ���¼�
        void OnLostTarget()
        {
            if (state == AIState.Follow || state == AIState.Attack)
            {
                state = AIState.Patrol;
            }
        }

        // ���������¼�
        void OnDamaged(float damage, GameObject damageSource)
        {
            //Debug.Log("Enemy On Damaged, current health: " + health.CurrentHealth);
            controller.EnemyDamaged(damage, damageSource);

            // ������Ч
        }

        void OnDie()
        {
            Debug.Log(gameObject.name + " Die");
        }

        // �ҵ�����ʼ����������
        void FindAndInitializeAllWeapons()
        {
            if (weapons == null)
            {
                weapons = GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyController>(weapons.Length, this, gameObject);

                // ��ʼ������������ӵ����
                foreach (WeaponController weapon in weapons)
                {
                    weapon.Owner = gameObject;
                }
            }
        }

        // ���ص�ǰ����
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

        // ���õ�ǰ����
        void SetCurrentWeapon(int index)
        {
            currentWeaponIndex = index;
            currentWeapon = weapons[currentWeaponIndex];
            lastTimeWeaponSwapped = SwapToNextWeapon ? Time.time : Mathf.NegativeInfinity;
        }

        // ������������
        // ������������
        public void OrientWeaponsTowards(Vector3 lookPostion)
        {
            // �������������ĳ��򣬲�����
            foreach (WeaponController weapon in weapons)
            {
                Vector3 weaponForward = (lookPostion - weapon.WeaponRoot.transform.position).normalized;
                weapon.transform.forward = weaponForward;
            }
        }
    }
}
