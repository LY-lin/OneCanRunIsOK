using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class BuffController : MonoBehaviour
    {   
        private Buff mbuff;
        private float getTime;
        public BuffController(Buff buff)
        {
            mbuff = buff;
            getTime = 0;
        }
        public float getGetTime()
        {
            return getTime;
        }
        public void setGetTime(float PTime)
        {
            getTime = PTime;
        }
        public Buff.BufferType getBuffType()
        {
            return mbuff.type;
        }

        public bool getHasExistTime()
        {
            return mbuff.hasExistTime;
        }
        public float getExistTime()
        {
            return mbuff.ExistTime;
        }

        public void ActorbuffAct(ref ActorProperties properties)
        {
            if (getBuffType()== Buff.BufferType.NumBuff)
            {
                properties.setHealRate(properties.getHealRate() + mbuff.getHealRateBuff());
                properties.setPhysicalAttack(properties.getPhysicalAttack() + mbuff.getPhysicalAttackBuff());
                properties.setMagicAttack(properties.getMagicAttack() + mbuff.getMagicAttackBuff());
                properties.setPhysicalDefence(properties.getPhysicalDefence() + mbuff.getPhysicalDefenceBuff());
                properties.setMagicDefence(properties.getMagicDefence() + mbuff.getMagicDefenceBuff());
                properties.setMaxSpeed(properties.getMaxSpeed() + mbuff.getMaxSpeedBuff());
                properties.setMaxJump(properties.getMaxJump() + mbuff.getMaxJumpBuff());
            }
            else if(getBuffType() == Buff.BufferType.PercentBuff)
            {
                properties.setHealRate(properties.getHealRate() * (1 + mbuff.getHealRateBuff()));
                properties.setPhysicalAttack(properties.getPhysicalAttack() * (1 + mbuff.getPhysicalAttackBuff()));
                properties.setMagicAttack(properties.getMagicAttack() * (1 + mbuff.getMagicAttackBuff()));
                properties.setPhysicalDefence(properties.getPhysicalDefence() * (1 + mbuff.getPhysicalDefenceBuff()));
                properties.setMagicDefence(properties.getMagicDefence() * (1 + mbuff.getMagicDefenceBuff()));
                properties.setMaxSpeed(properties.getMaxSpeed() * (1 + mbuff.getMaxSpeedBuff()));
                properties.setMaxJump(properties.getMaxJump() * (1 + mbuff.getMaxJumpBuff()));
            }
            else if(getBuffType() == Buff.BufferType.WeaponBuff) { }
            else { }

        }

        public void WeaponbuffAct()
        {

        }
    }
}

/*工作日志2022.6.8
 * 以及完成数值类buff的生效过程，下面考虑武器附魔类如何生效。
 */