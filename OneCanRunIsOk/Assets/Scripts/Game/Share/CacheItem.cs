using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share{

    public class CacheItem : MonoBehaviour{

        // you should overwrite the function
        // initialize the parameter you need
        public virtual void init(){
            throw new System.Exception("init function not implement in " + this.ToString());
        }

        // you shoud overwrite the function
        // reset the parameter so that the object can be reused
        public virtual void reset(){
            throw new System.Exception("reset function not implement in " + this.ToString());
        }
        // you should overwrite the function
        // put the object into pool, you should consider object state change such as setActive(false)
        public virtual void release(){
            throw new System.Exception("release function not implement in " + this.ToString());
        }
        
    }
}
