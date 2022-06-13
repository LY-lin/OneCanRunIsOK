
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

        [Tooltip("Tips for Button")]
        public RectTransform Button;

        [Tooltip("if too far to see")]
        public bool HideFar = true;

        [Tooltip("Distance to hide information")]
        public float distance;
        // Start is called before the first frame update

        private Interactive interactive;
        void Start()
        {
            interactive = GetComponent<Interactive>();
            information.text =interactive.description;
        }

        // Update is called once per frame
        void Update()
        {
            if (interactive.showInteractiveUI)
            {
                PlayerCharacterController player = FindObjectOfType<PlayerCharacterController>();
                DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, Interactive>(player, this);

                Vector3 position1 = player.transform.parent.localPosition;
                Vector3 position2 = this.transform.parent.localPosition;
                if (HideFar && Vector3.Distance(position1, position2) < distance)
                {
                    this.gameObject.SetActive(true);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
