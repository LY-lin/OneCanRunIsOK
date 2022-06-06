using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneCanRun.Game.Share
{
    //技能类型
    public enum SkillType
    {
        //投射攻击-如火球等
        Cast,
        //增益状态-如恢复HP、提高攻击力等
        Buff,
        //召唤物-炮台、机器人、召唤物等
        Summon,
    }

    public class SkillController : MonoBehaviour
    {
        [Header("通用设置")]
        [Tooltip("技能名称")]
        public string SkillName;

        [Tooltip("技能描述")]
        public string SkillDescription;

        [Tooltip("技能图标")]
        public Sprite SkillIcon;

        [Tooltip("技能类型-投射/增益/召唤")]
        public SkillType m_SkillType;

        [Tooltip("技能冷却时间")]
        public float CoolingTime = 10f;

        [Header("投射类型设置")]
        [Tooltip("投射物模型")]
        public GameObject Projectile;

        [Tooltip("投射点")]
        public Transform CastMuzzle;

        [Header("增益类型设置")]

        [Header("召唤类型设置")]

        //上次使用技能时间
        float m_LastTimeUse = Mathf.NegativeInfinity;

        //void Awake()
        //{

        //}

        //void Update()
        //{

        //}

        //使用该技能
        public bool UseSkill()
        {
            //仍在冷却中
            if (m_LastTimeUse + CoolingTime > Time.time)
            {
                Debug.Log("Cooling!");
                return false;
            }

            m_LastTimeUse = Time.time;

            //判断技能类型
            switch (m_SkillType)
            {
                case SkillType.Cast:
                    doCastSkill();
                    break;

                case SkillType.Buff:
                    doBuffSkill();
                    break;

                case SkillType.Summon:
                    doSummonSkill();
                    break;

                default:
                    break;
            }
            
            return true;
        }
        
        //投射技能实现
        void doCastSkill()
        {
            Debug.Log("Cast!");
        }

        //增益技能实现
        void doBuffSkill()
        {
            Debug.Log("Buff!");
        }

        //召唤技能实现
        void doSummonSkill()
        {
            Debug.Log("Summon!");
        }
    }
}
