using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.AI.Pets
{
    public class Turret : MonoBehaviour
    {
        // 召唤物的AI状态
        public enum AIState
        {
            Idle,
            Attack,
        }

        public AIState state;

        // 召唤物可分为两种，一种是血量，一种是持续时间，或者两者皆有限制
        [Tooltip("Duration of summon object：s")]
        public float Duration = 5;

        // 武器参数
        [Header("Weapons Parameters")]
        [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        // 切换武器延迟
        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        public Health health;

        
        
        // 所携带的所有武器
        public WeaponController[] weapons;
        // 当前使用的武器
        public WeaponController currentWeapon;
        // 当前武器索引
        int currentWeaponIndex;
        // 上次使用武器时间
        float lastTimeWeaponSwapped = Mathf.NegativeInfinity;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, Turret>(health, this, gameObject);


        }

        // Update is called once per frame
        void Update()
        {

        }

        void FindAndInitializeAllWeapons()
        {
            if (weapons == null)
            {
                weapons = GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, PetController>(weapons.Length, this, gameObject);

                foreach (WeaponController weapon in weapons)
                {
                    weapon.Owner = gameObject;
                }
            }
        }

        WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            if (currentWeapon == null)
            {
                SetCurrentWeapon(0);
            }
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, PetController>(currentWeapon, this, gameObject);
            return currentWeapon;
        }

        void SetCurrentWeapon(int index)
        {
            currentWeaponIndex = index;
            currentWeapon = weapons[currentWeaponIndex];
            lastTimeWeaponSwapped = SwapToNextWeapon ? Time.time : Mathf.NegativeInfinity;
        }

        public void OrientWeaponsTowards(Vector3 lookPostion)
        {
            foreach (WeaponController weapon in weapons)
            {
                Vector3 weaponForward = (lookPostion - weapon.WeaponRoot.transform.position).normalized;
                weapon.transform.forward = weaponForward;
            }
        }
    }
}
