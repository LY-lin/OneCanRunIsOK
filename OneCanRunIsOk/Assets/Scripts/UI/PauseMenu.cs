using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OneCanRun.UI
{
    public class PauseMenu : MonoBehaviour
    {
        // in case collision for game menu
        public GameObject GameMenu;
        private Backpack backpack;
        public GameObject pauseMenuObject;
        public GameObject optionsMenuObject;
        public Transform plane;

        PauseMenuController pause1;

        void Start(){
            backpack = GameMenu.GetComponent<InGamePackageManager>().getBackpack();
            GameObject pause = GameObject.Instantiate(pauseMenuObject, plane);
            pause1 = pause.GetComponent<PauseMenuController>();
            pause1.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update(){

            // open pause menu
            if (!backpack)
                return;
            if(!backpack.gameObject.activeSelf && Input.GetButtonDown(Game.GameConstants.k_ButtonNameGadiaPauseMenu)){

                setPauseMenuActivation(!pause1.gameObject.activeSelf);
            }

        }

        void setPauseMenuActivation(bool active){
            pause1.gameObject.SetActive(active);
            if (pause1.gameObject.activeSelf){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);


            }
            else{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;

            }
        }

        public void resume(){
            setPauseMenuActivation(false);
        }

    }
}
