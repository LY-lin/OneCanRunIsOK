using OneCanRun.AI.Enemies;
using OneCanRun.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OneCanRun.UI
{
    public class BossHpBar : MonoBehaviour
    {
        [Tooltip("Hp Bar")]
        public GameObject HpBar;

        [Tooltip("Num of Hp Bar")]
        [Range(1,20)]
        public int Num;

        [Tooltip("Plane to Manage HP")]
        public Transform HpPlane;

        [Tooltip("Plane to Manage All")]
        public Transform Plane;
        [Tooltip("Boss's Name")]
        public TextMeshProUGUI Name;

        [Tooltip("Boss hp ratio")]
        public TextMeshProUGUI HpRat;
        List<Image> listHp = new List<Image>();
        Boss boss;
        Health boss_health;
        float lastHp;
        void Start()
        {
            boss = FindObjectOfType<Boss>();
            DebugUtility.HandleErrorIfNullFindObject<Boss,BossHpBar>(boss, this);
            boss_health = boss.health;
            Name.text = boss.BossName;
            for(int i=0;i<Num;i++)
            {
                GameObject hpInstance = Instantiate(HpBar.gameObject, HpPlane);
                Image newHpBar = hpInstance.GetComponent<Image>();
                DebugUtility.HandleErrorIfNullGetComponent<BuffUI, Backpack>(newHpBar,
                    this, hpInstance.gameObject);
                lastHp = boss_health.MaxHealth;
                listHp.Add(newHpBar);
                int index = i % 4;
                //四种颜色轮换
                switch(index)
                {
                    case 0:
                        newHpBar.color = Color.red;
                        break;
                    case 1:
                        newHpBar.color = Color.blue;
                        break;
                    case 2:
                        newHpBar.color = new Color(255, 128, 0, 1);
                        break;
                    case 3:
                        newHpBar.color = new Color(0, 80, 0, 1);
                        break;
                }
            }
            Plane.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(boss && boss.GetCG())
            {
                Plane.gameObject.SetActive(true);
                float ratio = boss_health.CurrentHealth / boss_health.MaxHealth;
                
                int index = Mathf.FloorToInt(ratio / (1f / Num));
                if (ratio%(1f/Num)==0)
                    index = index - 1;
                if (ratio == 0)
                {
                    index = 0;
                    /*
                    Plane.gameObject.SetActive(false);
                    return;
                    */
                }

                listHp[index].fillAmount = (ratio - (index * (1f / Num)) )* Num;

                if(lastHp<boss_health.CurrentHealth)
                {
                    for (int i = Num - 1; i > index; i--)
                    {
                        listHp[i].fillAmount = 0;
                    }
                }
                else if(lastHp>boss_health.CurrentHealth)
                {
                    for (int i = 0; i < index; i++)
                    {
                        listHp[i].fillAmount = 1;
                    }
                }
                HpRat.text = boss_health.CurrentHealth.ToString() + "/" + boss_health.MaxHealth.ToString();
            }

        }
    }
}