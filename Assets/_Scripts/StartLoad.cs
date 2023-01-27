using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLoad : MonoBehaviour
{
    private string sceneToLoad;

    private void Start()
    {
        if(SaveSystem.LoadGameRecord() == null)
        {
            sceneToLoad = "TutorialScene";
        }
        else
        {
            sceneToLoad = "SampleScene";
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
