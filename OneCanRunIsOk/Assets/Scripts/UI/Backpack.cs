
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.GamePlay;
using OneCanRun.Game.Share;
using TMPro;
using OneCanRun.Game;

namespace OneCanRun.UI
{
    public class Backpack : MonoBehaviour
    {
        [Header("Skill Information")]
        [Tooltip("Image for F Skill")]
        public Image fSkill;

        [Tooltip("text for F skill")]
        public TextMeshProUGUI Ftext;

        [Tooltip("Image for Q Skill")]
        public Image qSkill;

        [Tooltip("text for Q skill")]
        public TextMeshProUGUI Qtext;

        [Header("Weapon Information")]
        [Tooltip("Weapon 1 name")]
        public TextMeshProUGUI Weapon1Name;

        [Tooltip("text for Descriping weapon 1")]
        public TextMeshProUGUI txtWeapon1;

        [Tooltip("Weapon 2 name")]
        public TextMeshProUGUI Weapon2Name;

        [Tooltip("text for Descriping weapon 2")]
        public TextMeshProUGUI txtWeapon2;

        [Header("Payer Base Priority")]
        [Tooltip("Endurance value")]
        public TextMeshProUGUI endurance;

        [Tooltip("Power value")]
        public TextMeshProUGUI power;

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
        public TextMeshProUGUI recouvery;

        [Tooltip("Max Hp")]
        public TextMeshProUGUI MaxHP;

        [Tooltip("Current Hp")]
        public TextMeshProUGUI CurrentHP;

        [Tooltip("Buffer details")]
        public GameObject Buff;

        PlayerWeaponsManager playerWeaponsManager;
        PlayerSkillsManager playerSkillsManager;
        ActorProperties actorProperties;
        ActorAttribute actorAttribute;
        Health health;
        // Start is called before the first frame update
        void Start()
        {
            GameObject player = GameObject.Find("Player1");

            DebugUtility.HandleErrorIfNullFindObject<GameObject, Backpack>(player, this);
            playerSkillsManager = player.GetComponent<PlayerSkillsManager>();
            playerWeaponsManager = player.GetComponent<PlayerWeaponsManager>();
            actorProperties = player.GetComponent<ActorProperties>();
            actorAttribute = player.GetComponent<ActorAttribute>();
            health = player.GetComponent<Health>();
           

        }

        // Update is called once per frame
        void OnEnable()
        {
            Weapon1Name.text = playerWeaponsManager.m_WeaponSlots[0].name;
            Weapon2Name.text = playerWeaponsManager.m_WeaponSlots[1].name;
            txtWeapon1.text = playerWeaponsManager.m_WeaponSlots[0].description;
            txtWeapon2.text = playerWeaponsManager.m_WeaponSlots[1].description;

            endurance.text = actorAttribute.stamina.ToString();
            power.text = actorAttribute.strength.ToString();
            intelligence.text = actorAttribute.intelligence.ToString();
            technique.text = actorAttribute.technique.ToString();

            physicalDamage.text = actorProperties.getPhysicalAttack().ToString();
            magicalDamage.text = actorProperties.getMagicAttack().ToString();
            physicalDefence.text = actorProperties.getPhysicalDefence().ToString();
            magicalDefence.text = actorProperties.getMagicDefence().ToString();
            MaxHP.text = health.MaxHealth.ToString();
            CurrentHP.text = health.CurrentHealth.ToString();



        }
    }
}
