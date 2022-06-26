using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game.Share
{
    public class SpitFlame : MonoBehaviour
    {
        public GameObject flameObject;
        public GameObject Owner;
        public Transform spitSocket;

        //private DragonFlame dragonFlame;
        private GameObject flameInstance;
        private bool isSpitting=false;

        // Start is called before the first frame update
        //void Start()
        //{
        //    dragonFlame = flameObject.GetComponent<DragonFlame>();
        //    DebugUtility.HandleErrorIfNullGetComponent<DragonFlame, SpitFlame>(dragonFlame, this,
        //        gameObject);
        //    dragonFlame.Owner = this.Owner;
        //}

        public void StartSpitting()
        {
            if (!isSpitting)
            {
                flameInstance = Instantiate(flameObject, spitSocket);
                flameInstance.GetComponent<DragonFlame>().Owner = this.Owner;
                isSpitting = true;
            }
        }

        public void StopSpitting()
        {
            if (isSpitting && flameInstance)
            {
                Destroy(flameInstance);
                isSpitting = false;
            }
        }
    }
}
