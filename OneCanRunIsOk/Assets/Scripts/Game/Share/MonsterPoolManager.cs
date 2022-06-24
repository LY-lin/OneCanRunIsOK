using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class MonsterPoolManager : MonoBehaviour
    {
        private static MonsterPoolManager monsterPoolManagerPtr = null;
<<<<<<< HEAD
        private const int cacheSize = 20;
        private static List<GameObject> sampleList;
        public List<GameObject> sampleList_exposed;
        //private static GameObject[] dataStream = new GameObject[cacheSize];
=======
        private const int cacheSize = 50;
        private static GameObject monster;
        private static List<GameObject> sampleList;
        public List<GameObject> sampleList_exposed;
        private static GameObject[] dataStream = new GameObject[cacheSize];
        private static List<List<GameObject>> pool;
>>>>>>> parent of 34ed919 (Revert "Merge branch 'new-new-new-branch' of https://github.com/LY-lin/OneCanRunIsOK into new-new-new-branch")
        private static GameObject parent;
        private static bool init = false;
        private static List<List<GameObject>> pool;
        private static List<List<bool>> used;
        public int activeNumber = 0;

        private void OnEnable(){
            if (monsterPoolManagerPtr == null){
                monsterPoolManagerPtr = this;
                sampleList = sampleList_exposed;
            }

        }


        public static MonsterPoolManager getInstance(){

            return monsterPoolManagerPtr;
        }


        // initialization
        public static void initialization(GameObject _parent){
<<<<<<< HEAD
            parent = _parent;
            pool = new List<List<GameObject>>();
            used = new List<List<bool>>();
            for(int i = 0;i < sampleList.Count; i++){
                pool.Add(new List<GameObject>(cacheSize));
                used.Add(new List<bool>(cacheSize));
                GameObject current = sampleList[i];
                for(int j = 0;j < cacheSize; j++){
                    GameObject temp = GameObject.Instantiate(current, _parent.transform);
                    temp.SetActive(false);
                    pool[i].Add(temp);
                    used[i].Add(false);

                }
=======
            pool = new List<List<GameObject>>(cacheSize);

           for(int i = 0;i < sampleList.Count; i++){
                GameObject current = sampleList[i];


>>>>>>> parent of 34ed919 (Revert "Merge branch 'new-new-new-branch' of https://github.com/LY-lin/OneCanRunIsOK into new-new-new-branch")
            }

            



        }

        // get a free object, if there is not a free one, a null will turn up.
        // you have to consider the rate in case there is not free object to get
        public GameObject getObject(int typeID,Vector3 position)
        {
            // illegal
            if (typeID < 0 || typeID >= pool.Count){
                Debug.LogError("Type " + typeID + " does not exit in pool");
                return null;
            }

            GameObject ret = null;
            int index = -1;
            for (int i = 0; i < cacheSize; i++)
            {
                if (!used[typeID][i]){

                    used[typeID][i] = true;
                    index = i;
                    ret = pool[typeID][i];
                    return ret;
                }
            }

            
            ret = UnityEngine.Object.Instantiate(sampleList[typeID], parent.transform);
            pool[typeID].Add(ret);
            used[typeID].Add(true);
            ret.transform.position = position;
            
            
            return ret;
        }

        // remove the object from scene 
        public void release(int typeID, GameObject objcect){
            objcect.SetActive(false);

            // illegal
            if (typeID < 0 || typeID >= pool.Count){
                Destroy(objcect);
                return;
            }

            for(int i = 0;i < pool[typeID].Count; i++){
                if(pool[typeID][i] == objcect){
                    used[typeID][i] = false;
                    return;
                }

            }
            Destroy(objcect);


        }

        public void release(GameObject objcect){
            objcect.SetActive(false);



            for(int i = 0;i < pool.Count; i++){
                for(int j = 0;j < pool[i].Count; j++){
                    if(pool[i][j] == objcect){
                    used[i][j] = false;
                    return;
                }
                }
            }
            Destroy(objcect);

        }


        public int getCacheSize(){
            return cacheSize;

        }
    }
}
