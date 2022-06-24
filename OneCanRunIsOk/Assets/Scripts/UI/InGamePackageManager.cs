using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OneCanRun.GamePlay;
using OneCanRun.Game;
namespace OneCanRun.UI
{
    public class InGamePackageManager : MonoBehaviour
    {

        [Tooltip("Root GameObject of the menu used to toggle its activation")]
        public GameObject BackpackPrefab;

        [Tooltip("UI plane for containing the backpack prefab")]
        public RectTransform BackPlane;

        PlayerInputHandler m_PlayerInputsHandler;
        Health m_PlayerHealth;
        Backpack m_backpack;

        public Backpack getBackpack(){
            return m_backpack;

        }


        void Start()
        {

            
            GameObject backpackInstance = Instantiate(BackpackPrefab, BackPlane);
            Backpack newBackpack = backpackInstance.GetComponent<Backpack>();
            DebugUtility.HandleErrorIfNullGetComponent<Backpack, InGamePackageManager>(newBackpack,
                this, backpackInstance.gameObject);
            newBackpack.initialize();
            m_backpack = newBackpack;

            m_backpack.gameObject.SetActive(false);
        }

        void Update()
        {
            

            //输入esc，退出背包
            if (m_backpack.gameObject.activeSelf&&Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            //按下tab键开关背包
            if (Input.GetButtonDown(GameConstants.k_ButtonNamePauseMenu)
                || (m_backpack.gameObject.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel)))
            {
                Debug.Log("Pause");
                SetPauseMenuActivation(!m_backpack.gameObject.activeSelf);

            }

            /* //滚轴
            if (Input.GetAxisRaw(GameConstants.k_AxisNameVertical) != 0)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
            */
        }

        public void ClosePauseMenu()
        {
            SetPauseMenuActivation(false);
        }

        void SetPauseMenuActivation(bool active)
        {

            m_backpack.gameObject.SetActive(active);
            if (m_backpack.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                EventSystem.current.SetSelectedGameObject(null);
                m_backpack.updateContent();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
            

        }



    }
}