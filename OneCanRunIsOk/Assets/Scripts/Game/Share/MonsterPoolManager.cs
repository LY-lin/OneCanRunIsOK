using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class MonsterPoolManager : MonoBehaviour
    {
        private static MonsterPoolManager monsterPoolManagerPtr = null;
        private const int cacheSize = 20;
        private static List<GameObject> sampleList;
        public List<GameObject> sampleList_exposed;
        private static GameObject parent;

        // for accelerating seach
        private static Dictionary<string, int> lookupTable;
        private static List<List<GameObject>> pool;
        private static List<List<bool>> used;
        public int activeNumber = 0;

        private void OnEnable(){
            if (monsterPoolManagerPtr == null){
                monsterPoolManagerPtr = this;
            }

        }


        public static MonsterPoolManager getInstance(){

            return monsterPoolManagerPtr;
        }


        // initialization
        public static void initialization(GameObject _parent){
            parent = _parent;
            pool = new List<List<GameObject>>();
            lookupTable = new Dictionary<string, int>();
            sampleList = monsterPoolManagerPtr.sampleList_exposed;
            used = new List<List<bool>>();
            for(int i = 0;i < sampleList.Count; i++){
                pool.Add(new List<GameObject>(cacheSize));
                used.Add(new List<bool>(cacheSize));
                GameObject current = sampleList[i];
                lookupTable.Add(sampleList[i].name + "(Clone)", i);
                for(int j = 0;j < cacheSize; j++){
                    GameObject temp = GameObject.Instantiate(current, _parent.transform);
                    temp.SetActive(false);
                    pool[i].Add(temp);
                    used[i].Add(false);

                }
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

            if(index == -1){
                ret = UnityEngine.Object.Instantiate(sampleList[typeID], parent.transform);
                pool[typeID].Add(ret);
                used[typeID].Add(true);
            }
            ret.transform.position = position;
            
            return ret;
        }

        // remove the object from scene 
        public void release(int typeID, GameObject objcect){
            Health health =  objcect.GetComponent<Health>();
            health.m_IsDead = false;
            ActorProperties properties = objcect.GetComponent<Actor>().GetActorProperties();
            health.MaxHealth = properties.getMaxHealth();
            health.CurrentHealth = properties.getMaxHealth();
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


        // main way to release
        public void release(GameObject objcect){
            Health health =  objcect.GetComponent<Health>();
            health.m_IsDead = false;
            ActorProperties properties = objcect.GetComponent<Actor>().GetActorProperties();
            health.MaxHealth = properties.getMaxHealth();
            health.CurrentHealth = properties.getMaxHealth();
            objcect.SetActive(false);

            objcect.gameObject.GetComponent<Actor>().reset();
            int typeID;
            if (!lookupTable.TryGetValue(objcect.name, out typeID))
                throw new System.Exception(objcect.name + " is illegal ");

            for(int j = 0;j < pool[typeID].Count; j++){
                if(pool[typeID][j] == objcect){
                    used[typeID][j] = false;
                    return;
                }
            }
            
            Destroy(objcect);

        }


        public int getCacheSize(){
            return cacheSize;

        }
    }
}
