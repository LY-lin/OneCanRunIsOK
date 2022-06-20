
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.GamePlay;
using OneCanRun.Game.Share;
using TMPro;
using OneCanRun.Game;
using System.Collections.Generic;

namespace OneCanRun.UI
{
    public class Backpack : MonoBehaviour
    {
        [Tooltip("Buff plane")]
        public RectTransform BuffPlane;

        [Tooltip("Buff Prefab")]
        public GameObject BuffPrefab;

        [Header("Skill Information")]
        [Tooltip("Image for F Skill")]
        public Image fSkill;

        [Tooltip("name for F skill")]
        public TextMeshProUGUI FName;

        [Tooltip("Description for F skill")]
        public TextMeshProUGUI Ftext;

        [Tooltip("Image for Q Skill")]
        public Image qSkill;

        [Tooltip("name for Q skill")]
        public TextMeshProUGUI QName;

        [Tooltip("Description for Q skill")]
        public TextMeshProUGUI Qtext;

        [Header("Weapon Information")]
        [Tooltip("Weapon 1 name")]
        public TextMeshProUGUI Weapon1Name;

        [Tooltip("Image for Weapon 1")]
        public Image Weapon1Img;

        [Tooltip("text for Descriping weapon 1")]
        public TextMeshProUGUI txtWeapon1;

        [Tooltip("Weapon 2 name")]
        public TextMeshProUGUI Weapon2Name;

        [Tooltip("Image for Weapon 2")]
        public Image Weapon2Img;
        
        [Tooltip("text for Descriping weapon 2")]
        public TextMeshProUGUI txtWeapon2;

        [Header("Payer Base Priority")]
        [Tooltip("stamina value")]
        public TextMeshProUGUI stamina;

        [Tooltip("Strength value")]
        public TextMeshProUGUI Strength;

        [Tooltip("Intelligence value")]
        public TextMeshProUGUI intelligence;

        [Tooltip("Techinque value")]
        public TextMeshProUGUI technique;

        [Header("Player Other Information")]
        [Tooltip("Physical damge")]
        public TextMeshProUGUI physicalDamage;

        [Tooltip("Magical damge")]
        public TextMeshProUGUI magicalDamage;

        [Tooltip("Physical Defence")]
        public TextMeshProUGUI physicalDefence;

        [Tooltip("Magical Denfence")]
        public TextMeshProUGUI magicalDefence;

        [Tooltip("speed value")]
        public TextMeshProUGUI speed;

        [Tooltip("Recovery value")]
        public TextMeshProUGUI recovery;

        [Tooltip("Hp Ratio")]
        public TextMeshProUGUI HpRatio;

        [Tooltip("Image for Hp Bar")]
        public Image HpBar;


        [Tooltip("Exp Ratio")]
        public TextMeshProUGUI ExpRatio;

        [Tooltip("Image for Exp Bar")]
        public Image ExpBar;

        PlayerWeaponsManager playerWeaponsManager;
        PlayerSkillsManager playerSkillsManager;
        ActorProperties actorProperties;
        ActorAttribute actorAttribute;
        Health health;
        Actor actor;

        ActorBuffManager actorBuffManager;
        List<BuffUI> m_Buffs = new List<BuffUI>();
   





        int counter = 0;
        // Start is called before the first frame update
        public void initialize(){

            GameObject player = GameObject.Find("Player1");
            DebugUtility.HandleErrorIfNullFindObject<GameObject, Backpack>(player, this);
            actor = player.GetComponent<Actor>();
            playerSkillsManager = player.GetComponent<PlayerSkillsManager>();
            playerWeaponsManager = player.GetComponent<PlayerWeaponsManager>();
            
            actorProperties = actor.GetActorProperties();
            actorAttribute = actor.actorAttribute;
            health = player.GetComponent<Health>();
            actorBuffManager = player.GetComponent<ActorBuffManager>();
            actorBuffManager.buffGained += AddBuff;
            actorBuffManager.buffLost += RemoveBuff;
        }

