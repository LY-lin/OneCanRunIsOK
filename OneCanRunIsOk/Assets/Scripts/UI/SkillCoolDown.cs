
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
        [Tooltip("Default foreground color")]
        public Color DefaultCoolColor;
        [Tooltip("Flash foreground color when  want to use but cooling")]
        public Color FlashCoolColor;
        [Tooltip("Skill Image")]
        public Image SkillIcon;

        [Tooltip("Text for noting cooling")]
        public TextMeshProUGUI Cooling;

        PlayerInputHandler playerInputHandler;
        SkillController m_skillController;
        float LastTimeButton = Mathf.NegativeInfinity;    //上次冷却中按下技能键的时间
        // Start is called before the first frame update
        void Start()
        {
            PlayerSkillsManager playerSkillsManager = FindObjectOfType<PlayerSkillsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerSkillsManager, SkillCoolDown>(playerSkillsManager,
                this);

            m_skillController = playerSkillsManager.GetComponent<SkillController>();
            DebugUtility.HandleErrorIfNullGetComponent<SkillController, SkillCoolDown>(m_skillController, this, playerSkillsManager.gameObject);

        }

        // Update is called once per frame
        void Update()
        {
            float lastTimeRatio = 1 - (Time.time - m_skillController.m_LastTimeUse / m_skillController.CoolingTime);
            CoolDownTimeImage.fillAmount = (lastTimeRatio) > 0 ? lastTimeRatio : 0 ;
            CoolDownTimeImage.gameObject.SetActive(lastTimeRatio < (1 - 0.1f / m_skillController.CoolingTime) && lastTimeRatio>0);

            if (CoolDownTimeImage.fillAmount > 0 && playerInputHandler.GetUseSkillButtonDown())
            {
                CoolDownTimeImage.color = FlashCoolColor;
                LastTimeButton = Time.time;
            }
            else
            {
                CoolDownTimeImage.color = DefaultCoolColor;
            }
            Cooling.gameObject.SetActive(Time.time - LastTimeButton > 1f);

        }
    }
}
