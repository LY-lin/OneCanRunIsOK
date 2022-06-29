using OneCanRun.Game;
using OneCanRun.GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace OneCanRun.UI
{
    public class Blood : MonoBehaviour
    {
        [Tooltip("Blood Image")]
        public Image blood;
        [Tooltip("Blood Canvas Group")]
        public CanvasGroup cg;

        private Health m_PlayerHealth;
        void Start()
        {
            PlayerCharacterController playerCharacterController =
              GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerInformation>(
                playerCharacterController, this);

            m_PlayerHealth = playerCharacterController.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerInformation>(m_PlayerHealth, this,
                playerCharacterController.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            float ratio = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
            if (ratio <= 0.75f)
            {
                cg.alpha = 1-ratio / 0.75f;
            }
            else
            {
                cg.alpha = 0;
            }
        }
    }
}
