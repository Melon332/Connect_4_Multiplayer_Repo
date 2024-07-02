using System;
using System.Collections;
using System.Collections.Generic;
using Netcode.Transports.Facepunch;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SteamManager : MonoBehaviour
{
    public async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(2);
    }

    public async void JoinLobbyWithID()
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
    
    public void LeaveLobby()
    {
        LobbySaver.CurrentLobby.Leave();
        UIManager.Instance.LeftLobby("");
        NetworkManager.Singleton.Shutdown();
        CharacterCustomizationManager.Instance.ClearCustomizations();
    }
    
    private async void SteamFriendsOnGameLobbyJoinRequested(Lobby lobby, SteamId steamID)
    {
        await lobby.Join();
    }

    private void SteamMatchmakingOnLobbyEntered(Lobby lobby)
    {
        LobbySaver.CurrentLobby = lobby;
        UIManager.Instance.JoinedLobby(lobby.Id.ToString());
        LobbySaver.CurrentLobby.SendChatString("has joined the match!");
        CharacterCustomizationManager.Instance.AddLocalCustomization();
        
        if(NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
        NetworkManager.Singleton.StartClient();
        UIManager.Instance.ToggleStartGameButton(false);
        UIManager.Instance.ToggleWaitingForPlayerText(false);
    }

    private void SteamMatchmakingOnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK) return;
        lobby.SetPublic();
        lobby.SetJoinable(true);
        NetworkManager.Singleton.StartHost();
        UIManager.Instance.ToggleStartGameButton(false);
        UIManager.Instance.ToggleWaitingForPlayerText(true);
    }
    
    private void SteamMatchmakingOnChatMessage(Lobby lobby, Friend sender, string message)
    {
        if (message.Contains("left"))
        {
            UIManager.Instance.DelegateMessage(MessageType.System, message, "SYSTEM:");
            return;
        }

        if (message.Contains("joined"))
        {
            UIManager.Instance.DelegateMessage(MessageType.System, message, $"SYSTEM: {sender.Name}");
            return;
        }
        UIManager.Instance.DelegateMessage(MessageType.Player, message, sender.Name);
    }
    
    private void SteamMatchmakingOnLobbyMemberLeave(Lobby lobby, Friend user)
    {
        LeaveLobby();
    }

    public void ServerStartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        SceneLoaderManager.Instance.LoadSceneNet("Gameplay");
    }

    private void OnEnable()
    {
       SteamMatchmaking.OnLobbyCreated += SteamMatchmakingOnLobbyCreated;
       SteamMatchmaking.OnLobbyEntered += SteamMatchmakingOnLobbyEntered;
       SteamMatchmaking.OnChatMessage += SteamMatchmakingOnChatMessage;
       SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmakingOnLobbyMemberLeave;
       SteamFriends.OnGameLobbyJoinRequested += SteamFriendsOnGameLobbyJoinRequested;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmakingOnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmakingOnLobbyEntered;
        SteamMatchmaking.OnChatMessage -= SteamMatchmakingOnChatMessage;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmakingOnLobbyMemberLeave;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriendsOnGameLobbyJoinRequested;
    }
}
