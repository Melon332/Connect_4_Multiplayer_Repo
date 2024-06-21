using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviourSingletonPersistent<SceneLoaderManager>
{
    private void Start()
    {
        StartCoroutine(LoadMainMenuScene());
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    
    public void LoadSceneNet(string name)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    IEnumerator LoadMainMenuScene()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        LoadScene("MainMenu");
    }
}
