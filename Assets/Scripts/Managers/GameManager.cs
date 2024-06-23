using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    [SerializeField] private int columns, rows;

    private Board currentPlayingBoard;

    private void Start()
    {
        currentPlayingBoard = new Board(columns, rows);
    }
    
    public void StartGame()
    {
        currentPlayingBoard.InitalizeBoard();
        Debug.LogError("Started game!");
    }

    [ContextMenu("ColumnTesting")]
    public void TestColumnStuff()
    {
        int row = currentPlayingBoard.GetFirstAvaliableRow(6);
        Debug.LogError($"The first avaliable row was: {row}");
    }
    
    [ContextMenu("Print Board")]
    public void TestPrintBoard()
    {
        currentPlayingBoard.PrintBoard();
    }

}
