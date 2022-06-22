using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share{
   [System.Serializable]
    public class ActorConfig{
        // for search index
        [System.Serializable]
        static class ActorConfigConstant
        {
            public const int stamina = 0;
            public const int strength = 1;
            public const int intelligence = 2;
            public const int technique = 3;
        }

        public enum CampType{
            Azeyma = 0,
            Halone = 1,
            Byregot = 2
        }
        private CampType campType;

        // base value for different camp
        private ActorAttribute baseAttribute;
        private ActorAttribute defaultAttribute;

        // basevalue for different camp
        private ActorProperties baseProperties;
        private ActorProperties defaultProperties;
        private ulong point2Allocate = 10;
        private bool dirty = true;
        /*  
            0 -> stamina
            1 -> strength
            2 -> intelligence
            3 -> technique
        */
        private int[] pointAllocated;

        public ActorConfig(){
            // initialization
            pointAllocated = new int[4];
            defaultAttribute = new ActorAttribute();
            baseAttribute = new ActorAttribute();
            baseProperties = new ActorProperties();
            defaultProperties = new ActorProperties();
            this.campType = CampType.Azeyma;


            //@ to do read base value

            // just for programming convenience
            pointAllocated[ActorConfigConstant.stamina] = 0;
            pointAllocated[ActorConfigConstant.intelligence] = 0;
            pointAllocated[ActorConfigConstant.strength] = 0;
            pointAllocated[ActorConfigConstant.technique] = 0;

        }

        // interface exposed to outside
        public void calculateImmediately(){
            if(this.baseAttribute.technique + pointAllocated[ActorConfigConstant.technique] > 10){
                int leftValue = baseAttribute.technique + pointAllocated[ActorConfigConstant.technique] - 10;
                pointAllocated[ActorConfigConstant.technique] -= leftValue;
                point2Allocate = (ulong)((int)point2Allocate + leftValue);

            }

            calculate();

        }

        public bool addStamina(){

            if (!sentence(true))
                return false;
            pointAllocated[ActorConfigConstant.stamina]++;
            point2Allocate--;
            dirty = true;
            update();
            return true;
        }

        public bool decreaseStamina(){

            // illeg?
            if (!sentence(false))
                return false;
            if (pointAllocated[ActorConfigConstant.stamina] <= 0)
                return false;

            pointAllocated[ActorConfigConstant.stamina]--;
            point2Allocate++;
            dirty = true;
            update();
            return true;

        }


        public bool addIntelligence(){

            if (!sentence(true))
                return false;
            pointAllocated[ActorConfigConstant.intelligence]++;
            point2Allocate--;
            dirty = true;
            update();
            return true;
        }

        public bool decreaseIntelligence(){

            // illeg?
            if (!sentence(false))
                return false;
            if (pointAllocated[ActorConfigConstant.intelligence] <= 0)
                return false;

            pointAllocated[ActorConfigConstant.intelligence]--;
            point2Allocate++;
            dirty = true;
            update();
            return true;

        }


        public bool addStrength(){

            if (!sentence(true))
                return false;
            pointAllocated[ActorConfigConstant.strength]++;
            point2Allocate--;
            dirty = true;
            update();
            return true;
        }

        public bool decreaseStrength(){

            // illeg?
            if (!sentence(false))
                return false;
            if (pointAllocated[ActorConfigConstant.strength] <= 0)
                return false;

            pointAllocated[ActorConfigConstant.strength]--;
            point2Allocate++;
            dirty = true;
            update();
            return true;

        }

        public bool addTechnique(){

            if (!sentence(true))
                return false;
            if (this.defaultAttribute.technique >= 10)
                return false;
            pointAllocated[ActorConfigConstant.technique]++;
            point2Allocate--;
            dirty = true;
            update();
            return true;
        }

        public bool decreaseTechnique(){

            // illeg?
            if (!sentence(false))
                return false;
            if (pointAllocated[ActorConfigConstant.technique] <= 0)
                return false;

            pointAllocated[ActorConfigConstant.technique]--;
            point2Allocate++;
            dirty = true;
            update();
            return true;

        }

        public void setCampType(CampType _campType){
            this.campType = _campType;

        }

        public CampType getCampType(){
            return this.campType;

        }


        // judge the action is feasible
        private bool sentence(bool add){
            if (add){
                if (point2Allocate <= 0)
                    return false;
                else
                    return true;

            }
            else{
                // we should consider whether to be minus.
                return true;
            }

        }

        // calculate properties if need
        private void update(){
            if (dirty)
                calculate();
        }

        // calculate core function
        private void calculate(){

            // calculate attribute
            defaultAttribute.intelligence = baseAttribute.intelligence + pointAllocated[ActorConfigConstant.intelligence];
            defaultAttribute.stamina = baseAttribute.stamina + pointAllocated[ActorConfigConstant.stamina];
            defaultAttribute.strength = baseAttribute.strength + pointAllocated[ActorConfigConstant.strength];
            defaultAttribute.technique = baseAttribute.technique + pointAllocated[ActorConfigConstant.technique];

            // calculate property via function
            this.defaultProperties.setMaxHealth(this.baseProperties.getMaxHealth() + (this.baseAttribute.stamina + getAllocatedStamina()) * 10);
            this.defaultProperties.setPhysicalAttack(this.baseProperties.getPhysicalAttack() + (this.baseAttribute.strength +  getAllocatedStrength()) * 3);
            this.defaultProperties.setMagicAttack(this.baseProperties.getMagicAttack() + (this.baseAttribute.intelligence + getAllocatedIntelligence()) * 10);
            this.defaultProperties.setPhysicalDefence(this.baseProperties.getPhysicalDefence() + (this.baseAttribute.stamina + getAllocatedStamina()) * 1);
            this.defaultProperties.setMagicDefence(this.baseProperties.getMagicDefence() + (this.baseAttribute.stamina + getAllocatedStamina()) * 1);
            this.defaultProperties.setMaxSpeed((float)(this.baseProperties.getMaxSpeed() + (this.baseAttribute.technique +  getAllocatedTechnique()) * 0.5));
            
            dirty = false;
        }

        // infornmation interface as follow
        public ulong getPoint2Allocate(){

            return point2Allocate;
        }

        public ActorAttribute getActorAttribute(){

            return defaultAttribute;
        }


        public ActorProperties GetActorProperties(){

            return defaultProperties;
        }


        public int getAllocatedStrength(){

            return this.pointAllocated[ActorConfigConstant.strength];
        }

        public int getAllocatedIntelligence(){

            return this.pointAllocated[ActorConfigConstant.intelligence];
        }

        public int getAllocatedStamina(){

            return this.pointAllocated[ActorConfigConstant.stamina];
        }

        public int getAllocatedTechnique(){

            return this.pointAllocated[ActorConfigConstant.technique];
        }

        public void setBaseAttribute(int _intelligence, int _stamina, int  _strength, int _technique){
            baseAttribute.intelligence = _intelligence;
            baseAttribute.stamina = _stamina;
            baseAttribute.strength = _strength;
            baseAttribute.technique = _technique;

        }

        // for next scene to create actor
        public static void saveAsFile(string name, ActorConfig actorConfig){
            string path;
            path = System.IO.Directory.GetCurrentDirectory();
            path += "\\Config\\";
            path += name;
            System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = 
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            binaryFormatter.Serialize(fileStream, actorConfig);
            fileStream.Close();

        }

        // read file to config actor attribute
        public static ActorConfig readFile(string name){
            string path;
            path = System.IO.Directory.GetCurrentDirectory();
            path += "\\Config\\";
            path += name;
            System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = 
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            ActorConfig config = binaryFormatter.Deserialize(fileStream) as ActorConfig;

            return config;
        }

    }
}
