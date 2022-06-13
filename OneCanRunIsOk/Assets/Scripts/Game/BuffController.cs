using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class BuffController
    {
        
        private Buff mbuff;
        public float getTime;
        public BuffController(Buff buff)
        {
            mbuff = buff;
            getTime = buff.ExistTime;
        }

        public Buff.BufferType getBuffType()
        {
            return mbuff.type;
        }

        public float getExistTime()
        {
            return mbuff.ExistTime;
        }

        public Buff getMBuff()
        {
            return mbuff;
        }

        public void WeaponbuffAct()
        {

        }
        public void ActorbuffAct(ref ActorProperties properties)
        {
            if (getBuffType() == Buff.BufferType.NumBuff)
            {
                properties.setHealRate(properties.getHealRate() + mbuff.getHealRateBuff());
                properties.setPhysicalAttack(properties.getPhysicalAttack() + mbuff.getPhysicalAttackBuff());
                properties.setMagicAttack(properties.getMagicAttack() + mbuff.getMagicAttackBuff());
                properties.setPhysicalDefence(properties.getPhysicalDefence() + mbuff.getPhysicalDefenceBuff());
                properties.setMagicDefence(properties.getMagicDefence() + mbuff.getMagicDefenceBuff());
                properties.setMaxSpeed(properties.getMaxSpeed() + mbuff.getMaxSpeedBuff());
                properties.setMaxJump(properties.getMaxJump() + mbuff.getMaxJumpBuff());
            }
            else if (getBuffType() == Buff.BufferType.PercentBuff)
            {
                properties.setHealRate(properties.getHealRate() * (1 + mbuff.getHealRateBuff()));
                properties.setPhysicalAttack(properties.getPhysicalAttack() * (1 + mbuff.getPhysicalAttackBuff()));
                properties.setMagicAttack(properties.getMagicAttack() * (1 + mbuff.getMagicAttackBuff()));
                properties.setPhysicalDefence(properties.getPhysicalDefence() * (1 + mbuff.getPhysicalDefenceBuff()));
                properties.setMagicDefence(properties.getMagicDefence() * (1 + mbuff.getMagicDefenceBuff()));
                properties.setMaxSpeed(properties.getMaxSpeed() * (1 + mbuff.getMaxSpeedBuff()));
                properties.setMaxJump(properties.getMaxJump() * (1 + mbuff.getMaxJumpBuff()));
            }
        }
    }
}

/*工作日志2022.6.8
            if (buff.type == Buff.BufferType.NumBuff)
            {
                healModifier = new Modifier(mbuff.getHealRateBuff(), Modifier.ModifierType.healRateBuff, this);
                PAttackModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.physicalAttackBuff, this);
                MAttackModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.magicAttackBuff, this);
                PDefenceModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.physicalDefenceBuff, this);
                MDefenceModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.magicDefenceBuff, this);
                SpeedModifier = new Modifier(mbuff.getMaxSpeedBuff(), Modifier.ModifierType.maxSpeedBuff, this);
                JumpModifier = new Modifier(mbuff.getMaxJumpBuff(), Modifier.ModifierType.maxJumpBuff, this);
            }
            else if(buff.type == Buff.BufferType.PercentBuff)
            {
                healModifier = new Modifier(mbuff.getHealRateBuff(), Modifier.ModifierType.p_healRateBuff, this);
                PAttackModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.p_physicalAttackBuff, this);
                MAttackModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.p_magicAttackBuff, this);
                PDefenceModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.p_physicalDefenceBuff, this);
                MDefenceModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.p_magicDefenceBuff, this);
                SpeedModifier = new Modifier(mbuff.getMaxSpeedBuff(), Modifier.ModifierType.p_maxSpeedBuff, this);
                JumpModifier = new Modifier(mbuff.getMaxJumpBuff(), Modifier.ModifierType.p_maxJumpBuff, this);
            }
 */