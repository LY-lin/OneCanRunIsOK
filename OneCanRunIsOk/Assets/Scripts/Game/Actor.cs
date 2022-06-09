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
        private bool dirty;
        private OneCanRun.Game.Share.ActorProperties baseProperty;
        private OneCanRun.Game.Share.ActorProperties exposedProperty;
        private List<OneCanRun.Game.Share.Modifier> mModifier;

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
            baseProperty = new OneCanRun.Game.Share.ActorProperties();
            exposedProperty = new OneCanRun.Game.Share.ActorProperties();

            // property from file 
            XmlDocument xml = new XmlDocument();
            xml.Load(configDirectory + fileName);

            XmlNodeList xmlNodeList = xml.SelectSingleNode("PropertyConfig").ChildNodes;

            foreach (XmlElement child in xmlNodeList)
            {
                if (child.GetAttribute("exp") == "")
                {
                    baseProperty.setEXP(ulong.Parse(child.InnerText));
                }
                else if (child.GetAttribute("maxHealth") == "")
                {
                    baseProperty.setMaxHealth(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("healRate") == "")
                {
                    baseProperty.setHealRate(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("physicalAttack") == "")
                {
                    baseProperty.setPhysicalAttack(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("magicAttack") == "")
                {
                    baseProperty.setMagicAttack(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("physicalDefence") == "")
                {
                    baseProperty.setPhysicalDefence(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("magicDefence") == "")
                {
                    baseProperty.setMagicDefence(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("maxSpeed") == "")
                {
                    baseProperty.setMaxSpeed(float.Parse(child.InnerText));
                }
                else if (child.GetAttribute("maxJump") == "")
                {
                    baseProperty.setMaxJump(float.Parse(child.InnerText));
                }
                else
                {
                    throw new System.Exception();

                }
            }    
        }

        void Start()
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

        public ActorBuffManager buffManager;


    }
}
