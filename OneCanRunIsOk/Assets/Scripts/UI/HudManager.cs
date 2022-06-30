using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.UI
{
    public class HudManager : MonoBehaviour
    {
        [Tooltip("Canvas Group for all Hud UI")]
        public CanvasGroup Hud;

        //CgManager cgManager;
        // Start is called before the first frame update
        void Start()
        {
            /* cgManage = FindObjectOfType<CgManager>();
                cgManager.showAction +=open;
                cgManager.closeAction +=close;
            */
        }

        // Update is called once per frame
        public void open()
        {
            Hud.alpha = 1;
        }

        public void close()
        {
            Hud.alpha = 0;
        }
    }

}