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
        //private List<OneCanRun.Game.Share.Modifier> mModifier;
        public ActorBuffManager buffManager;


        public void Awake()
        {
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

        // we should consider the IPC in case two calculate function in run time
        private void calculate(){
            /*if(this.mModifier[0].baseValue >= getNextLevelCount()){
                levelUpdate();
            }*/
            // core function 

            // left blank
            // @ to do
            ResetExposedProperty();
            foreach (BuffController b in buffManager.NumBuffList)
            {
                Debug.Log("exposedProperty " + exposedProperty.getMagicAttack());
                b.ActorbuffAct(ref exposedProperty);
                Debug.Log("After exposedProperty " + exposedProperty.getMagicAttack());
            }

            // core function 


        }
        private void ResetExposedProperty()
        {
            exposedProperty.setHealRate(baseProperty.getHealRate());
            exposedProperty.setMagicAttack(baseProperty.getMagicAttack());
            exposedProperty.setMagicDefence(baseProperty.getMagicDefence());
            exposedProperty.setPhysicalAttack(baseProperty.getPhysicalAttack());
            exposedProperty.setPhysicalDefence(baseProperty.getPhysicalDefence());
            exposedProperty.setMaxJump(baseProperty.getMaxJump());
            exposedProperty.setMaxSpeed(baseProperty.getMaxSpeed());

        }

        private void levelUpdate(){

            //@ to do
        }

        private ulong getNextLevelCount(){
            return 500;
        }


        private void OnEnable()
        {
            //mModifier = new List<Share.Modifier>();
            string fileName = "defaultProperties";
            string configDirectory = System.IO.Directory.GetCurrentDirectory();
            configDirectory += "\\Config\\";
            switch (Affiliation)
            {
                case affiliationType.allies:
                    fileName += ".allies.xml";
                    break;
                case affiliationType.enemy:
                    fileName += ".enemy.xml";
                    break;
                case affiliationType.neutral:
                    fileName += ".neutral.xml";
                    break;
                default:
                    break;
            }

            //OneCanRun.Game.Share.Modifier modifier = new Share.Modifier(0, Share.Modifier.ModifierType.experience, this);
            //mModifier.Add(modifier);
            baseProperty = new OneCanRun.Game.Share.ActorProperties();
            exposedProperty = new OneCanRun.Game.Share.ActorProperties();

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

        ActorProperties m_BaseProperties;

        ActorProperties m_PresentProperties;

        public ActorProperties getBaseProperties()
        {
            return m_BaseProperties;
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




    }
}
