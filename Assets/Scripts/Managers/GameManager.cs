using System;
using System.Collections;
using System.Collections.Generic;
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
        currentPlayingBoard.PrintBoard();
    }

}
