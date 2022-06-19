using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class BuffController
    {
        
        private Buff mbuff;
        bool isForever;
        //剩余持续时间
        public float getTime;

        public float ExistTime;
        public GameObject ImpactVfx;
        public Sprite BuffIcon;
        public string BuffName;
        public string Description;
        public string Num { get; private set; }
        public BuffController(Buff buff)
        {
            mbuff = buff;
            getTime = buff.ExistTime;
            ExistTime = buff.ExistTime;
            isForever = !buff.hasExistTime;
            GameObject ImpactVfx = buff.ImpactVfx;
    
            BuffName = buff.name;
            Description = buff.description;
        }

        public bool GetIsForever()
        {
            return isForever;
        }
        public Buff.BufferType getBuffType()
        {
            return mbuff.type;
        }

        public float getExistTime()
        {
            if (isForever)
                return -10;
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
            else if (getBuffType() == Buff.BufferType.PercentBuff)
            {

            }
        }
    }
}
