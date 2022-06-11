using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
using OneCanRun.Game.Share;

namespace OneCanRun.AI.Pets
{
    public class Turret : MonoBehaviour
    {
        // �ٻ����AI״̬
        public enum AIState
        {
            Idle,
            Attack,
        }

        public AIState state;

        // �ٻ���ɷ�Ϊ���֣�һ����Ѫ����һ���ǳ���ʱ�䣬�������߽�������
        [Tooltip("Duration of summon object��s")]
        public float Duration = 5;

        // ��������
        [Header("Weapons Parameters")]
        [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        // �л������ӳ�
        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        public Health health;

        
        
        // ��Я������������
        public WeaponController[] weapons;
        // ��ǰʹ�õ�����
        public WeaponController currentWeapon;
        // ��ǰ��������
        int currentWeaponIndex;
        // �ϴ�ʹ������ʱ��
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
