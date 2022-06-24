
using OneCanRun.Game;
using OneCanRun.GamePlay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace OneCanRun.UI
{
    public class PlayerInformation : MonoBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;
        [Tooltip("Image component when healthing")]
        public Image Healthing;

        [Tooltip("Image component displaying Current exp")]
        public Image ExpFill;


        Health m_PlayerHealth;
        float m_LastTimeHealth;
        bool ifHealth;
        float m_LastRatio = 1;
        Actor actor;
        // Start is called before the first frame update
        void Start()
        {
            PlayerCharacterController playerCharacterController =
              GameObject.FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, PlayerInformation>(
                playerCharacterController, this);

            m_PlayerHealth = playerCharacterController.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerInformation>(m_PlayerHealth, this,
                playerCharacterController.gameObject);

            actor = playerCharacterController.GetComponentInParent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerInformation>(actor, this,
                playerCharacterController.gameObject);

        }

        // Update is called once per frame
        void Update()
        {
            float ratio  = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
           if(m_LastRatio<ratio)
            {
                m_LastTimeHealth = Time.time;
                ifHealth = true;
                HealthFillImage.fillAmount = Mathf.Lerp(m_LastRatio, ratio, 10 * Time.deltaTime);
            }
           else
            {
                HealthFillImage.fillAmount = Mathf.Lerp(ratio,m_LastRatio, 10 * Time.deltaTime);
            }

           if(ifHealth&& Time.time- m_LastTimeHealth > 1f)
            {
                ifHealth = false;
            }

            ExpFill.fillAmount = actor.getExperience() / actor.getNextLevelCount();
            m_LastRatio = HealthFillImage.fillAmount;
            Healthing.gameObject.SetActive(ifHealth);
        }
    }
}
