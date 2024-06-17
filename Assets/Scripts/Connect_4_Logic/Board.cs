using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private List<int[]> gameBoard;

    private int rows, columns;

    public Board(int cols, int rows)
    {
        gameBoard = new List<int[]>();
        for (int i = 0; i < rows; i++)
        {
            int[] temp = new int[cols];
            gameBoard.Add(temp);
        }
        
        Debug.Log(gameBoard.Count);

        columns = cols;
        this.rows = rows;
    }

    public void PrintBoard()
    {
        string demo = String.Empty;
        for (int i = 0; i < rows - 1; i++)
        {
            demo += $"{i} ";
            for (int j = 0; j < columns; j++)
            {
                gameBoard[i][j] = 0;
                demo += " 1";
            }

            //demo += 0;
            demo += "\n";
        }
        Debug.Log(demo);

    }
}
