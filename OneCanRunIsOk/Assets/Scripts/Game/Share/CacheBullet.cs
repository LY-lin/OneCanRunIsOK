using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share{

    public class CacheBullet : CacheItem{

        public CacheBullet():base(){

        }

        public CacheBullet(GameObject _gameObject):base(_gameObject){

        }

        


        public override void init(){
            this.cacheObject.GetComponent<BulletController>().m_ShootTime = Time.time;
        }


        public override void reset(){
            this.cacheObject.GetComponent<BulletController>().restart = true;
            this.cacheObject.SetActive(false);
        }


        public override void release(){
            this.cacheObject.SetActive(false);
        }


    }
}
