using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share{

    public class CachePool{
        private int poolSize = 0;
        private static int counter = 1;
        private List<CacheItem> mPool;
        private List<bool> used;
        private GameObject collector;
        private CacheItem sample;
        // you must give the original pool size
        // but you can not control the size after that
        // sampleItem is the object you wanna cache
        public CachePool(int _poolSize, CacheItem sampleItem){
            collector = new GameObject("Number " + counter + " Pool");
            counter++;
            this.poolSize = _poolSize;
            mPool = new List<CacheItem>(_poolSize);
            used = new List<bool>(_poolSize);
            sample = sampleItem;
            
            // initialization
            for(int i = 0;i < poolSize; i++){
                CacheItem temp = (CacheItem)CachePool.deepCopy(sampleItem);
                mPool.Add(temp);
                used.Add(false);
            }
        }
        // require an object from pool
        public CacheItem getObject()
        {
            CacheItem ret = null;
            int index = -1;
            // O(n) search
            for(int i = 0;i < used.Count; i++){
                if (!used[i]){
                    used[i] = true;
                    ret = mPool[i];
                    index = i;
                    break;
                }
            }

            // not free one found, create one then return
            if(index == -1){
                ret = (CacheItem)CachePool.deepCopy(sample);
                ret.init();
                mPool.Add(ret);
                used.Add(true);
                poolSize++;
            }
            ret.init();
            ret.cacheObject.SetActive(true);
            return ret;

        }


        // put object into pool
        public void release(GameObject mObject){
            
            for(int i = 0;i < used.Count; i++){
                if(mPool[i].cacheObject.GetInstanceID() == mObject.GetInstanceID()){
                    mPool[i].reset();
                    used[i] = false;
                    return;
                }
            }

            //throw new System.Exception(mObject.ToString() + " not found in pool " + this.ToString());
            int a = 0;
            mObject.SetActive(false);

        }

        private static object deepCopy(object _object)
        {
            System.Type T = _object.GetType();
            object o = System.Activator.CreateInstance(T);

            System.Reflection.PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++){

                System.Reflection.PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(_object));
            }

            return o;

        }



    }
}