        // Update is called once per frame
        public void updateContent()
        {
            Debug.Log("Open Backpack");
            Weapon1Name.text = playerWeaponsManager.m_WeaponSlots[0].WeaponName;
            Weapon2Name.text = playerWeaponsManager.m_WeaponSlots[1].WeaponName;
            txtWeapon1.text = playerWeaponsManager.m_WeaponSlots[0].description;
            txtWeapon2.text = playerWeaponsManager.m_WeaponSlots[1].description;
            Weapon1Img.sprite = playerWeaponsManager.m_WeaponSlots[0].WeaponImg;
            Weapon2Img.sprite = playerWeaponsManager.m_WeaponSlots[1].WeaponImg;

            fSkill.sprite = playerSkillsManager.CurrentSkillInstance.SkillIcon;
            FName.text = playerSkillsManager.CurrentSkillInstance.SkillName;
            Ftext.text = playerSkillsManager.CurrentSkillInstance.SkillDescription;
            qSkill.sprite = playerSkillsManager.CurrentSpSkillInstance.SkillIcon;
            QName.text = playerSkillsManager.CurrentSpSkillInstance.SkillName;
            Qtext.text = playerSkillsManager.CurrentSpSkillInstance.SkillDescription;

            stamina.text =    "Stamina:      " + actorAttribute.stamina.ToString();
            Strength.text =        "Strength:     " + actorAttribute.strength.ToString();
            intelligence.text = "Intellect:      " + actorAttribute.intelligence.ToString();
            technique.text =    "Technique:  " + actorAttribute.technique.ToString();

            physicalDamage.text = "Physical Damage:   " + actorProperties.getPhysicalAttack().ToString();
            magicalDamage.text = "Magic Damage:       " + actorProperties.getMagicAttack().ToString();
            physicalDefence.text = "Physical Defence:   " + actorProperties.getPhysicalDefence().ToString();
            magicalDefence.text = "Magic Defence:       " + actorProperties.getMagicDefence().ToString();

            speed.text = "Speed:                       " + actorProperties.getMaxSpeed();
            recovery.text = "Recovery:  " + actorProperties.getHealRate();

            int maxhp = Mathf.RoundToInt(health.MaxHealth);
            int curhp = Mathf.RoundToInt(health.CurrentHealth);
            HpRatio.text = curhp.ToString() + " / " + maxhp.ToString();
            HpBar.fillAmount = health.CurrentHealth / health.MaxHealth;

            int curexp =Mathf.RoundToInt(actor.getExperience());
            int wexp = Mathf.RoundToInt( actor.getNextLevelCount());
            ExpRatio.text = curexp.ToString() + " / " + wexp.ToString();
            ExpBar.fillAmount = actor.getExperience() / actor.getNextLevelCount();
            BuffPrefab.gameObject.SetActive(true);
            Debug.Log(HpBar.fillAmount);
        }

        public void closeBuff()
        {
            BuffPrefab.gameObject.SetActive(false);
        }

        void AddBuff(BuffController buffController)
      {
            GameObject buffUIInstance = Instantiate(BuffPrefab, BuffPlane);
            BuffUI newBuffUI = buffUIInstance.GetComponent<BuffUI>();
            DebugUtility.HandleErrorIfNullGetComponent<BuffUI, Backpack>(newBuffUI, 
                this, buffUIInstance.gameObject);
            newBuffUI.Initialize(buffController, buffController.BuffName);
            m_Buffs.Add(newBuffUI);
        }

        void RemoveBuff(BuffController buffController)
        {
            int index = -1;

            for (int i = 0; i<m_Buffs.Count;i++)
            {
                if(m_Buffs[i].BuffName == buffController.BuffName)
                {
                    index = i;
                    Destroy(m_Buffs[i].gameObject);
                }
            }
            if(index>=0)
            {
                m_Buffs.RemoveAt(index);
            }
        }
    }
}
