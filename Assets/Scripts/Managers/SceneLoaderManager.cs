using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviourSingletonPersistent<SceneLoaderManager>
{
    private void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    public void LoadUISceneAdditive()
    {
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
        LoadUISceneAdditive();
    }

    IEnumerator LoadMainScene()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        LoadScene("MainMenu");
    }
}
