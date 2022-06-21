using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun{

    [System.Serializable]
    public class ActorAttribute{
        public int strength;
        public int intelligence;
        public int technique;
        public int stamina;
        public ActorAttribute(){
            strength = 0;
            intelligence = 0;
            technique = 0;
            stamina = 0;

        }

    }
}
