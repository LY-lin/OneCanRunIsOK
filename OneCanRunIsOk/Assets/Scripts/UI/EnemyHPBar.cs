using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OneCanRun.Game;

namespace OneCanRun.UI
{
    public class EnemyHPBar : MonoBehaviour
    {
        [Tooltip("Image showing the left health")]
        public Image HealthImage;

        [Tooltip("The floating healthbar pivot transform")]
        public Transform HealthBarPivot;

        [Tooltip("Whether the health bar is visible when at full health or not")]
        public bool HideFullHealthBar = true;

        Health m_health;
        void Start()
        {
            m_health = GetComponentInParent<Health>();
           
        }

        // Update is called once per frame
        void Update()
        {
           //Debug.Log(m_health.CurrentHealth);
            HealthImage.fillAmount = m_health.CurrentHealth / m_health.MaxHealth;
            HealthBarPivot.LookAt(Camera.main.transform.position);
            if (HideFullHealthBar)
                HealthBarPivot.gameObject.SetActive(HealthImage.fillAmount < (1 - 0.1f / m_health.MaxHealth));
        }
    }
}