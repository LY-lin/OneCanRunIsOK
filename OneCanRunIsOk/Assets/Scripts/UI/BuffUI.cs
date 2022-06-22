
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OneCanRun.GamePlay;
using OneCanRun.Game;
namespace OneCanRun.UI
{
    public class BuffUI : MonoBehaviour
    {
        [Tooltip("Image for Buff icon")]
        public Image BuffIMG;

        [Tooltip("Plane to Manage Num")]
        public RectTransform NumPlane;

        [Tooltip("Number of the Buff")]
        public TextMeshProUGUI Num;

        [Tooltip("Last time of the Buff")]
        public TextMeshProUGUI LastTime;

        [Tooltip("Description of the Buff")]
        public TextMeshProUGUI Description;
        // Start is called before the first frame update
        [Tooltip("Description's Canvas Group")]
        public CanvasGroup itsDes;

        public BuffController m_buff;
        public string BuffName { get; private set; }

        private void Update()
        {
            
            if(!m_buff.GetIsForever())
            {

                LastTime.text = m_buff.getTime.ToString("0.0");
            }
            else
            {
                //Num.text = m_buff.Num;
            }
        }

        public void Initialize(BuffController buff,string name)
        {
            m_buff = buff;
            BuffIMG.sprite = buff.BuffIcon;
            BuffName = name;
            Description.text = buff.Description;
            if(buff.GetIsForever())
            {
                LastTime.gameObject.SetActive(false);
                NumPlane.gameObject.SetActive(true);
            }
            else
            {
                LastTime.gameObject.SetActive(true);
                NumPlane.gameObject.SetActive(false);
            }

            Description.gameObject.SetActive(false);
            itsDes.alpha = 0;
        }

        public void EnterUIShow()
        {
            itsDes.alpha = 1; 
        }
        public void ExitUIHide()
        {
            itsDes.alpha = 0;
        }
        


    }
}