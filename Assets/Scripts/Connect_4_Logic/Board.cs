using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private List<int[]> gameBoard;

    private int rows, columns;

    private int rowsAndColumnsCombined;

    public Board(int cols, int rows)
    {
        gameBoard = new List<int[]>();
        for (int i = 0; i < rows; i++)
        {
            int[] temp = new int[cols];
            gameBoard.Add(temp);
        }
        
        columns = cols;
        this.rows = rows;
        rowsAndColumnsCombined = this.rows * columns;
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
        Debug.LogError(demo);
    }

    public void SetTile(int row, int column, int playerNumber)
    {
        gameBoard[row][column] = playerNumber;
    }

    public bool CheckIfBoardIsFull()
    {
        int filledGrid = 0;
        for (int i = 0; i < rows; i++)
        {
            //Debug.Log(i);
            for (int j = 0; j < columns; j++)
            {
                //Check if the grid position is taken
                if (gameBoard[i][j] != 0)
                {
                    filledGrid += 1;
                }
            }
        }

        return filledGrid >= rowsAndColumnsCombined;
    }

    public bool CheckForWin(int playerIndex)
    {
        // horizontalCheck 
        for (int j = 0; j<rows-3 ; j++ )
        {
            for (int i = 0; i<columns; i++)
            {
                if (gameBoard[i][j] == playerIndex && gameBoard[i][j+1] == playerIndex && gameBoard[i][j+2] == playerIndex && gameBoard[i][j+3] == playerIndex)
                {
                    return true;
                }           
            }
        }
        // verticalCheck
        for (int i = 0; i<columns-3 ; i++ )
        {
            for (int j = 0; j<rows; j++)
            {
                if (gameBoard[i][j] == playerIndex && gameBoard[i+1][j] == playerIndex && gameBoard[i+2][j] == playerIndex && gameBoard[i+3][j] == playerIndex){
                    return true;
                }           
            }
        }
        // ascendingDiagonalCheck 
        for (int i=3; i<columns; i++)
        {
            for (int j=0; j<rows-3; j++)
            {
                if (gameBoard[i][j] == playerIndex && gameBoard[i-1][j+1] == playerIndex && gameBoard[i-2][j+2] == playerIndex && gameBoard[i-3][j+3] == playerIndex)
                    return true;
            }
        }
        // descendingDiagonalCheck
        for (int i=3; i<columns; i++)
        {
            for (int j=3; j<rows; j++)
            {
                if (gameBoard[i][j] == playerIndex && gameBoard[i-1][j-1] == playerIndex && gameBoard[i-2][j-2] == playerIndex && gameBoard[i-3][j-3] == playerIndex)
                    return true;
            }
        }
        return false;
    }

    public void ResetBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gameBoard[i][j] = 0;
            }
        }
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
                return i;
            }
        }
        return -1;
    }
}
