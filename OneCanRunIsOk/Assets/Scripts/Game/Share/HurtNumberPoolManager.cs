using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share{

    public class HurtNumberPoolManager{
        private const int poolSize = 32;
        private List<GameObject> pool;
        private List<bool> used;
        private GameObject sample;
        private Transform plane;

        // for hurt Number to release
        public static HurtNumberPoolManager instance;
        public HurtNumberPoolManager(GameObject hurtNumberObject, Transform _plane){
            instance = this;
            sample = hurtNumberObject;
            plane = _plane;
            pool = new List<GameObject>(poolSize);
            used = new List<bool>(poolSize);    

            for(int i = 0;i < poolSize; i++){
                GameObject temp = GameObject.Instantiate(sample, plane);
                temp.SetActive(false);
                pool.Add(temp);
                used.Add(false);
            }
        }
       


        public GameObject getObject(){
            GameObject ret = null;
            int index = -1;
            for (int i = 0; i < pool.Count; i++){

                if (!used[i]){

                    used[i] = true;
                    index = i;
                    ret = pool[i];
                    break;
                }
            }

            // in case too much require
            if (index == -1){

                GameObject temp = GameObject.Instantiate(sample, plane);
                pool.Add(temp);
                used.Add(true);
                ret = temp;
                //ret.SetActive(true);

            }
            // ?
            //ret.GetComponent<>

            return ret;

        }


        public void release(GameObject gameObject){
            
            gameObject.SetActive(false);
            for (int i = 0; i < pool.Count; i++){

                if (pool[i] == gameObject){
                    used[i] = false;
                    return;
                }
            }

            GameObject.Destroy(gameObject);

        }
    }
}
