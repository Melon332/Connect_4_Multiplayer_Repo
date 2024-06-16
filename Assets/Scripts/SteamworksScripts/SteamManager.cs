using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public class SteamManager : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.SetHostLobbyButtonMethod(HostLobby);
        UIManager.Instance.SetJoinLobbyButtonMethod(JoinLobbyWithID);
        UIManager.Instance.SetReturnToMenuButtonMethod(LeaveLobby);
    }

    private async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(2);
    }

    private async void JoinLobbyWithID()
    {
        ulong ID;
        if (!ulong.TryParse(UIManager.Instance.GetLobbyIDText(), out ID))
        {
            return;
        }
        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();

        foreach (var lobby in lobbies)
        {
            if (lobby.Id == ID)
            {
                await lobby.Join();
                return;
            }
        }
    }
    
    private void LeaveLobby()
    {
        LobbySaver.CurrentLobby?.Leave();
        UIManager.Instance.LeftLobby("");
        LobbySaver.CurrentLobby = null;
    }
    
    private async void SteamFriendsOnGameLobbyJoinRequested(Lobby lobby, SteamId steamID)
    {
        await lobby.Join();
    }

    private void SteamMatchmakingOnLobbyEntered(Lobby lobby)
    {
        LobbySaver.CurrentLobby = lobby;
        Debug.Log($"We've entered the lobby with id: {lobby.Id.ToString()}");
        UIManager.Instance.JoinedLobby(lobby.Id.ToString());
    }

    private void SteamMatchmakingOnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK) return;
        lobby.SetPublic();
        lobby.SetJoinable(true);
    }

    private void OnEnable()
    {
       SteamMatchmaking.OnLobbyCreated += SteamMatchmakingOnLobbyCreated;
       SteamMatchmaking.OnLobbyEntered += SteamMatchmakingOnLobbyEntered;
       SteamFriends.OnGameLobbyJoinRequested += SteamFriendsOnGameLobbyJoinRequested;
    }
    
    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmakingOnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmakingOnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriendsOnGameLobbyJoinRequested;
    }
}
