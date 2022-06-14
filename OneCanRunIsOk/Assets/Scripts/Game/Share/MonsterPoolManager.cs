using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class MonsterPoolManager : MonoBehaviour
    {
        private static MonsterPoolManager monsterPoolManagerPtr = null;
        private const int cacheSize = 200;
        private static GameObject monster;
        private static GameObject[] dataStream = new GameObject[cacheSize];
        private static GameObject parent;
        private static bool init = false;
        private static bool[] used = new bool[cacheSize];
        public int activeNumber = 0;

        private void OnEnable(){
            if (monsterPoolManagerPtr == null)
                monsterPoolManagerPtr = this;

        }


        public static MonsterPoolManager getInstance(){

            return monsterPoolManagerPtr;
        }


        // initialization
        public static void initialization(GameObject _parent, GameObject _monster)
        {
            parent = _parent;
            if (init)
                return;
            init = true;
            monster = _monster;

            for (int i = 0; i < cacheSize; i++)
            {
                GameObject temp = UnityEngine.Object.Instantiate(monster, _parent.transform);
                temp.SetActive(false);
                dataStream[i] = temp;
                used[i] = false;
            }

        }

        // get a free object, if there is not a free one, a null will turn up.
        // you have to consider the rate in case there is not free object to get
        public GameObject getObject(Vector3 position)
        {
            GameObject ret = null;
            int index = -1;
            for (int i = 0; i < cacheSize; i++)
            {
                if (!used[i])
                {
                    used[i] = true;
                    index = i;
                    activeNumber++;
                    break;
                }
            }

            if (index != -1)
            {
                ret = dataStream[index];
                ret.transform.position = position;
                
                ret.SetActive(true);

            }
            else{
                ret = UnityEngine.Object.Instantiate(monster, parent.transform);
                //ret = dataStream[index];
                ret.transform.position = position;
            }

            return ret;
        }

        // remove the object from scene 
        public void release(GameObject objcect)
        {
            bool toDestory = true;
            for (int i = 0; i < cacheSize; i++)
            {
                if (dataStream[i] == objcect)
                    toDestory = false;

                if (dataStream[i] == objcect && used[i])
                {
                    objcect.GetComponent<Health>().CurrentHealth = objcect.GetComponent<Health>().MaxHealth;
                    objcect.GetComponent<Health>().m_IsDead = false;
                    objcect.SetActive(false);
                    activeNumber--;
                    used[i] = false;
                    return;
                }
            }
            if(toDestory)
                Destroy(objcect);

        }

        public int getCacheSize(){
            return cacheSize;

        }
    }
}
