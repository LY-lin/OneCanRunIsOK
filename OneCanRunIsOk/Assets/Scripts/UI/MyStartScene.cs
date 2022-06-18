using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStartScene : MonoBehaviour
{

    private void Update()
    {
        int a = 9;
    }

    public void startGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
        
    }

    public void quitGame(){

        Application.Quit();
    }
}
