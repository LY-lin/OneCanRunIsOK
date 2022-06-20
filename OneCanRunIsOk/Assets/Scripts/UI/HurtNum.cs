using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OneCanRun.Game.Share;
using OneCanRun.GamePlay;

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
        public void init(GameObject obj,float damage, DamageType _type)
        {
            hurt = damage.ToString();
            type = _type;
            if (type == DamageType.magic)
                mText.color = new Color(0, 0, 255);
            else
                mText.color = new Color(255, 255, 0);
            plane = this.transform.parent.GetComponent<RectTransform>();
                this.obj = obj;
            
        }


        private void Update()
        {
            
            if (obj)
            {
                
                if (obj.activeSelf)
                    transform.localPosition = GetUIPosition(obj.transform.position);
                else
                    Destroy(this);
            }
            else
                Destroy(this);
        }

        private void Start()
        {

            /*transform.LookAt(Camera.main.transform);
            transform.forward = -transform.forward;
            */
            if (mText)
                mText.text = hurt.ToString();
            else
            {
                Destroy(this.gameObject);
                return;
            }
            Destroy(this.gameObject, lifeTime);
        }

        private Vector3 GetUIPosition(Vector3 point)
        {
            Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main,point);
            if (position.x < -100 || position.y < -100 || position.x > Screen.width+100 || position.y > Screen.height+100)
                Destroy(this);
            Vector2 uiPosition;
           
            RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, position, null, out uiPosition);
            return uiPosition;
        }
    }
}
