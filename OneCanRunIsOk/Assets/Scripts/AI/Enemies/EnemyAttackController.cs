using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.AI.Enemies
{
    public class EnemyAttackController : MonoBehaviour
    {
        [Tooltip("Weapons which are carried by the enemy")]
        public List<WeaponController> weaponList = new List<WeaponController>();

        [Tooltip("Melee limit attack range")]
        public float MeleeRange;

        [Tooltip("Weapon limit attack range")]
        public float WeaponRange;

        [Tooltip("Skill limit attack range")]
        public float SkillRange;

        public enum AttackState
        {
            Melee,
            Weapon,
            Skill
        }

        public AttackState attackState = AttackState.Melee;

        WeaponController[] weapons;

        int currentWeaponIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            weapons = weaponList.ToArray();
            foreach(WeaponController weapon in weapons)
            {
                weapon.Owner = gameObject;
            }
        }

        public void UpdateAttackState(Vector3 targetPostion)
        {
            float distance = (transform.position - targetPostion).magnitude;
            if (distance <= MeleeRange)
            {
                attackState = AttackState.Melee;
            }
            else if(distance <= WeaponRange)
            {
                attackState = AttackState.Weapon;
            }
            else
            {
                attackState = AttackState.Skill;
            }
        }

        public void Attack(Vector3 target)
        {
            switch (attackState)
            {
                case AttackState.Melee:
                    break;
                case AttackState.Weapon:
                    AttackByWeapon(target);
                    break;
                case AttackState.Skill:
                    break;
                default:
                    break;
            }
                
        }

        public void AttackByWeapon(Vector3 target)
        {
            WeaponController weapon = weapons[currentWeaponIndex];
            Vector3 weaponForward = (target - weapon.WeaponRoot.transform.position).normalized;
            weapon.transform.forward = weaponForward;

            weapon.HandleShootInputs(false, true, false);
        }

        public void AttackBySkill()
        {

        }

        public void AttackByMelee()
        {

        }
    }
}
