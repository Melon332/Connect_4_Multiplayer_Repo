using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    public Camera cameraMain { private get; set; }
    public int PlayerID { get; private set; }
    public bool IsMyTurn { get; set; }
    
    public string PlayerName { get; set; }
    
    public bool IsPaused { get; set; }
    
    public int Wins { get; set; }

    private bool inGame = false;

    private RowNumber currentHoveringRow;

    private Action<int> SetTileAction;

    private CharacterContainer ownedCharacterContainer;
    private InputManager inputManager;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            inputManager = new InputManager(GetComponent<PlayerInput>());
            inputManager.OnLeftMouseClickAction += MouseClick;
            inputManager.OnPauseButtonPressedAction += PauseGame;
            inputManager.OnToggleChatBoxButtonPressedAction += ToggleChatBox;
            inputManager.OnMouseMoveAction += HoveringOverRow;
            inputManager.EnableInput();
            
        }
        PlayerID = (int)OwnerClientId;
        if (PlayerID == 0)
        {
            IsMyTurn = true;
        }
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
        IsPaused = false;
        SetTileAction += IsHost ? ServerSetTileOnColumnRpc : ClientSetTileOnColumnRpc;
        ownedCharacterContainer = CharacterCustomizationManager.Instance.GetCharacterContainer();
        if (!NetworkManager.Singleton.IsHost && IsOwner)
        {
            ClientJoinedRpc();
            SendCustomizationDataToServerRpc(ownedCharacterContainer.currentTorso,
                ownedCharacterContainer.currentHead, ownedCharacterContainer.currentEars, ownedCharacterContainer.currentEyes);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnLoadEventCompleted;
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (scenename == "Gameplay")
        {
            GameManager.Instance.AddPlayer(this);
            if (!DeveloperMode.LocalMode)
            {
                PlayerName = LobbySaver.CurrentLobby.Members.ToList()[PlayerID].Name;
            }
            inGame = true;
            cameraMain = FindObjectOfType<Camera>();
        }
    }

    private void ToggleChatBox()
    {
        if (!IsOwner) return;
        UIManager.Instance.ToggleChatBox();
    }
    
    private void PauseGame()
    {
        if (!inGame) return;
        GameManager.Instance.PauseGame(!IsPaused);
    }

    [Rpc(SendTo.Server)]
    private void ClientJoinedRpc()
    {
        UIManager.Instance.ToggleStartGameButton();
    }

    [Rpc(SendTo.Server)]
    private void SendCustomizationDataToServerRpc(int torso, int head, int ears, int eyes)
    {
        CharacterCustomizationManager.Instance.AddNewCustomization(torso, head, ears, eyes);
        SendCustomizationDataToClientsRpc(ownedCharacterContainer.currentTorso,
            ownedCharacterContainer.currentHead,
            ownedCharacterContainer.currentEars,
            ownedCharacterContainer.currentEyes);
    }

    [Rpc(SendTo.NotServer)]
    private void SendCustomizationDataToClientsRpc(int torso, int head, int ears, int eyes)
    {
        CharacterCustomizationManager.Instance.AddNewCustomization(torso,
            head,
            ears,
            eyes);
    }

    private void HoveringOverRow(Vector2 mousePos)
    {
        if (!inGame) return;
        Ray ray = cameraMain.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            RowNumber hoveringRow = hit.collider.GetComponent<RowNumber>();
            if (hoveringRow)
            {
                currentHoveringRow = hoveringRow;
                GameManager.Instance.SetPhantomTilePosition(PlayerID, hoveringRow.transform);
            }
        }
        else
        {
            currentHoveringRow = null;
            GameManager.Instance.TogglePhantomTile(PlayerID, false);
        }
    }

    private void MouseClick()
    {
        if (!inGame) return;
        if (!IsMyTurn || IsPaused || GameManager.Instance.IsDroppingTile || GameManager.Instance.gameOver) return;
        if (currentHoveringRow)
        {
            SetTileAction?.Invoke(currentHoveringRow.number);
        }
    }
    
    [Rpc(SendTo.Server)]
    private void ClientSetTileOnColumnRpc(int column)
    {
        ServerSetTileOnColumnRpc(column);
    }

    [Rpc(SendTo.Everyone)]
    private void ServerSetTileOnColumnRpc(int column)
    {
        GameManager.Instance.SetTile(column, PlayerID + 1);
    }
}
