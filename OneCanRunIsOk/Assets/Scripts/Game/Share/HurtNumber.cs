using UnityEngine;
using TMPro;
namespace OneCanRun.Game.Share
{

    public enum DamageType
    {
        physical = 0,
        magic = 1
    }
    public class HurtNumber : MonoBehaviour{
    

        [Tooltip("text")]
        public TextMeshProUGUI mText;
        [Tooltip("life time")]
        public float lifeTime = 3f;
        private string hurt;
        private DamageType type;

        // create Time 
        public float time;
        

        public void init(float damage, DamageType _type){
            hurt = damage.ToString();
            type = _type;
            if (type == DamageType.magic)
                mText.color = new Color(0, 0, 255);
            else
                mText.color = new Color(255, 255, 0);
        }


        private void Update(){
            if(Time.time - time >= lifeTime){
                // release the object into pool
                //HurtNumberPoolManager.instance.release(this.gameObject);
            }
            transform.LookAt(Camera.main.transform);
            transform.forward = -transform.forward;
        }

        private void Start(){
            //mText = GetComponent<UnityEngine.TextMesh>(
            transform.LookAt(Camera.main.transform);
            transform.forward = -transform.forward;
            if (mText)
                mText.text = hurt;
            else{

                //Destroy(this.gameObject);
                return;
            }
            //Destroy(this.gameObject, lifeTime);
        }



    }
}
