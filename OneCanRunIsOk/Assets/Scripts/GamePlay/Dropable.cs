using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneCanRun.Game.Share;

namespace OneCanRun.GamePlay
{
    public class Dropable : MonoBehaviour
    {
        public DropList m_DropList;
        private GameObject targetDrop;
        int[] pre;
        void Start()
        {
            //GameObject dropList = GameObject.Find("DropList");
            //List<GameObject> tempList = 
            //    dropList.GetComponent<Game.Share.DropList>().dropList;
            if (m_DropList.dropList.Count == 0)
                return;
            List<int> weightList = m_DropList.weight;
            pre = new int[weightList.Count];
            pre[0] = weightList[0];
            int total = weightList[0];
            for(int i = 1; i < weightList.Count; i++)
            {
                pre[i] = pre[i - 1] + weightList[i];
                total += weightList[i];
            }
            int target = Random.Range(1, total + 1);
            targetDrop = m_DropList.dropList[BinarySearch(target)];
            //Debug.Log(target);
        }

        // Update is called once per frame
        //void Update()
        //{
        
        //}
        //public ForceMode  
        public void drop(){
            Rigidbody m_Rigidbody;
            Transform burnPosition = this.gameObject.transform.Find("LootBurn").GetComponent<Transform>();
            //Debug.Log(this.gameObject.transform.GetChild(3).GetComponent<Transform>().position);
            //popForce.set
            if (targetDrop != null){
                GameObject loot = GameObject.Instantiate(targetDrop,burnPosition.position,burnPosition.rotation);
                
                Debug.Log(loot.GetComponent<Transform>().position);
                m_Rigidbody = loot.GetComponent<Rigidbody>();
                float m_ForceX= Random.Range(1f,2f), m_ForceY = 3f, m_ForceZ = Random.Range(1f, 2f);
                Vector3 m_NewForce = new Vector3(m_ForceX, m_ForceY, m_ForceZ);
                m_Rigidbody.AddForce(m_NewForce, ForceMode.Impulse);
            }

        }

        private int BinarySearch(int x)
        {
            int low = 0, high = pre.Length - 1;
            while (low < high)
            {
                int mid = (high - low) / 2 + low;
                if (pre[mid] < x)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            return low;
        }

    }
}
