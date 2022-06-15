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
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void drop(){
            if(targetDrop != null){
                GameObject.Instantiate(targetDrop, this.transform);

            }

        }
    }
}
