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

        //����ֵ�ָ��ٶ�
        [Header("Buff(Only For NumBuff)")]
        public float healRateBuff;
        //����������-physical Attack
        public float physicalAttackBuff;
        //ħ��������
        public float magicAttackBuff;
        //����������
        public float physicalDefenceBuff;
        //ħ��������
        public float magicDefenceBuff;
        //����ƶ��ٶ�
        public float maxSpeedBuff;
        //��Ծ��
        public float maxJumpBuff;

        public string name;
        public string description;

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
