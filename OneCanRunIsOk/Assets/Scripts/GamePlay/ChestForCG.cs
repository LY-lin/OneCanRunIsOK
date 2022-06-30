using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.GamePlay
{
    public class ChestForCG : MonoBehaviour
    {
        public GameObject PortalVFX;

        BossAwake trap;
        // Start is called before the first frame update
        void Start()
        {
            trap = GetComponent<BossAwake>();
            trap.bossAwake += CreatPortal;
        }

        // Update is called once per frame
        public void CreatPortal()
        {

        }
    }
}
