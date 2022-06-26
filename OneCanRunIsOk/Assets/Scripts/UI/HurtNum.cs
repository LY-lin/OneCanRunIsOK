using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OneCanRun.Game.Share;
using OneCanRun.GamePlay;
using OneCanRun.Game;

namespace OneCanRun.UI
{
    public class HurtNum : MonoBehaviour
    {
        [Tooltip("text")]
        public TextMeshProUGUI mText;

        [Tooltip("life time")]
        public float lifeTime = 3f;

        private string hurt;
        private DamageType type;
        private RectTransform plane;
        private GameObject obj;
        private Vector2 RandomPos;
        private float radis;
        private float BaseDis = 10;
        private Vector3 LastPosition;
        private float initTime;
        public void init(GameObject obj,float damage, DamageType _type)
        {
            hurt = damage.ToString();
            type = _type;
            if (type == DamageType.magic)
                mText.color = new Color(0,127, 255);
            else
                mText.color = new Color(255, 255, 0);
            plane = this.transform.parent.GetComponent<RectTransform>();
            this.obj = obj;
            radis = Screen.width / 20;
            float x = Random.Range(0 - radis, radis);
            float y = Mathf.Sqrt(radis * radis - x * x);
            RandomPos = new Vector2(x, y);
            initTime = Time.time;
        }


        private void Update()
        {
            if (obj&&obj.activeSelf)
            {
                Debug.Log(obj.transform.parent.GetComponent<Health>() == null);
                if (obj.transform.parent.GetComponent<Health>().CurrentHealth != 0)
                {
                    transform.localPosition = GetUIPosition(obj.transform.position);
                    LastPosition = obj.transform.position;
                }
                else if (obj.transform.parent.GetComponent<Health>().CurrentHealth == 0)
                {
                    transform.localPosition = GetUIPosition(LastPosition);
                }
            }
            else
            {
                HurtNumberHudManage.poolManager.release(this.gameObject);
            }

            if (Time.time - initTime >= lifeTime)
                HurtNumberHudManage.poolManager.release(this.gameObject);
        }

        private void Start()
        {
            if (mText)
                mText.text = hurt.ToString();
            else
            {
                HurtNumberHudManage.poolManager.release(this.gameObject);
                return;
            }
        }

        private Vector3 GetUIPosition(Vector3 point)
        {
            Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main,point);
            float distance = Vector3.Distance(Camera.main.transform.position, point);
            float Base = BaseDis / distance;
            position += (RandomPos * Base);
            mText.transform.localScale = (Base>1?1:Base)*Vector3.one;
            if (position.x < -100 || position.y < -100 || position.x > Screen.width+100 || position.y > Screen.height+100)
                HurtNumberHudManage.poolManager.release(this.gameObject);
            Vector2 uiPosition;
           
            RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, position, null, out uiPosition);
            return uiPosition;
        }
    }
}
