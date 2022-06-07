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
        public float healRateNumBuff;
        //物理攻击力-physical Attack
        public float physicalAttackNumBuff;
        //魔法攻击力
        public float magicAttackNumBuff;
        //物理防御力
        public float physicalDefenceNumBuff;
        //魔法防御力
        public float magicDefenceNumBuff;
        //最高移动速度
        public float maxSpeedNumBuff;
        //跳跃力
        public float maxJumpNumBuff;

        [Header("Buff failure condition")]
        public bool hasExistTime = true;

        [Header("Buff Exist Time")]
        public int ExistTime;
    }
}
