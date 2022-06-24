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
        private int waveNumber = 4;
        public int timeInterval = 30;
        private int timeCounter = 0;
        private int frameInterval = 0;
        private Game.Share.MonsterPoolManager monsterPoolManager;
        private int startTime;
        // Start is called before the first frame update

        private int compareMonsterFreshInfo(Game.Share.MonsterFreshInfo a, Game.Share.MonsterFreshInfo b){
            if (a.time < b.time)
                return -1;
            if (a.time > b.time)
                return 1;
            return 0;

        }

        private void OnEnable(){
            startTime = (int)Time.time;
            frameInterval = (int)(((float)timeInterval)/Time.deltaTime);
            mMonsterList = new List<Game.Share.MonsterFreshInfo>();
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


            mMonsterList.Sort(compareMonsterFreshInfo);

        }

        void Start()
        {
            Game.Share.MonsterPoolManager.initialization(this.gameObject);
            monsterPoolManager = Game.Share.MonsterPoolManager.getInstance();
        }

        // Update is called once per frame
        void Update()
        {

            // fresh as stable time
            if (Time.frameCount% frameInterval == 0){
                //refreshOneWave();

            }


            if (counter >= mMonsterList.Count)
                return;
            
            

            if((int)Time.time - startTime >= mMonsterList[counter].time){
                Game.Share.MonsterFreshInfo current = mMonsterList[counter];
                for(int i = 0;i < current.number; i++){
                   GameObject gameObject = monsterPoolManager.getObject(current.typeID, new Vector3(current.position_x, current.position_y, current.position_z));
                    if (gameObject)
                        gameObject.SetActive(true);
                }
                counter++;
            }

        }

        void refreshAllFreeMonster(){
            while (monsterPoolManager.activeNumber < monsterPoolManager.getCacheSize()){
                //monsterPoolManager.getObject(new Vector3(64, 5, 60));
                GameObject temp = monsterPoolManager.getObject(new Vector3(42, 0.7f, 22));
                
            }

        }

        void refreshOneWave(){
            for(int i = 0;i < waveNumber; i++){
                GameObject temp = monsterPoolManager.getObject(new Vector3(42, 0.7f, 22));
            }

        }
    }
}
