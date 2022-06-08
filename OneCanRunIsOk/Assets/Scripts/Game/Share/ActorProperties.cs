using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    //本类定义了一个角色的所有基础数值，This class defines all the base values for a role
    public class ActorProperties
    {

        // experience
        private ulong EXP;

        public ulong getEXP(){
            return EXP;
        }

        public void setEXP(ulong exp){
            EXP = exp;
        }

        //最大生命值
        private float maxHealth;
        public float getMaxHealth()
        {
            return maxHealth;
        }
        public bool setMaxHealth(float newHealth)
        {
            maxHealth = newHealth;
            return true;
        }
        
        //生命值恢复速度
        private float healRate;
        public float getHealRate()
        {
            return healRate;
        }
        public bool setHealRate(float newHealRate)
        {
            healRate = newHealRate;
            return true;
        }

        //物理攻击力-physical Attack
        private float physicalAttack;
        public float getPhysicalAttack()
        {
            return physicalAttack;
        }
        public bool setPhysicalAttack(float newPhyAttack)
        {
            physicalAttack = newPhyAttack;
            return true;
        }

        //魔法攻击力
        private float magicAttack;
        public float getMagicAttack()
        {
            return magicAttack;
        }
        public bool setMagicAttack(float newMagicAttack)
        {
            magicAttack = newMagicAttack;
            return true;
        }

        //物理防御力
        private float physicalDefence;
        public float getPhysicalDefence()
        {
            return physicalDefence;
        }
        public bool setPhysicalDefence(float newPhyDefence)
        {
            physicalDefence = newPhyDefence;
            return true;
        }

        //魔法防御力
        private float magicDefence;
        public float getMagicDefence()
        {
            return magicDefence;
        }
        public bool setMagicDefence(float newMagicDefence)
        {
            magicDefence = newMagicDefence;
            return true;
        }

        //最高移动速度
        private float maxSpeed;
        public float getMaxSpeed()
        {
            return maxSpeed;
        }
        public bool setMaxSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
            return true;
        }

        //跳跃力
        private float maxJump;
        public float getMaxJump()
        {
            return maxJump;
        }
        public bool setMaxJump(float newJump)
        {
            maxJump = newJump;
            return true;
        }
    }
}
