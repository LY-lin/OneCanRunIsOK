using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class LaserRealse : MonoBehaviour
    {
        public GameObject laserObject;
        public GameObject Owner;
        public Transform laserSocket;

        //private LaserController laser;
        private GameObject laserInstance;
        private bool isLasering = false;

        // Start is called before the first frame update
        //void Start()
        //{
        //    laser = laserObject.GetComponent<LaserController>();
        //    DebugUtility.HandleErrorIfNullGetComponent<LaserController, LaserRealse>(draglaseronFlame, this,
        //        gameObject);
        //    laser.Owner = this.Owner;
        //}

        public void StartLasering()
        {
            if (!isLasering)
            {
                laserInstance = Instantiate(laserObject, laserSocket);
                laserInstance.GetComponent<LaserController>().Owner = this.Owner;
                isLasering = true;
            }
        }

        public void StopLasering()
        {
            if (isLasering && laserInstance)
            {
                laserInstance.GetComponent<LaserController>().DisablePrepare();
                Destroy(laserInstance,0.1f);
                isLasering = false;
            }
        }
    }
}
