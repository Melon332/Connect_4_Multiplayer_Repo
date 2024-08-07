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
    public bool IsDroppingTile { get; set; }
    public static GameManager Instance { get; set; }

    [Header("Board and tile data variables")]
    [SerializeField] private GameObject currentSelectedPlayBoard;
    [SerializeField] private List<GameObject> bricks = new List<GameObject>();
    [SerializeField] private float offsetY;
    [SerializeField] private int columns, rows;
    [SerializeField] private bool devMode;
    [SerializeField] private float tileMoveSpeed;
    [SerializeField] private List<GameObject> phantomTiles;
    [SerializeField] private float yPosPhantomOffset = 0.3f;

    [Header("UI timing variables")] 
    [SerializeField] private float secondsToShowTurnText = 2.0f;

    private List<PlayerManager> players = new List<PlayerManager>();
    private List<GameObject> spawnedTiles = new List<GameObject>();
    
    private GameUIManager gameUIManager;
    private Board currentPlayingBoard;
    private VisualBoard visualBoard;

    private int winnerID;
    private GameObject tileToLerp;
    private float yPos;
    private float sinTime;
    public bool gameOver { private set; get; }
    
    private void Awake()
    {
        Instance = this;
        gameUIManager = FindObjectOfType<GameUIManager>();
    }

    private void FixedUpdate()
    {
        if (tileToLerp == null) return;
        LerpTile();
    }

    private void LerpTile()
    {
        if (!Mathf.Approximately(tileToLerp.transform.position.y,yPos))
        {
            sinTime += Time.deltaTime * tileMoveSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = Evaluate(sinTime);
            float lerpPos = Mathf.Lerp(tileToLerp.transform.position.y, yPos, t);
            tileToLerp.transform.position =
                new Vector3(tileToLerp.transform.position.x, lerpPos, tileToLerp.transform.position.z);
            IsDroppingTile = true;
        }
        else
        {
            IsDroppingTile = false;
            tileToLerp = null;
            sinTime = 0;
        }
    }

    private float Evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2) + 0.5f;
    }

    private void StartGame()
    {
        currentPlayingBoard = new Board(columns, rows);
        currentPlayingBoard.InitalizeBoard();
        visualBoard = Instantiate(currentSelectedPlayBoard, Vector3.zero, Quaternion.identity).GetComponent<VisualBoard>();
        gameUIManager.InitalizePlayerScoreText(players);
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

        spawnedTiles.Add(tile);
        tileToLerp = tile;
        yPos = tile.transform.position.y - offset;
    }

    public void SetPhantomTilePosition(int playerIndex, Transform rowObject)
    {
        GameObject tile = phantomTiles[playerIndex];
        TogglePhantomTile(playerIndex, true);
        tile.transform.position = new Vector3(rowObject.transform.position.x,
            rowObject.transform.position.y + yPosPhantomOffset, rowObject.transform.position.z);
    }

    public void TogglePhantomTile(int playerIndex, bool toggle)
    {
        GameObject tile = phantomTiles[playerIndex];
        if (tile.activeInHierarchy == toggle) return;
        tile.SetActive(toggle);
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
        gameOver = true;
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
        gameOver = false;
        currentPlayingBoard.ResetBoard();
        ResetAllTiles();
        InitalizeUI();
        sinTime = 0;
        IsDroppingTile = false;
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
       GetOwningPlayer().IsPaused = toggle;
    }

    private PlayerManager GetOwningPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].IsOwner)
            {
                return players[i];
            }
        }

        return null;
    }
    
    private void SteamMatchmakingOnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        ShutdownConnection();
        SceneLoaderManager.Instance.LoadScene("MainMenu");
        CharacterCustomizationManager.Instance.ClearCustomizations();
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
