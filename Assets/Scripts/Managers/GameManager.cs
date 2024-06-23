using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    [SerializeField] private int columns, rows;
    [SerializeField] private bool devMode;

    private List<PlayerManager> players = new List<PlayerManager>();
    
    private GameUIManager gameUIManager;
    private Board currentPlayingBoard;
    
    protected override void Awake()
    {
        base.Awake();
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
        foreach (var player in players)
        {
            player.IsMyTurn = !player.IsMyTurn;
        }
    }

    public void SetTile(int column, int playerIndex)
    {
        int row = currentPlayingBoard.GetFirstAvaliableRow(column);
        currentPlayingBoard.SetTile(row, column, playerIndex);
        Debug.Log("Added tile........... NEW BOARD");
        TestPrintBoard();
        SwitchTurns();
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

    private void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
    }
}
