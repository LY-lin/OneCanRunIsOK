
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OneCanRun.GamePlay;
using OneCanRun.Game;

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
              
                    this.gameObject.SetActive(true);
                    CanvasGroup.LookAt(-Camera.main.transform.position);
                    //this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
