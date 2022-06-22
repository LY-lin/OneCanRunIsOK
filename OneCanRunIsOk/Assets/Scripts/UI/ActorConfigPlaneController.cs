using OneCanRun.Game.Share;
using UnityEngine;
using OneCanRun.GamePlay;
using OneCanRun.Game;
namespace OneCanRun.UI{

    public class ActorConfigPlaneController : MonoBehaviour{
        private OneCanRun.Game.Share.ActorConfig config;

        // text object for updating value
        TMPro.TextMeshProUGUI staminaValue;
        TMPro.TextMeshProUGUI strengthValue;
        TMPro.TextMeshProUGUI intelligeValue;
        TMPro.TextMeshProUGUI techniqueValue;
        TMPro.TextMeshProUGUI staminaAllocatedValue;
        TMPro.TextMeshProUGUI strengthAllocatedValue;
        TMPro.TextMeshProUGUI intelligeAllocatedValue;
        TMPro.TextMeshProUGUI techniqueAllocatedValue;
        TMPro.TextMeshProUGUI pointLeft;
        TMPro.TextMeshProUGUI recoveryValue;
        TMPro.TextMeshProUGUI speedValue;
        TMPro.TextMeshProUGUI magicDefenceValue;
        TMPro.TextMeshProUGUI phyDefenceValue;
        TMPro.TextMeshProUGUI magicDamageValue;
        TMPro.TextMeshProUGUI phyDamageValue;
        TMPro.TextMeshProUGUI campLabel;
        UnityEngine.UI.Image weaponImage;
        TMPro.TextMeshProUGUI weaponDescription;
        TMPro.TextMeshProUGUI weaponName;

        TMPro.TextMeshProUGUI QName;
        TMPro.TextMeshProUGUI QDescription;
        UnityEngine.UI.Image QImage;
        TMPro.TextMeshProUGUI FName;
        TMPro.TextMeshProUGUI FDescription;
        UnityEngine.UI.Image FImage;

        // for decreasing update 
        private bool dirty = true;
        private void Start()
        {
            updateCampInfo();
        }
        private void OnEnable(){
            staminaValue = GameObject.Find("staminaValue").GetComponent<TMPro.TextMeshProUGUI>();
            strengthValue = GameObject.Find("strengthValue").GetComponent<TMPro.TextMeshProUGUI>();
            intelligeValue = GameObject.Find("intelligeValue").GetComponent<TMPro.TextMeshProUGUI>();
            techniqueValue = GameObject.Find("techniqueValue").GetComponent<TMPro.TextMeshProUGUI>();
            staminaAllocatedValue = GameObject.Find("staminaAllocatedValue").GetComponent<TMPro.TextMeshProUGUI>();
            strengthAllocatedValue = GameObject.Find("strengthAllocatedValue").GetComponent<TMPro.TextMeshProUGUI>();
            intelligeAllocatedValue = GameObject.Find("intelligeAllocatedValue").GetComponent<TMPro.TextMeshProUGUI>();
            techniqueAllocatedValue = GameObject.Find("techniqueAllocatedValue").GetComponent<TMPro.TextMeshProUGUI>();
            pointLeft = GameObject.Find("PointLeft").GetComponent<TMPro.TextMeshProUGUI>();
            recoveryValue = GameObject.Find("recoveryValue").GetComponent<TMPro.TextMeshProUGUI>();
            speedValue = GameObject.Find("speedValue").GetComponent<TMPro.TextMeshProUGUI>();
            magicDefenceValue = GameObject.Find("magicDefenceValue").GetComponent<TMPro.TextMeshProUGUI>();
            phyDefenceValue = GameObject.Find("phyDefenceValue").GetComponent<TMPro.TextMeshProUGUI>();
            magicDamageValue = GameObject.Find("magicDamageValue").GetComponent<TMPro.TextMeshProUGUI>();
            phyDamageValue = GameObject.Find("phyDamageValue").GetComponent<TMPro.TextMeshProUGUI>();


            // camp info are as follows
            campLabel = GameObject.Find("campLabel").GetComponent<TMPro.TextMeshProUGUI>();
            weaponDescription = GameObject.Find("weaponDescription").GetComponent<TMPro.TextMeshProUGUI>();
            weaponName = GameObject.Find("weaponName").GetComponent<TMPro.TextMeshProUGUI>();
            weaponImage = GameObject.Find("weaponImage").GetComponent<UnityEngine.UI.Image>();

            QImage = GameObject.Find("QImage").GetComponent<UnityEngine.UI.Image>();
            QName = GameObject.Find("QName").GetComponent<TMPro.TextMeshProUGUI>();
            QDescription = GameObject.Find("QDescription").GetComponent<TMPro.TextMeshProUGUI>();

            FImage = GameObject.Find("FImage").GetComponent<UnityEngine.UI.Image>();
            FName = GameObject.Find("FName").GetComponent<TMPro.TextMeshProUGUI>();
            FDescription = GameObject.Find("FDescription").GetComponent<TMPro.TextMeshProUGUI>();
            
            config = new Game.Share.ActorConfig();
        }

