using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public class BuffController : MonoBehaviour
    {
        
        private Buff mbuff;
        public float getTime;

        public Modifier healModifier;
        public Modifier PAttackModifier;
        public Modifier MAttackModifier;
        public Modifier PDefenceModifier;
        public Modifier MDefenceModifier;
        public Modifier SpeedModifier;
        public Modifier JumpModifier;
        public BuffController(Buff buff)
        {
            mbuff = buff;
            getTime = 0;
            healModifier = new Modifier(mbuff.getHealRateBuff(), Modifier.ModifierType.healRateBuff, this);
            PAttackModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.physicalAttackBuff, this);
            MAttackModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.magicAttackBuff, this);
            PDefenceModifier = new Modifier(mbuff.getPhysicalAttackBuff(), Modifier.ModifierType.physicalDefenceBuff, this);
            MDefenceModifier = new Modifier(mbuff.getMagicAttackBuff(), Modifier.ModifierType.magicDefenceBuff, this);
            SpeedModifier = new Modifier(mbuff.getMaxSpeedBuff(), Modifier.ModifierType.maxSpeedBuff, this);
            JumpModifier = new Modifier(mbuff.getMaxJumpBuff(), Modifier.ModifierType.maxJumpBuff, this);
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
    }
}

/*工作日志2022.6.8
 * 以及完成数值类buff的生效过程，下面考虑武器附魔类如何生效。
 */