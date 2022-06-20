using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.UI {
    public class PauseMenuController : MonoBehaviour {
        public GameObject title;
        public GameObject resumeButton;
        public GameObject optionButton;
        public GameObject backTitleButton;
        // option menu
        public GameObject volumeTip;
        public GameObject volumeSlider;
        public GameObject back2PauseMenuButton;

        private void Start(){
            resumeButton = GameObject.Find("Play");
            backTitleButton = GameObject.Find("Back2Title");
            optionButton = GameObject.Find("Options");
            volumeSlider = GameObject.Find("VolumeSlider");
            back2PauseMenuButton = GameObject.Find("Back");
            volumeTip = GameObject.Find("VolumeTitle");
            volumeSlider.SetActive(false);
            back2PauseMenuButton.SetActive(false);
            volumeTip.SetActive(false);
        }

        private void OnEnable()
        {
            
        }

        public void resume() {
            PauseMenu pauseMenu = this.GetComponentInParent<PauseMenu>();
            pauseMenu.resume();
        }

        public void enterOptions() {
            resumeButton.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            backTitleButton.gameObject.SetActive(false);
            title.GetComponent<TMPro.TextMeshProUGUI>().text = "Options";
            volumeSlider.SetActive(true);
            back2PauseMenuButton.SetActive(true);
            volumeTip.SetActive(true);

        }

        public void back2Title() {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("myStartScene");
            

        }

        public void back2Menu(){
            title.GetComponent<TMPro.TextMeshProUGUI>().text = "Pause";
            resumeButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            backTitleButton.gameObject.SetActive(true);
            volumeSlider.SetActive(false);
            back2PauseMenuButton.SetActive(false);
            volumeTip.SetActive(false);


        }

    }
}
