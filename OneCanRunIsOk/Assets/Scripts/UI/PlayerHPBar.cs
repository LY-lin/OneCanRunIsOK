
using OneCanRun.Game;
using OneCanRun.GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace OneCanRun.UI
{
    public class PlayerHPBar : MonoBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;
        [Tooltip("Image component when healthing")]
        public Image Healthing;

        Health m_PlayerHealth;
        float m_LastTimeHealth;
        bool ifHealth;
        float m_LastRatio = 1;
        // Start is called before the first frame update
        void Start()
        {
            PlayerCharacterController playerCharacterController =
              GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerHPBar>(
                playerCharacterController, this);

            m_PlayerHealth = playerCharacterController.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHPBar>(m_PlayerHealth, this,
                playerCharacterController.gameObject);

        }

        // Update is called once per frame
        void Update()
        {
            HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
           if(m_LastRatio<HealthFillImage.fillAmount)
            {
                m_LastTimeHealth = Time.time;
                ifHealth = true;
            }

           if(ifHealth&& Time.time- m_LastTimeHealth > 1f)
            {
                ifHealth = false;
            }

            m_LastRatio = HealthFillImage.fillAmount;
            Healthing.gameObject.SetActive(ifHealth);
        }
    }
}
