
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.Game.Share;
using OneCanRun.Game;
using OneCanRun.GamePlay;
using TMPro;

namespace OneCanRun.UI
{

    public class SkillCoolDown : MonoBehaviour
    {
        [Tooltip("Image component displaying current last Cool down time")]
        public Image CoolDownTimeImage;
        [Tooltip("Warning cooling image")]
        public Image Warning;

        [Tooltip("Skill Image")]
        public Image SkillIcon;

        [Tooltip("Text for noting cooling")]
        public TextMeshProUGUI Cooling;

        [Tooltip("spSkill Image component displaying current last Cool down time")]
        public Image spCoolDownTimeImage;
        [Tooltip("spSkill Warning cooling image")]
        public Image spWarning;

        [Tooltip("spSkill Skill Image")]
        public Image spSkillIcon;

        [Tooltip("spSkill Text for noting cooling")]
        public TextMeshProUGUI spCooling;

        PlayerInputHandler playerInputHandler;
        SkillController m_skillController;
        SkillController m_SpSkillController;
        float LastTimeButton = Mathf.NegativeInfinity;    //上次冷却中按下技能键的时间
        float spLastTimeButton = Mathf.NegativeInfinity;

        bool ifTip;
        bool spIfTip;
        // Start is called before the first frame update
        void Start()
        {
            PlayerSkillsManager playerSkillsManager = FindObjectOfType<PlayerSkillsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerSkillsManager, SkillCoolDown>(playerSkillsManager,
                this);
            
            playerInputHandler = FindObjectOfType<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerInputHandler, PlayerWeaponsManager>(playerInputHandler, this);

            m_skillController = playerSkillsManager.CurrentSkillInstance;

            SkillIcon.sprite = m_skillController.SkillIcon;
            Cooling.gameObject.SetActive(false);
            Warning.gameObject.SetActive(false);
            ifTip = false;

            m_SpSkillController = playerSkillsManager.CurrentSpSkillInstance;
            spSkillIcon.sprite = m_SpSkillController.SkillIcon;
            spCooling.gameObject.SetActive(false);
            spWarning.gameObject.SetActive(false);
            spIfTip = false;
        }

        // Update is called once per frame
        void Update()
        {
             //普通技能
            if (CoolDownTimeImage.fillAmount >0 && playerInputHandler.GetUseSkillButtonDown())
            {
                LastTimeButton = Time.time;
                Cooling.gameObject.SetActive(true);
                ifTip = true;
                Warning.gameObject.SetActive(true);
            }
            float lastTimeRatio = 1 - (Time.time - m_skillController.m_LastTimeUse )/ m_skillController.CoolingTime;

            CoolDownTimeImage.fillAmount = (lastTimeRatio) > 0 ? lastTimeRatio : 0;
            CoolDownTimeImage.gameObject.SetActive(lastTimeRatio <= 1 && lastTimeRatio > 0);


            if (CoolDownTimeImage.fillAmount == 0 || (ifTip&&Time.time-LastTimeButton > 1f))
            {
                Cooling.gameObject.SetActive(false);
                Warning.gameObject.SetActive(false);
                ifTip = false;
            }

            //sp技能
            if (spCoolDownTimeImage.fillAmount > 0 && playerInputHandler.GetUseSPSkillButtonDown())
            {
                spLastTimeButton = Time.time;
                spCooling.gameObject.SetActive(true);
                spIfTip = true;
                spWarning.gameObject.SetActive(true);
            }
            float splastTimeRatio = 1 - (Time.time - m_SpSkillController.m_LastTimeUse) / m_SpSkillController.CoolingTime;

            spCoolDownTimeImage.fillAmount = (splastTimeRatio) > 0 ? splastTimeRatio : 0;
            spCoolDownTimeImage.gameObject.SetActive(splastTimeRatio <= 1 && splastTimeRatio > 0);


            if (spCoolDownTimeImage.fillAmount == 0 || (spIfTip && Time.time - spLastTimeButton > 1f))
            {
                spCooling.gameObject.SetActive(false);
                spWarning.gameObject.SetActive(false);
                spIfTip = false;
            }

        }
    }
}
