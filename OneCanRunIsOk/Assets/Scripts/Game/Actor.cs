using System.Collections.Generic;
using System.Xml;
using UnityEngine;

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
        // init it as true as to calculate exposedProperty
        private bool dirty = true;
        private bool calculating = false; // for ipc semaphore

        private void tryCalculate(){
            if (calculating){
                while (calculating) ;
                return;
            }

            calculate();

        }


        // we should consider the IPC in case two calculate function in run time
        private void calculate(){
            calculating = true;
            if((ulong)this.mModifier[0].baseValue >= getNextLevelCount()){
                levelUpdate();
            }
            // core function 

            // left blank
            // @ to do
            exposedProperty = baseProperty;

            // core function 

            calculating = false;

        }

        private void levelUpdate(){

            //@ to do
        }

        private ulong getNextLevelCount(){
            return 500;
        }


        private void OnEnable()
        {
            mModifier = new List<Share.Modifier>();
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

            OneCanRun.Game.Share.Modifier modifier = new Share.Modifier(0, Share.Modifier.ModifierType.experience, this);
            mModifier.Add(modifier);
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

        void Start()
        {
            m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);
            // Register as an actor
            if (!m_ActorsManager.Actors.Contains(this))
            {
                m_ActorsManager.Actors.Add(this);
            }
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
            if (!dirty)
                return this.exposedProperty;

            // we should only allow the function in single instance only to be excuted once at the same time 
            tryCalculate();

            return exposedProperty;
        }

        public void addModifier(OneCanRun.Game.Share.Modifier mod){

            // experience just linear overwrite
            if(mod.type == Share.Modifier.ModifierType.experience){
                this.mModifier[0].baseValue += mod.baseValue;
                if (this.mModifier[0].baseValue >= getNextLevelCount())
                    dirty = true;
                return;
            }


            this.mModifier.Add(mod);
        }

        public bool removeModifier(OneCanRun.Game.Share.Modifier mod){

            return this.mModifier.Remove(mod);

        }

        
        public bool removeModifierFromSource(Object obj){
            bool deleted = false;
            for(int i = mModifier.Count - 1;i >= 0; i--){

                if(mModifier[i].source == obj){
                    dirty = true;
                    deleted = true;
                    mModifier.RemoveAt(i);
                }

            }

            return deleted;
        }

    }
}
