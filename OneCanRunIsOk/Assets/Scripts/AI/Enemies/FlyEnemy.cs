using OneCanRun.Game;
using OneCanRun.Game.Share;
using UnityEngine;

namespace OneCanRun.AI.Enemies
{
    [RequireComponent(typeof(Health), typeof(EnemyController))]
    public class FlyEnemy : MonoBehaviour
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

        // ��ǹ��ǹ����ȴʱ�䣬����ǹ�����������
        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        // ״̬ö�����ͣ�ö�ٵ�ÿ��������������һ��״̬���ֱ�Ϊ��Ѳ�죬׷��������
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

        public CharacterController characterController;

        public Vector3 prev = Vector3.zero;
        public Vector3 v = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            // ��ʼ��
            maxHealth = 10f;
            controller = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, FlyEnemy>(controller, this, gameObject);

            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, FlyEnemy>(health, this, gameObject);
            health.MaxHealth = maxHealth;

            characterController = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, FlyEnemy>(characterController, this, gameObject);

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
                        Fly(transform.position);
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

        // ����ǰ״̬��Ҫ������
        public virtual void UpdateCurrentAIState()
        {
            switch (state)
            {
                case AIState.Patrol:
                    controller.UpdatePathDestination();
                    Fly(controller.GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    Fly(controller.KnownDetectedTarget.transform.position);
                    // controller.SetNavDestination(controller.KnownDetectedTarget.transform.position);
                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    OrientWeaponsTowards(controller.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(controller.KnownDetectedTarget.transform.position,
                            controller.DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * controller.DetectionModule.AttackRange))
                    {
                        Fly(controller.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        Fly(transform.position);
                    }

                    controller.OrientTowards(controller.KnownDetectedTarget.transform.position);
                    if ((lastTimeWeaponSwapped + DelayAfterWeaponSwap) < Time.time && controller.TryAttack())
                    {
                        Attack(controller.KnownDetectedTarget.transform.position);
                    };
                    break;
            }
        }

        // �������¼�
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

        void Fly(Vector3 destination)
        {
            Vector3 direction = (destination - transform.position).normalized;
            Vector3 fly = direction * 2f;
            controller.OrientTowards(destination);
            characterController.Move(fly * Time.deltaTime);
        }

        // �����⵽Ŀ���¼�
        void OnDetectedTarget()
        {
            if (state == AIState.Patrol)
            {
                state = AIState.Follow;
            }
        }

        // ����ʧĿ���¼�
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
            controller.EnemyDamaged(damage, damageSource);

            // ������Ч
        }

        void OnDie()
        {
            controller.EnemyDie();
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
