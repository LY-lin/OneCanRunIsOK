using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share{

    public class CacheItem{
        public GameObject cacheObject;

        // for clone
        public CacheItem(){

        }

        public CacheItem(GameObject gameObject){
            cacheObject = gameObject;
            GameObject.Instantiate(cacheObject);
            cacheObject.SetActive(false);
        }

        public CacheItem(GameObject gameObject, GameObject parent){
            cacheObject = gameObject;
            GameObject.Instantiate(cacheObject, parent.transform);
            cacheObject.SetActive(false);

        }

        // you should overwrite the function
        // initialize the parameter you need, you must pass the object you wanna cache
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
