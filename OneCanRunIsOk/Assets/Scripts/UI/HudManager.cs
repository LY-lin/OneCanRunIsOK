using System.Collections;
using System.Collections.Generic;
using OneCanRun.GamePlay;
using UnityEngine;

namespace OneCanRun.UI
{
    public class HudManager : MonoBehaviour
    {
        [Tooltip("Canvas Group for all Hud UI")]
        public CanvasGroup Hud;

        CgManager cgManager;
        void Start()
        {
             cgManager = FindObjectOfType<CgManager>();
                cgManager.showAction +=open;
                cgManager.closeAction +=close;
            
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