using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.Game.Share
{
    public class BulletPoolManager
    {
        const int cacheSize = 256;
        private static int counter = 1;
        private GameObject bullet;
        private GameObject collector;
        private GameObject[] dataStream = new GameObject[cacheSize];
        private bool[] used = new bool[cacheSize];

        // initialization
        public BulletPoolManager(GameObject _bullet)
        {
            collector = new GameObject("BulletCollector" + counter);
            counter++;
            bullet = _bullet;
            for (int i = 0; i < cacheSize; i++)
            {
                GameObject temp = UnityEngine.Object.Instantiate(bullet, collector.transform);
                temp.GetComponent<BulletController>().restart = false;
                temp.SetActive(false);
                dataStream[i] = temp;
                used[i] = false;
            }

        }

        // get a free object, if there is not a free one, a null will turn up.
        // you have to consider the rate in case there is not free object to get
        public GameObject getObject(Vector3 position, Quaternion rotation)
        {
            GameObject ret = null;
            int index = -1;
            for (int i = 0; i < cacheSize; i++)
            {
                if (!used[i])
                {
                    used[i] = true;
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                ret = dataStream[index];
                ret.transform.position = position;
                ret.transform.rotation = rotation;
                ret.GetComponent<BulletController>().m_ShootTime = Time.time;

                ret.SetActive(true);

            }

            return ret;
        }

        // remove the object from scene 
        public void release(GameObject objcect)
        {
            objcect.GetComponent<BulletController>().restart = true;
            objcect.SetActive(false);
            for (int i = 0; i < cacheSize; i++)
            {
                if (dataStream[i] == objcect)
                {
                    used[i] = false;
                    break;
                }
            }

        }
    }
}
