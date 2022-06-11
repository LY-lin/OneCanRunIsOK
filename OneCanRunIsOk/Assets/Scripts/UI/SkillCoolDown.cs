
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

        PlayerInputHandler playerInputHandler;
        SkillController m_skillController;
        float LastTimeButton = Mathf.NegativeInfinity;    //上次冷却中按下技能键的时间
        bool ifTip;
        // Start is called before the first frame update
        void Start()
        {
            PlayerSkillsManager playerSkillsManager = FindObjectOfType<PlayerSkillsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerSkillsManager, SkillCoolDown>(playerSkillsManager,
                this);

            playerInputHandler = FindObjectOfType<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerInputHandler, PlayerWeaponsManager>(playerInputHandler, this);

            Debug.Log(playerSkillsManager.CurrentSkillInstance);
            m_skillController = playerSkillsManager.CurrentSkillInstance;
            Cooling.gameObject.SetActive(false);
            Warning.gameObject.SetActive(false);
            ifTip = false;
        }

        // Update is called once per frame
        void Update()
        {
             
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

        }
    }
}
