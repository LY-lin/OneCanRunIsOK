using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class MonsterFresh : MonoBehaviour
    {
        private List<OneCanRun.Game.Share.MonsterFreshInfo> mMonsterList;
        private static int counter = 0;
        private List<GameObject> monsterSample;

        public GameObject monster1;
        public GameObject monster2;
        // Start is called before the first frame update

        private void OnEnable(){
            mMonsterList = new List<Game.Share.MonsterFreshInfo>();
            monsterSample = new List<GameObject>();
            if (monster1 != null)
                monsterSample.Add(monster1);
            if (monster2 != null)
                monsterSample.Add(monster2);

            // read config from file
            string configDirectory = System.IO.Directory.GetCurrentDirectory();
            configDirectory += "\\Config\\MonsterFreshConfig.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(configDirectory);
            

            XmlNodeList xmlNodeList = xml.SelectSingleNode("MonsterConfig").ChildNodes;

            foreach(XmlElement xl1 in xmlNodeList){
                //int wave = int.Parse(xl1.GetAttribute("Wave"));
                int time = 0;
                float position_x = 0;
                float position_y = 0;
                float position_z = 0;
                int typeID = 0;
                foreach(XmlElement x12 in xl1.ChildNodes){
                    if(x12.Name == "Time"){
                        time = int.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_x"){
                        position_x = float.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_y"){
                        position_y = float.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_z"){
                        position_z = float.Parse(x12.InnerText); 
                    }
                    if (x12.Name == "TypeID"){
                        typeID = int.Parse(x12.InnerText);
                    }
                }

                Game.Share.MonsterFreshInfo tmp = new Game.Share.MonsterFreshInfo(time, position_x, position_y, position_z, typeID);
                mMonsterList.Add(tmp);
            }

        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (counter >= mMonsterList.Count)
                return;

            if((int)Time.time >= mMonsterList[counter].time){
                Debug.Log("Fresh!");
                Game.Share.MonsterFreshInfo current = mMonsterList[counter];
                GameObject.Instantiate(monsterSample[current.typeID], new Vector3(current.position_x, current.position_y, current.position_z),
                    new Quaternion(0, 0, 0, 0), this.transform);

                counter++;
            }

        }
    }
}
