using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun
{
    public class Dropable : MonoBehaviour
    {
        private GameObject targetDrop;
        void Start()
        {
            GameObject dropList = GameObject.Find("DropList");
            List<GameObject> tempList = 
                dropList.GetComponent<Game.Share.DropList>().dropList;
            if (tempList.Count == 0)
                return;
            int target = Random.Range(0, tempList.Count);
            targetDrop = tempList[target];
            Debug.Log(target);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        //public ForceMode
        public void drop(){
            Rigidbody m_Rigidbody;
            Transform burnPosition = this.gameObject.transform.GetChild(3).GetComponent<Transform>();
            //Debug.Log(this.gameObject.transform.GetChild(3).GetComponent<Transform>().position);
            //popForce.set
            if (targetDrop != null){
                GameObject loot = GameObject.Instantiate(targetDrop, burnPosition);
                Debug.Log(loot.GetComponent<Transform>().position);
                m_Rigidbody = loot.GetComponent<Rigidbody>();
                float m_ForceX= Random.Range(1f,2f), m_ForceY = 3f, m_ForceZ = Random.Range(1f, 2f);
                Vector3 m_NewForce = new Vector3(m_ForceX, m_ForceY, m_ForceZ);
                m_Rigidbody.AddForce(m_NewForce, ForceMode.Impulse);
            }

        }
    }
}
