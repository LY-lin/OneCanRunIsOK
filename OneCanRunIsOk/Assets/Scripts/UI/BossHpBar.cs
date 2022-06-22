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
        void Start()
        {
            boss = FindObjectOfType<Boss>();
            DebugUtility.HandleErrorIfNullFindObject<Boss,BossHpBar>(boss, this);
            boss_health = boss.health;
            Name.text = boss.name;
            for(int i=0;i<Num;i++)
            {
                GameObject hpInstance = Instantiate(HpBar.gameObject, HpPlane);
                Image newHpBar = hpInstance.GetComponent<Image>();
                DebugUtility.HandleErrorIfNullGetComponent<BuffUI, Backpack>(newHpBar,
                    this, hpInstance.gameObject);
                
                listHp.Add(newHpBar);
                int index = i % 4;
                switch(index)
                {
                    case 0:
                        newHpBar.color = Color.red;
                        break;
                    case 1:
                        newHpBar.color = Color.blue;
                        break;
                    case 2:
                        newHpBar.color = new Color(127, 0, 255, 1);
                        break;
                    case 3:
                        newHpBar.color = new Color(0, 127, 0, 1);
                        break;
                }
            }
            Plane.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(boss.GetCG())
            {
                Plane.gameObject.SetActive(true);
                float ratio = boss_health.CurrentHealth / boss_health.MaxHealth;
                Debug.Log(ratio / (1f / Num));
                int index = Mathf.FloorToInt(ratio / (1f / Num));
                if (index == Num)
                    index = Num - 1;
                listHp[index].fillAmount = (ratio - (index * (1f / Num)) )* Num;
                Debug.Log(index + "   " + (1f/Num)+"  " + (ratio - (index * (1f / Num))));
                for(int i= Num-1;i>index;i--)
                {
                    listHp[i].fillAmount = 0;
                }
                HpRat.text = boss_health.CurrentHealth.ToString() + "/" + boss_health.MaxHealth.ToString();
            }
        }
    }
}