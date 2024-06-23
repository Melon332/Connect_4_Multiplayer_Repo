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

    public void InitalizeBoard()
    {
        string demo = String.Empty;
        for (int i = 0; i < rows; i++)
        {
            gameBoard[i][0] = rows - i;
            for (int j = 0; j < columns; j++)
            {
                gameBoard[i][j] = 0;
                demo += $"{gameBoard[i][j]} ";
            }

            //demo += 0;
            demo += "\n";
        }
        Debug.Log(demo);
    }

    public void PrintBoard()
    {
        string demo = String.Empty;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                demo += $"{gameBoard[i][j]} ";
            }

            //demo += 0;
            demo += "\n";
        }
        Debug.Log(demo);
    }

    public void SetTile(int row, int column, int playerNumber)
    {
        gameBoard[row][column] = playerNumber;
    }

    public int GetFirstAvaliableRow(int column)
    {
        if (column >= columns)
        {
            Debug.LogError("Column was out of index reach stuffs");
            return -1;
        }
        for (int i = rows - 1; i >= 0; i--)
        {
            if (gameBoard[i][column] == 0)
            {
                Debug.Log("Went in");
                gameBoard[i][column] = 1;
                return i;
            }
        }
        return -1;
    }
}
