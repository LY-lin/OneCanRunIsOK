using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour{

    float time = 5f;
    bool load = false;
    public void loadGame(){
        SceneManager.LoadScene(1);

    }

}
