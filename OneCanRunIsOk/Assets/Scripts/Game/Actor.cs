using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.Game
{
    public enum affiliationType
    {
        allies,
        neutral,
        enemy
    }
    // This class contains general information describing an actor (player or enemies).
    // It is mostly used for AI detection logic and determining if an actor is friend or foe
    public class Actor : MonoBehaviour
    {
        [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to each other")]
        public affiliationType Affiliation;

        [Tooltip("Represents point where other actors will aim when they attack this actor")]
        public Transform AimPoint;

        ActorsManager m_ActorsManager;
        private OneCanRun.Game.Share.ActorProperties baseProperty;
        private OneCanRun.Game.Share.ActorProperties exposedProperty;
        private List<OneCanRun.Game.Share.Modifier> mModifier;
        public ActorAttribute actorAttribute{get;private set;}
        public ActorBuffManager buffManager;
        public ActorConfig.CampType campType;
        private static XmlDocument alliesXml = null;
        private static XmlDocument enemyXml = null;
        private static XmlDocument neutralXml = null;
        public bool isPlayer = false;
        private uint level = 1;

        public uint getLevel(){
            return level;
        }

        public void Awake()
        {
            actorAttribute = new ActorAttribute();
            mModifier = new List<Modifier>();
            OneCanRun.Game.Share.Modifier modifier = new Share.Modifier(0, Share.Modifier.ModifierType.experience, this);
            mModifier.Add(modifier);
            baseProperty = new OneCanRun.Game.Share.ActorProperties();
            exposedProperty = new OneCanRun.Game.Share.ActorProperties();
            m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

            buffManager = GetComponent<ActorBuffManager>();
            buffManager.buffChanged += calculate;
            // Register as an actor
            if (!m_ActorsManager.Actors.Contains(this))
            {
                m_ActorsManager.Actors.Add(this);
            }
        }
        private void tryCalculate(){

            calculate();

        }

        
        private void calculate(){


            if(this.mModifier[0].baseValue >= getNextLevelCount()){
                levelUpdate();
            }


            // core function 

            // left blank
            // @ to do
            ResetExposedProperty();
                //if(buffManager.NumBuffList.Count>0)
            foreach (BuffController b in buffManager.NumBuffList)
            {
                //Debug.Log(this +" exposedProperty " + exposedProperty.getMagicAttack());
                b.ActorbuffAct(ref exposedProperty);
                //Debug.Log(this + " After exposedProperty " + exposedProperty.getMagicAttack());
            }
            foreach (BuffController b in buffManager.PercentBuffList)
            {
                //Debug.Log(this + " exposedProperty " + exposedProperty.getMagicAttack());
                b.ActorbuffAct(ref exposedProperty);
                //Debug.Log(this + " After exposedProperty " + exposedProperty.getMagicAttack());
            }
            // core function 


        }
        private void ResetExposedProperty()
        {
            exposedProperty.setMaxHealth(baseProperty.getMaxHealth());
            exposedProperty.setHealRate(baseProperty.getHealRate());
            exposedProperty.setMagicAttack(baseProperty.getMagicAttack());
            exposedProperty.setMagicDefence(baseProperty.getMagicDefence());
            exposedProperty.setPhysicalAttack(baseProperty.getPhysicalAttack());
            exposedProperty.setPhysicalDefence(baseProperty.getPhysicalDefence());
            exposedProperty.setMaxJump(baseProperty.getMaxJump());
            exposedProperty.setMaxSpeed(baseProperty.getMaxSpeed());

        }

        // in case up more than one level
        private void levelOneUp(){
            int strenthEnhanced = 1;
            int staminaEnhanced = 1;
            int intelligenceEnhanced = 1;
            this.actorAttribute.intelligence += intelligenceEnhanced;
            this.actorAttribute.stamina += staminaEnhanced;
            this.actorAttribute.strength += strenthEnhanced;

            //update base

            // strenthEnhanced
            baseProperty.setMaxHealth(baseProperty.getMaxHealth() + strenthEnhanced *2);
            baseProperty.setPhysicalDefence(baseProperty.getPhysicalDefence() + strenthEnhanced);
            baseProperty.setMagicDefence(baseProperty.getMagicDefence() + strenthEnhanced);
            baseProperty.setPhysicalAttack(baseProperty.getPhysicalAttack() + strenthEnhanced * 3);
            // intelligence
            baseProperty.setMagicAttack(baseProperty.getMagicAttack() + intelligenceEnhanced);
            // technique
            baseProperty.setPhysicalAttack(baseProperty.getPhysicalAttack() + staminaEnhanced);


        }

        private void levelUpdate(){
            if (!this.isPlayer){
                return;
            }
            int counter = 0;
            while(this.mModifier[0].baseValue >= getNextLevelCount()){
                this.mModifier[0].baseValue -= getNextLevelCount();
                level++;
                counter++;
            }

            for (int i = 0; i < counter; i++)
                levelOneUp();
            

        }

        public ulong getNextLevelCount(){

            
            return 500 + (ulong)(level * (ulong)Mathf.Log(level)) + (ulong)Mathf.Exp(level);
        }


        private void OnEnable()
        {
            //mModifier = new List<Share.Modifier>();
            string fileName = "defaultProperties";
            string configDirectory = System.IO.Directory.GetCurrentDirectory();
            configDirectory += "\\Config\\";
            // property from file 
            XmlDocument xml = null;
            Renderer[] renderers;
            switch (Affiliation)
            {
                case affiliationType.allies:
                    fileName += ".allies.xml";
                    if(alliesXml == null){
                        alliesXml = new XmlDocument();
                        alliesXml.Load(configDirectory + fileName);
                    }
                    xml = alliesXml;
                    renderers = this.gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer.gameObject.layer == LayerMask.NameToLayer("MiniMap"))
                        {
                            renderer.material.color = new Color(255, 255, 255);
                            break;
                        }
                    }
                    break;
                case affiliationType.enemy:
                    fileName += ".enemy.xml";
                    if(enemyXml == null){
                        enemyXml = new XmlDocument();
                        enemyXml.Load(configDirectory + fileName);
                    }
                    xml = enemyXml;
                    renderers = this.gameObject.GetComponentsInChildren<Renderer>();
                    foreach(Renderer renderer in renderers)
                    {
                        if(renderer.gameObject.layer == LayerMask.NameToLayer("MiniMap"))
                        {
                            renderer.material.color = new Color(255, 0, 0);
                            break;
                        }
                    }

                    break;
                case affiliationType.neutral:
                    fileName += ".neutral.xml";
                    if(neutralXml == null){
                        neutralXml = new XmlDocument();
                        neutralXml.Load(configDirectory + fileName);
                    }
                    xml = enemyXml;
                    renderers = this.gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer.gameObject.layer == LayerMask.NameToLayer("MiniMap"))
                        {
                            renderer.material.color = new Color(255, 255, 0);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            

            
           
            XmlNodeList xmlNodeList = xml.SelectSingleNode("PropertyConfig").ChildNodes;

            foreach (XmlElement child in xmlNodeList)
            {
                //XmlNodeList a = child.GetElementsByTagName("EXP");
                //string a = child.GetAttribute("exp");
                //string a = child.Name;
                if (child.Name == "EXP")
                {
                    baseProperty.setEXP(ulong.Parse(child.InnerText));
                }
                if (child.Name == "MAXHEALTH")
                {
                    baseProperty.setMaxHealth(float.Parse(child.InnerText));
                }
                if (child.Name == "HEALRATE")
                {
                    baseProperty.setHealRate(float.Parse(child.InnerText));
                }
                if (child.Name == "PHYSICALATTACK")
                {
                    baseProperty.setPhysicalAttack(float.Parse(child.InnerText));
                }
                if (child.Name == "MAGICATTACK")
                {
                    baseProperty.setMagicAttack(float.Parse(child.InnerText));
                }
                if (child.Name == "PHYSICALDEFENCE")
                {
                    baseProperty.setPhysicalDefence(float.Parse(child.InnerText));
                }
                if (child.Name == "MAGICDEFENCE")
                {
                    baseProperty.setMagicDefence(float.Parse(child.InnerText));
                }
                if (child.Name == "MAXSPEED")
                {
                    baseProperty.setMaxSpeed(float.Parse(child.InnerText));
                }
                if (child.Name == "MAXJUMP")
                {
                    baseProperty.setMaxJump(float.Parse(child.InnerText));
                }
                
            }    

            if(gameObject.name == "Player1"){
                // read player actor config
                Share.ActorConfig actorConfig = Share.ActorConfig.readFile("ActorConfig.cfg");
                this.campType = actorConfig.getCampType();
                this.actorAttribute = actorConfig.getActorAttribute();
                this.baseProperty = actorConfig.GetActorProperties();

            }

        }


        public ActorProperties getBaseProperties()
        {
            return baseProperty;
        }

        void Start()
        {
            
        }

        void OnDestroy()
        {
            // Unregister as an actor
            if (m_ActorsManager)
            {
                m_ActorsManager.Actors.Remove(this);
            }
        }


        // public interface are as follows


        public OneCanRun.Game.Share.ActorProperties GetActorProperties()
        {
            tryCalculate();
            return exposedProperty;
        }

        public void addModifier(Modifier mod){
            if (mod.type != Modifier.ModifierType.experience)
                throw new System.Exception();

            mModifier[0].baseValue += mod.baseValue;
        }

        public float getExperience()
        {
            return this.mModifier[0].baseValue;
        }

        public void reset(){
            if (this.gameObject.name == "Player1")
                return;

            this.Awake();
            this.OnEnable();
            level = 1;
        }

        public void setLevel(uint targetlevel){
            for(uint i = level;i < targetlevel; i++){
                levelOneUp();
            }

        }

    }
}
