
using UnityEngine;
using TMPro;
using OneCanRun.GamePlay;
using UnityEngine.UI;

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
        [Tooltip("Pivot")]
        public Transform Pivot;

        [Tooltip("Canvas Group")]
        public CanvasGroup plane;

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
                plane.gameObject.SetActive(true);
                if(Camera.main)
                    Pivot.LookAt(Camera.main.transform.position);
            }
            else
            {

                plane.gameObject.SetActive(false);
            }
        }
    }
}
