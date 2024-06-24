using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    
    [SerializeField] private int columns, rows;
    [SerializeField] private bool devMode;

    private List<PlayerManager> players = new List<PlayerManager>();
    
    private GameUIManager gameUIManager;
    private Board currentPlayingBoard;
    
    private void Awake()
    {
        Instance = this;
        gameUIManager = FindObjectOfType<GameUIManager>();
    }
    
    private void StartGame()
    {
        currentPlayingBoard = new Board(columns, rows);
        currentPlayingBoard.InitalizeBoard();
        Debug.LogError("Started game!");
    }

    public void AddPlayer(PlayerManager player)
    {
        players.Add(player);
    }

    private void SwitchTurns()
    {
        if (devMode) return;
        foreach (var player in players)
        {
            player.IsMyTurn = !player.IsMyTurn;
        }
    }

    public void SetTile(int column, int playerIndex)
    {
        int row = currentPlayingBoard.GetFirstAvaliableRow(column);
        if (row == -1)
        {
            return;
        }
        currentPlayingBoard.SetTile(row, column, playerIndex);
        TestPrintBoard();
        SwitchTurns();
        bool result = currentPlayingBoard.CheckForWin(playerIndex);
        //Debug.LogError(result);
        if (result)
        {
            Debug.Log($"Player {playerIndex} won!");
        }
    }
    
    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (clientscompleted.Count < 2 && !devMode) return;
        StartGame();
        gameUIManager.ToggleWaitingForPlayersPanel(false);
    }

    [ContextMenu("ColumnTesting")]
    public void TestColumnStuff()
    {
        int row = currentPlayingBoard.GetFirstAvaliableRow(0);
        Debug.LogError($"The first avaliable row was: {row}");
    }
    
    [ContextMenu("Print Board")]
    public void TestPrintBoard()
    {
        currentPlayingBoard.PrintBoard();
    }

    public void ReturnToMenu()
    {
        LobbySaver.CurrentLobby.Leave();
        ShutdownConnection();
        SceneLoaderManager.Instance.LoadScene("MainMenu");
    }

    public void PauseGame(bool toggle)
    {
       gameUIManager.TogglePauseMenu(toggle); 
    }
    
    private void SteamMatchmakingOnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        ShutdownConnection();
        SceneLoaderManager.Instance.LoadScene("MainMenu");
    }

    private void ShutdownConnection()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmakingOnLobbyMemberLeave;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmakingOnLobbyMemberLeave;
    }
}