        private void mValueUpdate(){
            ActorAttribute actorAttribute = config.getActorAttribute();
            staminaValue.text = (actorAttribute.stamina).ToString();
            strengthValue.text = (actorAttribute.strength).ToString();
            intelligeValue.text = (actorAttribute.intelligence).ToString();
            techniqueValue.text = (actorAttribute.technique).ToString();
            staminaAllocatedValue.text = config.getAllocatedStamina().ToString();
            strengthAllocatedValue.text = config.getAllocatedStrength().ToString();
            intelligeAllocatedValue.text = config.getAllocatedIntelligence().ToString();
            techniqueAllocatedValue.text = config.getAllocatedTechnique().ToString();
            pointLeft.text = config.getPoint2Allocate().ToString();

            OneCanRun.Game.Share.ActorProperties properties = config.GetActorProperties();
            recoveryValue.text = properties.getHealRate().ToString();
            speedValue.text = properties.getMaxSpeed().ToString();
            magicDefenceValue.text = properties.getMagicDefence().ToString();
            phyDefenceValue.text = properties.getPhysicalDefence().ToString();
            magicDamageValue.text = properties.getMagicAttack().ToString();
            phyDamageValue.text = properties.getPhysicalAttack().ToString();

            dirty = false;
        }

        private void Update(){

            if (dirty)
                mValueUpdate();
        }


        // just fill the following 3 function if the camp changes
        private void useAzeymaCampInfo(){
            config.setBaseAttribute(5, 7, 2, 6);
            weaponDescription.text = "A staff carved out of rotten wood.";
            weaponName.text = "Stuffy Wand";
            //weaponImage.sprite = gameObject.GetComponentInChildren<WeaponController>().WeaponImg;

            QImage.sprite = Resources.Load<Sprite>("Skills/FireRain");
            QName.text = "FireRain";
            QDescription.text = "A massive rain of fire from the sky.";

            
            FImage.sprite = Resources.Load<Sprite>("Skills/Cast"); ;
            FName.text = "BlitzBall";
            FDescription.text = "Deals a small area of magic damage";
        }

        private void useHaloneCampInfo(){
            weaponDescription.text = "A spear that glows, but has no special magic";
            weaponName.text = "Shining Spear";
            //weaponImage.sprite = gameObject.GetComponentInChildren<WeaponController>().WeaponImg;

            QImage.sprite = Resources.Load<Sprite>("Skills/tornado");
            QName.text = "Tonado";
            QDescription.text = "Create a tornado that attracts monsters";


            FImage.sprite = Resources.Load<Sprite>("Skills/Buff");
            FName.text = "Holy Help";
            FDescription.text = "Pray for strength and healing";
            config.setBaseAttribute(1, 10, 5, 8);
        }

        private void useByregotCampInfo(){
            weaponDescription.text = "Cheap, leathery, old and capable of firing";
            weaponName.text = "AR1";
            //weaponImage.sprite = gameObject.GetComponentInChildren<WeaponController>().WeaponImg;

            QImage.sprite = Resources.Load<Sprite>("Skills/Flash");
            QName.text = "Flash";
            QDescription.text = "Stop nearby enemies at time and teleport to the target location";


            //FImage.sprite = gameObject.GetComponent<SkillController>().SkillIcon;
            FName.text = "Unfinish";
            FDescription.text = "Unfinish";
            config.setBaseAttribute(2, 6, 2, 6);
        }


        private void updateCampInfo(){
            // change weapon and skill info
            switch (this.config.getCampType()){
                case Game.Share.ActorConfig.CampType.Azeyma:
                    useAzeymaCampInfo();
                    break;
                case Game.Share.ActorConfig.CampType.Byregot:
                    useByregotCampInfo();
                    break;
                case Game.Share.ActorConfig.CampType.Halone:
                    useHaloneCampInfo();
                    break;
                default:
                    break;

            }
            

        }

        // click event
        public void addStamina(){
            if (config.addStamina())
                dirty = true;
        }

        public void decreaseStamina(){

            if(config.decreaseStamina())
                dirty = true; 
        }

        public void addStrength(){
            if(config.addStrength())
                dirty = true;
        }

        public void decreaseStrength(){
            if(config.decreaseStrength())
                dirty = true;
        }

        public void addIntelligence(){
            if (config.addIntelligence())
                dirty = true;
        }

        public void decreaseIntelligence(){
            if (config.decreaseIntelligence())
                dirty = true;
        }

        public void addTechnique(){
            if (config.addTechnique())
                dirty = true;
        }

        public void decreaseTechnique(){
            if (config.decreaseTechnique())
                dirty = true;
        }

        public void confirm(){
            // save the config
            Game.Share.ActorConfig.saveAsFile("ActorConfig.cfg", this.config);
            // change the scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
        }


        public void changeCamp(){

            switch (this.campLabel.text){

                case "Azeyma":
                    config.setCampType(Game.Share.ActorConfig.CampType.Azeyma);
                    break;
                case "Halone":
                    config.setCampType(Game.Share.ActorConfig.CampType.Halone);
                    break;
                case "Byregot":
                    config.setCampType(Game.Share.ActorConfig.CampType.Byregot);
                    break;
                default:
                    break;
            }

            this.updateCampInfo();
            mValueUpdate();

        }

        public void backButton(){
            UnityEngine.SceneManagement.SceneManager.LoadScene("MyStartScene");

        }


    }
}
