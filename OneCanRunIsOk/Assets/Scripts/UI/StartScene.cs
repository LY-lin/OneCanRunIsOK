using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OneCanRun.Game;

public class StartScene : MonoBehaviour{

    float time = 5f;
    bool load = false;
    public void startGame()
    {

        LoadingHelper.Instance.LoadScene("StarMenu");
    }
    public void loadGame(){
        
        LoadingHelper.Instance.LoadScene("SampleScene");
    }

}
