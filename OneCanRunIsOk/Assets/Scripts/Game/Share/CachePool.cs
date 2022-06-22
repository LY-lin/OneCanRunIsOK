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
                mPool.Add(GameObject.Instantiate(sample, collector.transform));
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
                    index = i;
                    break;
                }
            }

            // not free one found, create one then return
            if(index == -1){
                ret = new CacheItem();
                ret.init();
                mPool.Add(ret);
                used.Add(true);
                poolSize++;
            }

            return ret;

        }


        // put object into pool
        public void release(CacheItem item){
            item.reset();
            for(int i = 0;i < used.Count; i++){
                if(mPool[i] == item){
                    used[i] = false;
                    return;
                }
            }

            throw new System.Exception(item.ToString() + " not found in pool " + this.ToString());


        }



    }
}
