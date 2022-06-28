using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game;
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
        public List<BodyMeleeController> MeleeList = new List<BodyMeleeController>();

        [Tooltip("attack animation which are carried by the enemy")]
        public List<string> AttackList = new List<string>();

        [Tooltip("attack animation duration which are carried by the enemy")]
        public List<float> IntervalList = new List<float>();

        [Tooltip("Melee limit attack range")]
        public float MeleeRange;

        [Tooltip("Weapon limit attack range")]
        public float WeaponRange;

        [Tooltip("Skill limit attack range")]
        public float SkillRange;

        [Tooltip("Skill limit attack range")]
        [Min(1.1f)]
        public float MeleeInterval = 2f;

        public enum AttackState
        {
            Melee,
            Weapon,
            Skill
        }

        public AttackState attackState = AttackState.Melee;

        WeaponController[] weapons;

        SkillController[] skills;

        BodyMeleeController[] melees;

        float[] intervals;

        string[] attacks;

        Actor actor;

        Animator anim;

        int currentWeaponIndex = 0;
        int currentSkillIndex = 0;
        int currentMeleeIndex = 0;
        int currentAttackIndex = 0;
        float duration = 0f;
        AttackState preAttackState;
        float latestMeleeAttackTime = float.NegativeInfinity;
        float latestWeaponAttackTime = float.NegativeInfinity;
        float latestSkillAttackTime = float.NegativeInfinity;
        bool Attacking = false;

        // Start is called before the first frame update
        void Start()
        {

            actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyAttackController>(actor, this, gameObject);
            
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
            foreach (BodyMeleeController melee in melees)
            {
                melee.init(actor);
            }

            intervals = IntervalList.ToArray();

            anim = GetComponent<Animator>();
            DebugUtility.HandleErrorIfNullGetComponent<Animator, EnemyAttackController>(anim, this, gameObject);

            attacks = AttackList.ToArray();
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
                    if(melees.Length != 0 && latestMeleeAttackTime + MeleeInterval <= Time.time)
                    {
                        Attacking = true;
                        melees[currentMeleeIndex].preOneAttack();
                        AttackByMelee();
                    }
                    break;
                case AttackState.Weapon:
                    if(weapons.Length !=0 && latestWeaponAttackTime + MeleeInterval <= Time.time)
                    {
                        Attacking = true;
                        AttackByWeapon(target);
                    }
                    break;
                case AttackState.Skill:
                    if(skills.Length != 0 && latestSkillAttackTime + MeleeInterval <= Time.time)
                    {
                        Attacking = true;
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

            currentAttackIndex = currentWeaponIndex;
            duration = intervals[currentAttackIndex];
            preAttackState = AttackState.Weapon;
            latestWeaponAttackTime = Time.time;
            anim.SetTrigger(attacks[currentAttackIndex]);
            weapon.HandleShootInputs(false, true, false);
        }

        public void AttackBySkill()
        {
            preAttackState = AttackState.Skill;
            currentAttackIndex = weapons.Length + currentSkillIndex;
            SkillController skillController = skills[currentSkillIndex];
            duration = intervals[currentAttackIndex];
            skillController.UseSkill();
        }

        public void AttackByMelee()
        {
            preAttackState = AttackState.Melee;
            latestMeleeAttackTime = Time.time;
            currentAttackIndex = Random.Range(weapons.Length + skills.Length, attacks.Length);
            duration = intervals[currentAttackIndex];
            if (attacks[currentAttackIndex] == "Laser")
            {
                anim.SetBool(attacks[currentAttackIndex], true);
            }
            else
            {
                anim.SetTrigger(attacks[currentAttackIndex]);
            }
            duration = intervals[currentAttackIndex];
            anim.SetTrigger(attacks[currentAttackIndex]);
        }

        public bool TryFinishAttack()
        {
            if (!Attacking)
            {
                return true;
            }
            bool flag = false;
            switch (preAttackState)
            {
                case AttackState.Melee:
                    if(latestMeleeAttackTime + duration <= Time.time)
                    {
                        if (attacks[currentAttackIndex] == "Laser")
                        {
                            anim.SetBool(attacks[currentAttackIndex], false);
                            LaserRealse laser = gameObject.GetComponent<LaserRealse>();
                            if(laser != null)
                            {
                                laser.StopLasering();
                            }
                        }
                        flag = true;
                        melees[currentMeleeIndex].SetAttacking(false);
                        Attacking = false;
                    }
                    break;
                case AttackState.Weapon:
                    if(latestWeaponAttackTime + duration <= Time.time)
                    {
                        Attacking = false;
                        flag = true;
                    }
                    break;
                case AttackState.Skill:
                    if (latestSkillAttackTime + duration <= Time.time)
                    {
                        Attacking = false;
                        flag = true;
                    }
                    break;
                default:
                    break;
            }
            return flag;
        }
    }
}
