using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; set; }

    [Header("Board and tile data variables")]
    [SerializeField] private GameObject currentSelectedPlayBoard;
    [SerializeField] private List<GameObject> bricks = new List<GameObject>();
    [SerializeField] private float offsetY;
    [SerializeField] private int columns, rows;
    [SerializeField] private bool devMode;

    [Header("UI timing variables")] 
    [SerializeField] private float secondsToShowTurnText = 2.0f;

    private List<PlayerManager> players = new List<PlayerManager>();
    private List<GameObject> spawnedTiles = new List<GameObject>();
    
    private GameUIManager gameUIManager;
    private Board currentPlayingBoard;
    private VisualBoard visualBoard;

    private int winnerID;
    
    private void Awake()
    {
        Instance = this;
        gameUIManager = FindObjectOfType<GameUIManager>();
    }
    
    private void StartGame()
    {
        currentPlayingBoard = new Board(columns, rows);
        currentPlayingBoard.InitalizeBoard();
        visualBoard = Instantiate(currentSelectedPlayBoard, Vector3.zero, Quaternion.identity).GetComponent<VisualBoard>();
        gameUIManager.InitalizePlayerScoreText(players);
        Debug.LogError("Started game!");
    }

    public void AddPlayer(PlayerManager player)
    {
        players.Add(player);
    }

    private void SwitchTurns()
    {
        if (devMode)
        {
            return;
        }
        foreach (var player in players)
        {
            player.IsMyTurn = !player.IsMyTurn;
            if (player.IsMyTurn && player.IsOwner)
            {
                ShowTurnText();
            }
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
        SwitchTurns();
        
        SpawnTile(column, row, playerIndex);
        GameResult(playerIndex);
    }

    private void SpawnTile(int column, int row, int playerIndex)
    {
        Transform col = visualBoard.GetColumnTransform(column);
        GameObject tile = Instantiate(bricks[playerIndex - 1], col.transform.position, Quaternion.identity, col);
        float offset = offsetY * row;
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y - offset, tile.transform.position.z);
        spawnedTiles.Add(tile);
    }

    private void GameResult(int playerIndex)
    {
        //Game result checks here
        bool tie = currentPlayingBoard.CheckIfBoardIsFull();
        if (tie)
        {
            EndGame(true);
            return;
        }
        
        var result = CheckForWin(playerIndex);
        if (result)
        {
            EndGame(false, playerIndex);
        }
    }

    private void EndGame(bool tie, int playerIndex = -1)
    {
        gameUIManager.ToggleEndGamePanel(true);
        gameUIManager.ToggleHUDPanel(false);
        if (tie)
        {
            //TODO: Add tie logic here
            return;
        }

        int playerIndexZeroed = playerIndex - 1;
        gameUIManager.SetWinnerText(players[playerIndexZeroed]);
        GiveWinnerFirstTurn(playerIndexZeroed);
        winnerID = playerIndexZeroed;
        players[winnerID].Wins += 1;
        //TODO: Add end game logic here
    }

    private void InitalizeUI()
    {
        gameUIManager.ToggleEndGamePanel(false);
        gameUIManager.ToggleHUDPanel(true);
        gameUIManager.UpdatePlayerScoreText(players);
        if (players[winnerID].IsOwner)
        {
            gameUIManager.ShowTurnText(secondsToShowTurnText);
        }
    }

    private bool CheckForWin(int playerIndex)
    {
        return currentPlayingBoard.CheckForWin(playerIndex);
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (clientscompleted.Count < 2 && !devMode) return;
        StartGame();
        gameUIManager.ToggleWaitingForPlayersPanel(false);
        gameUIManager.ToggleHUDPanel(true);
    }

    public void ShowTurnText()
    {
        gameUIManager.ShowTurnText(secondsToShowTurnText);
    }

    [Rpc(SendTo.Everyone)]
    public void ResetBoardRpc()
    {
        currentPlayingBoard.ResetBoard();
        ResetAllTiles();
        InitalizeUI();
        Debug.Log("Game restarted!");
    }

    private void GiveWinnerFirstTurn(int winnerPlayerIndex)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].PlayerID == winnerPlayerIndex)
            {
                players[i].IsMyTurn = true;
            }
            else
            {
                players[i].IsMyTurn = false;
            }
        }
    }

    public void ResetAllTiles()
    {
        foreach (var tiles in spawnedTiles)
        {
            Destroy(tiles);
        }
        spawnedTiles.Clear();
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
