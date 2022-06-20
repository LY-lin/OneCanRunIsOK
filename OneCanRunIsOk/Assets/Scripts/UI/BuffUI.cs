
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

        [Tooltip("Number of the Buff")]
        public TextMeshProUGUI Num;

        [Tooltip("Last time of the Buff")]
        public TextMeshProUGUI LastTime;

        [Tooltip("Description of the Buff")]
        public TextMeshProUGUI Description;
        // Start is called before the first frame update
        [Tooltip("Description's Canvas Group")]
        public CanvasGroup itsDes;

        BuffController m_buff;
        public string BuffName { get; private set; }

        private void OnEnable()
        {
            Num.text = m_buff.Num;
            if(!m_buff.GetIsForever())
            {
                LastTime.gameObject.SetActive(true);
                LastTime.text = m_buff.getExistTime().ToString();
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