using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    //���ඨ����һ����ɫ�����л�����ֵ��This class defines all the base values for a role
    public class ActorProperties
    {
<<<<<<< HEAD
<<<<<<< HEAD
        //��Ϊ����Buff
        public ActorProperties()
        {
            maxHealth = 0;
            maxJump = 0;
            maxSpeed = 0;
            physicalAttack = 0;
            physicalDefence = 0;
            magicAttack = 0;
            magicDefence = 0;
            healRate = 0;
        }

=======
=======
>>>>>>> a5118a3bd9fb77311ac0ae8aa284180ce3985e6e

        // experience
        private ulong EXP;

        public ulong getEXP(){
            return EXP;
        }

        public void setEXP(ulong exp){
            EXP = exp;
        }
<<<<<<< HEAD
>>>>>>> a5118a3bd9fb77311ac0ae8aa284180ce3985e6e
=======
>>>>>>> a5118a3bd9fb77311ac0ae8aa284180ce3985e6e

        //�������ֵ
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
        
        //����ֵ�ָ��ٶ�
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

        //��������-physical Attack
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

        //ħ��������
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

        //���������
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

        //ħ��������
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

        //����ƶ��ٶ�
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

        //��Ծ��
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
