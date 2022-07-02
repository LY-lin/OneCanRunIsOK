using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.Game.Share;

namespace OneCanRun.UI
{
    public class HurtFeedback : MonoBehaviour
    {
        [Tooltip("Cross Image")]
        public Image Cross;
        [Tooltip("Feedback Sprite")]
        public Sprite Feedback;

        [Tooltip("Crosshair sprite")]
        public Sprite Crosshair;

        [Tooltip("Time to stand showing feedback")]
        public float standTime = 0.5f;

        private CollectDamage collect;
        private float lastTime = 0;
        private bool usingFB = false;
        // Start is called before the first frame update
        void Start()
        {
            collect = GetComponentInParent<CollectDamage>();
            collect.Feedback += ShowFeedback;
        }

        // Update is called once per frame
        void Update()
        {
            if (usingFB&&Time.time - lastTime >= standTime)
            {
                Cross.sprite = Crosshair;
            }
        }

        void ShowFeedback()
        {
            Cross.sprite = Feedback;
            usingFB = true;
            lastTime = Time.time;
        }
    }
}
