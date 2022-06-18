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
        private bool dirty = true;
        public bool isPlayer = false;


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

        private void levelUpdate(){
            if (!this.isPlayer){

                return;
            }
            // update level up data, including level up seriesly
            // reset experice
            this.actorAttribute.point2allocate += 5;
            int strenthEnhanced = 1;
            int techniqueEnhanced = 1;
            int intelligenceEnhanced = 1;
            this.mModifier[0].baseValue = 0;

            //update base

            // strenthEnhanced
            baseProperty.setMaxHealth(baseProperty.getMaxHealth() + strenthEnhanced *2);
            baseProperty.setPhysicalDefence(baseProperty.getPhysicalDefence() + strenthEnhanced);
            baseProperty.setMagicDefence(baseProperty.getMagicDefence() + strenthEnhanced);
            baseProperty.setPhysicalAttack(baseProperty.getPhysicalAttack() + strenthEnhanced * 3);
            // intelligence
            baseProperty.setMagicAttack(baseProperty.getMagicAttack() + intelligenceEnhanced);
            // technique
            baseProperty.setPhysicalAttack(baseProperty.getPhysicalAttack() + techniqueEnhanced);
            baseProperty.setMaxSpeed(baseProperty.getMaxSpeed() * (1 + 0.1f * techniqueEnhanced));


            Debug.Log("level up");

        }

        public ulong getNextLevelCount(){
            return 500;
        }


        private void OnEnable()
        {
            //mModifier = new List<Share.Modifier>();
            string fileName = "defaultProperties";
            string configDirectory = System.IO.Directory.GetCurrentDirectory();
            configDirectory += "\\Config\\";
            Renderer[] renderers;
            switch (Affiliation)
            {
                case affiliationType.allies:
                    fileName += ".allies.xml";
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

            

            //OneCanRun.Game.Share.Modifier modifier = new Share.Modifier(0, Share.Modifier.ModifierType.experience, this);
            //mModifier.Add(modifier);
            

            // property from file 
            XmlDocument xml = new XmlDocument();
            xml.Load(configDirectory + fileName);

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

    }
}
