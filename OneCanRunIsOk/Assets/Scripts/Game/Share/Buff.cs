using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class Buff : MonoBehaviour
    {
        
        public enum BufferType
        {
            NumBuff,
            PercentBuff,
            WeaponBuff,
            ReburnBuff
        }
        [Header("Buff Type")]
        public BufferType type;

        //生命值恢复速度
        [Header("Buff(Only For NumBuff)")]
        public float healRateBuff;
        //物理攻击力-physical Attack
        public float physicalAttackBuff;
        //魔法攻击力
        public float magicAttackBuff;
        //物理防御力
        public float physicalDefenceBuff;
        //魔法防御力
        public float magicDefenceBuff;
        //最高移动速度
        public float maxSpeedBuff;
        //跳跃力
        public float maxJumpBuff;

        public float getHealRateBuff() { return healRateBuff; }
        public float getPhysicalAttackBuff() { return physicalAttackBuff; }
        public float getMagicAttackBuff() { return magicAttackBuff; }
        public float getPhysicalDefenceBuff() { return physicalDefenceBuff; }
        public float getMagicDefenceBuff() { return magicDefenceBuff; }
        public float getMaxSpeedBuff() { return maxSpeedBuff; }
        public float getMaxJumpBuff() { return maxJumpBuff; }

        [Header("Buff failure condition")]
        public bool hasExistTime = true;

        [Header("Buff Exist Time")]
        public float ExistTime;
    }
}
