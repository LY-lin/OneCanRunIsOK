using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneCanRun.Game
{
    public class LoadingHelper
    {
    public static LoadingHelper Instance = new LoadingHelper();

    private const string LoadingSceneName = "LoadingScene";
    private const string DefaultSceneName = "StartMune";

    private bool isLoading = false;
    private string nextSceneName = null;

    public void LoadScene(string sceneName, Dictionary<string, object> sceneOneshotData = null)
    {
        if (isLoading)
        {
            Debug.LogError("The last one was still being doing.");
            return;
        }

        this.isLoading = true;
        this.nextSceneName = string.IsNullOrWhiteSpace(sceneName) ? DefaultSceneName : sceneName;

        SceneOneshotDataManager.Instance.WriteSceneData(sceneOneshotData);
        SceneManager.LoadScene(LoadingSceneName, LoadSceneMode.Single);
    }

    public string GetNextSceneName()
    {
        return this.nextSceneName;
    }

    public void FinishLoading()
    {
        this.nextSceneName = null;
        this.isLoading = false;
    }
}
}
