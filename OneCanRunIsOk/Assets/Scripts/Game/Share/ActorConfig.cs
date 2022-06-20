using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share{
   
    public class ActorConfig{
        // for search index
        static class ActorConfigConstant
        {
            public const int stamina = 0;
            public const int strength = 1;
            public const int intelligence = 2;
            public const int technique = 3;
        }

        private ActorAttribute defaultAttribute;
        private ActorProperties defaultProperties;
        private ulong point2Allocate = 10;
        private bool dirty = true;
        /*  0 -> stamina
            1 -> strength
            2 -> intelligence
            3 -> technique
        */
        private int[] pointAllocated;

        public ActorConfig(){
            pointAllocated = new int[4];
            defaultAttribute = new ActorAttribute();
            defaultProperties = new ActorProperties();
            // just for programming convenience
            pointAllocated[ActorConfigConstant.stamina] = 0;
            pointAllocated[ActorConfigConstant.intelligence] = 0;
            pointAllocated[ActorConfigConstant.strength] = 0;
            pointAllocated[ActorConfigConstant.technique] = 0;

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

    }
}
