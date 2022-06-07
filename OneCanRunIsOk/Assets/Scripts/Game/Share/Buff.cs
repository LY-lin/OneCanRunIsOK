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
        public float healRateNumBuff;
        //��������-physical Attack
        public float physicalAttackNumBuff;
        //ħ��������
        public float magicAttackNumBuff;
        //���������
        public float physicalDefenceNumBuff;
        //ħ��������
        public float magicDefenceNumBuff;
        //����ƶ��ٶ�
        public float maxSpeedNumBuff;
        //��Ծ��
        public float maxJumpNumBuff;

        [Header("Buff failure condition")]
        public bool hasExistTime = true;

        [Header("Buff Exist Time")]
        public int ExistTime;
    }
}
