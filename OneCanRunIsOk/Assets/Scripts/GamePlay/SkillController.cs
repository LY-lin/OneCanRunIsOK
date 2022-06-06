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
        
        [Header("增益类型设置")]

        [Header("召唤类型设置")]

        //上次使用技能时间
        float m_LastTimeUse = Mathf.NegativeInfinity;
        //适配成技能的武器控制器
        WeaponController m_SkillWeapon;
        //源预制件
        public GameObject SourcePrefab { get; set; }

        void Awake()
        {
            m_SkillWeapon = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, SkillController>(m_SkillWeapon,
                this, gameObject);
        }

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
                    UseCastSkill();
                    break;

                case SkillType.Buff:
                    UseBuffSkill();
                    break;

                case SkillType.Summon:
                    UseSummonSkill();
                    break;

                default:
                    break;
            }
            
            return true;
        }
        
        //投射技能实现
        void UseCastSkill()
        {
            m_SkillWeapon.HandleShootInputs(true, false, false);
            Debug.Log("Cast!");
        }

        //增益技能实现
        void UseBuffSkill()
        {
            Debug.Log("Buff!");
        }

        //召唤技能实现
        void UseSummonSkill()
        {
            Debug.Log("Summon!");
        }

    }
}
