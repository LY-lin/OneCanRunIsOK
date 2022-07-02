using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

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
        private int levelCounter = 1;
        private Game.Share.MonsterPoolManager monsterPoolManager;
        private static int startTime;
        public static bool freshable = true;
        // Start is called before the first frame update

        public UnityAction newWave;

        private int compareMonsterFreshInfo(Game.Share.MonsterFreshInfo a, Game.Share.MonsterFreshInfo b){
            if (a.time < b.time)
                return -1;
            if (a.time > b.time)
                return 1;
            return 0;

        }

        public static void setTime(float _time)
        {
            counter = 0;
            startTime = (int)_time;
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
                int number = 0;
                int level = 1;
                foreach(XmlElement x12 in xl1.ChildNodes){
                    if(x12.Name == "Time"){
                        time = int.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_x"){
                        position_x = float.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_y") {
                        position_y = float.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Position_z"){
                        position_z = float.Parse(x12.InnerText); 
                    }
                    if (x12.Name == "TypeID"){
                        typeID = int.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Number"){
                        number = int.Parse(x12.InnerText);
                    }
                    if (x12.Name == "Level"){
                        number = int.Parse(x12.InnerText);
                    }
                }

                Game.Share.MonsterFreshInfo tmp = new Game.Share.MonsterFreshInfo(time, position_x, position_y, position_z, typeID, number, level);
                mMonsterList.Add(tmp);
            }


            mMonsterList.Sort(compareMonsterFreshInfo);

        }

        void Start()
        {
            setTime(Time.time);
            Game.Share.MonsterPoolManager.initialization(this.gameObject);
            monsterPoolManager = Game.Share.MonsterPoolManager.getInstance();
        }

        // Update is called once per frame
        void Update(){

            if (!freshable)
                return;

            // fresh as stable time
            if (Time.frameCount% frameInterval == 0 && monsterPoolManager.activeNumber <= 30){
                freshOnceButInfinite();

            }


            if (counter >= mMonsterList.Count)
                return;
            
            

            if((int)Time.time - startTime >= mMonsterList[counter].time){
                Game.Share.MonsterFreshInfo current = mMonsterList[counter];
                for(int i = 0;i < current.number; i++){
                   GameObject gameObject = monsterPoolManager.getObject(current.typeID, new Vector3(current.position_x, current.position_y, current.position_z));
                    if (gameObject){
                        gameObject.GetComponent<Game.Actor>().setLevel((uint)(levelCounter + 1));
                        levelCounter++;
                        gameObject.SetActive(true);
                        Debug.Log(gameObject.transform.position);
                    }
                }

                newWave?.Invoke();
                counter++;
            }

        }



        void freshOnceButInfinite(){
            for(int i = 0;i < 5; i++){
                Game.Share.MonsterFreshInfo current = mMonsterList[Random.Range(0, mMonsterList.Count)];
                GameObject gameObject = monsterPoolManager.getObject(Random.Range(0, monsterPoolManager.totalMonsterNumber), new Vector3(current.position_x, current.position_y, current.position_z));
                if (gameObject){
                    gameObject.GetComponent<Game.Actor>().setLevel((uint)(levelCounter + 1));
                    levelCounter++;
                    gameObject.SetActive(true);
                }

            }

        }
    }
}
