using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;
using OneCanRun.GamePlay;

namespace OneCanRun.AI.Enemies
{
    public class EnemyAttackController : MonoBehaviour
    {
        [Tooltip("Weapons which are carried by the enemy")]
        public List<WeaponController> weaponList = new List<WeaponController>();

        [Tooltip("Skills which are carried by the enemy")]
        public List<SkillController> skillList = new List<SkillController>();

        [Tooltip("Skills which are carried by the enemy")]
        public List<WeaponController> MeleeList = new List<WeaponController>();

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

        SkillController[] skills;

        WeaponController[] melees;

        int currentWeaponIndex = 0;
        int currentSkillIndex = 0;
        int currentMeleeIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            
            weapons = weaponList.ToArray();
            foreach(WeaponController weapon in weapons)
            {
                weapon.Owner = gameObject;
            }

            skills = skillList.ToArray();
            foreach(SkillController skill in skills)
            {
                if(skill.m_SkillType == SkillType.Cast)
                {
                    skill.setWeaponOwner(gameObject);
                }
            }

            melees = MeleeList.ToArray();
            foreach (WeaponController melee in melees)
            {
                melee.Owner = gameObject;
                melee.InitMelee();
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
                    if(melees.Length != 0)
                    {
                        AttackByMelee();
                    }
                    break;
                case AttackState.Weapon:
                    if(weapons.Length !=0)
                    {
                        AttackByWeapon(target);
                    }
                    break;
                case AttackState.Skill:
                    if(skills.Length != 0)
                    {
                        AttackBySkill();
                    }
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
            SkillController skillController = skills[currentSkillIndex];
            skillController.UseSkill();
        }

        public void AttackByMelee()
        {
            WeaponController melee = melees[currentWeaponIndex];
            melee.HandleAttackInputs(false, true);
        }
    }
}
