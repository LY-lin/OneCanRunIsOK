using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun
{
    public class MyLoadingScene : MonoBehaviour
    {
        public UnityEngine.UI.Slider slider;


        public void Start()
        {
            loadNextScene();
        }

        public void loadNextScene(){
            StartCoroutine(loadScene());

        }

        IEnumerator loadScene(){

            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            slider.value = 0.2f;
            operation.allowSceneActivation = false;

            while (!operation.isDone){

                slider.value = operation.progress;

                if(operation.progress >= 0.9f){
                    slider.value = 1.0f;
                    operation.allowSceneActivation = true;
                }

                yield return null;

            }

        }
    }
}
