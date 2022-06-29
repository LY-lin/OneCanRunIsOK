using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.Game.Share;

namespace OneCanRun.UI
{
    public class HurtFeedback : MonoBehaviour
    {
        [Tooltip("Feedback Image")]
        public Image Feedback;

        [Tooltip("Crosshair image")]
        public Image Crosshair;

        [Tooltip("Time to stand showing feedback")]
        public float standTime = 0.5f;

        private CollectDamageNumber collect;
        private float lastTime = 0;
        // Start is called before the first frame update
        void Start()
        {
            collect = GetComponentInParent<CollectDamageNumber>();
            collect.Feedback += ShowFeedback;
            Feedback.gameObject.SetActive(false);
            Crosshair.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (Feedback.gameObject.activeSelf && Time.time - lastTime >= standTime)
            {
                Feedback.gameObject.SetActive(false);
                Crosshair.gameObject.SetActive(true);
            }
        }

        void ShowFeedback()
        {
            Feedback.gameObject.SetActive(true);
            Crosshair.gameObject.SetActive(false);
            lastTime = Time.time;
        }
    }
}
