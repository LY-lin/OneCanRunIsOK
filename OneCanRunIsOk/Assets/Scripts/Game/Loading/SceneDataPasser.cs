using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneCanRun.Game
{

}
public class SceneOneshotDataManager
{
    public static SceneOneshotDataManager Instance = new SceneOneshotDataManager();

    Dictionary<string, object> sceneOneshotData = null;

    public bool Exist()
    {
        return sceneOneshotData != null;
    }

    public bool WriteSceneData(Dictionary<string, object> data)
    {
        if (this.sceneOneshotData != null)
        {
            Debug.LogError("The last data was not used.");
            return false;
        }
        this.sceneOneshotData = data;

        return true;
    }

    public Dictionary<string, object> ReadSceneData()
    {
        Dictionary<string, object> result = sceneOneshotData;
        sceneOneshotData = null;
        return result;
    }
}
