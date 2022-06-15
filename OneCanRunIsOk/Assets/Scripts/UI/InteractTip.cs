
using UnityEngine;
using TMPro;
using OneCanRun.GamePlay;

namespace OneCanRun.UI
{
    public class InteractTip : MonoBehaviour
    {
        [Tooltip("text information")]
        public TextMeshProUGUI information;

        /*
        [Tooltip("Tips for Button")]
        public RectTransform Button;
        */
        [Tooltip("Canvas Group")]
        public Transform CanvasGroup;

        [Tooltip("Canvas Group")]
        public CanvasGroup CanvasG;

        [Tooltip("if too far to see")]
        public bool HideFar = true;

        [Tooltip("Distance to hide information")]
        public float distance;
        // Start is called before the first frame update

        private Interactive interactive;
        void Start()
        {
            interactive = GetComponentInParent<Interactive>();
            information.text =interactive.description;
        }

        // Update is called once per frame
        void Update()
        {
            if (interactive.showInteractiveUI)
            {

                    CanvasG.alpha = 1;
                    CanvasGroup.LookAt(-Camera.main.transform.position);
            }
            else
            {
                CanvasG.alpha = 0;
            }
        }
    }
}
