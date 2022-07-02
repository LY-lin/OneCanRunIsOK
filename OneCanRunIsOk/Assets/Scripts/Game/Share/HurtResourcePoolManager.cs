using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun
{
    public class HurtResourcePoolManager : MonoBehaviour
    {
        private const int poolSize = 10;
        private static List<GameObject> pool;
        private static List<bool> used;
        private static GameObject sample;
        private static Transform plane;

        // for hurt Number to release
        public static HurtResourcePoolManager instance;
        public HurtResourcePoolManager(GameObject hurtResourceObject, Transform _plane)
        {
            instance = this;


            sample = hurtResourceObject;
            plane = _plane;
            pool = new List<GameObject>(poolSize);
            used = new List<bool>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                GameObject temp = GameObject.Instantiate(sample, plane);
                temp.SetActive(false);
                pool.Add(temp);
                used.Add(false);
            }
        }

        public void reset(GameObject hurtResourceObject, Transform _plane)
        {
            sample = hurtResourceObject;
            plane = _plane;
            pool = new List<GameObject>(poolSize);
            used = new List<bool>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                GameObject temp = GameObject.Instantiate(sample, plane);
                temp.SetActive(false);
                pool.Add(temp);
                used.Add(false);
            }
        }

        public GameObject getObject()
        {
            GameObject ret = null;
            int index = -1;
            for (int i = 0; i < pool.Count; i++)
            {

                if (!used[i])
                {

                    used[i] = true;
                    index = i;
                    ret = pool[i];
                    break;
                }
            }

            // in case too much require
            if (index == -1)
            {

                GameObject temp = GameObject.Instantiate(sample, plane);
                pool.Add(temp);
                used.Add(true);
                ret = temp;
                //ret.SetActive(true);

            }
            // ?
            //ret.GetComponent<>
            ret.SetActive(true);
            return ret;

        }


        public void release(GameObject gameObject)
        {
            Debug.LogError("error");
            gameObject.SetActive(false);
            for (int i = 0; i < pool.Count; i++)
            {

                if (pool[i] == gameObject)
                {
                    used[i] = false;
                    return;
                }
            }

            GameObject.Destroy(gameObject);

        }
    }
}
