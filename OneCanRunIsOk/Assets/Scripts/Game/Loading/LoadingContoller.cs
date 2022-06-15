
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OneCanRun.Game;
namespace OneCanRun
{
    public class LoadingContoller : MonoBehaviour
    {
        // Start is called before the first frame update
        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var nextSceneName = LoadingHelper.Instance.GetNextSceneName();
            StartCoroutine(loadScene(nextSceneName));
        }

        private IEnumerator loadScene(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
