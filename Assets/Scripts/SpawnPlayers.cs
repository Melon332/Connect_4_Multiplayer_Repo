using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : NetworkBehaviour
{
    /*
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (var client in clientscompleted)
        {
            PlayerManager player = Instantiate()
        }
    }

    private void SpawnPlayer()
    {
        
    }
    */
}
