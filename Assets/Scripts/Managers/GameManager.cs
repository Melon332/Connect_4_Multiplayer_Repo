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
    private GameUIManager gameUIManager;


    private Board currentPlayingBoard;
    public override void Awake()
    {
        base.Awake();
        gameUIManager = FindObjectOfType<GameUIManager>();
    }

    private void Start()
    {
        currentPlayingBoard = new Board(columns, rows);
    }
    
    public void StartGame()
    {
        currentPlayingBoard.InitalizeBoard();
        Debug.LogError("Started game!");
    }
    
    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (clientscompleted.Count >= 2 || devMode)
        {
            gameUIManager.ToggleWaitingForPlayersPanel(false);
        }
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
