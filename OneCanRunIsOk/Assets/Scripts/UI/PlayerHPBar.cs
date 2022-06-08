
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

        Health m_PlayerHealth;
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
            HealthFillImage.fillAmount = Mathf.Lerp(1, m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth, 10 * Time.deltaTime);
        }
    }
}
