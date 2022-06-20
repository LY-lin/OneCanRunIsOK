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

        [Tooltip("in which distance to show hp")]
        public float distance = 50;

        Health m_health;
        GameObject player;
        void Start()
        {
            m_health = GetComponentInParent<Health>();
            player = GameObject.Find("Player1");
            DebugUtility.HandleErrorIfNullFindObject<GameObject, Backpack>(player, this);
            if (HideFullHealthBar)
                HealthBarPivot.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
           //Debug.Log(m_health.CurrentHealth);
            HealthImage.fillAmount = m_health.CurrentHealth / m_health.MaxHealth;
            HealthBarPivot.LookAt(Camera.main.transform.position);
            Vector3 position = player.transform.position;
            //bool ifnear = Vector3.Distance(position, this.gameObject.transform.position) < distance;
            bool ifnear = (position - this.gameObject.transform.position).magnitude<distance;
            
            if (HideFullHealthBar)
                    HealthBarPivot.gameObject.SetActive(ifnear&&HealthImage.fillAmount < (1 - 0.1f / m_health.MaxHealth));
       }
    }
}