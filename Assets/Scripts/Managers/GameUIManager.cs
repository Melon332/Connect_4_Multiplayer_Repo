using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject waitingForPlayersPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject endGamePanel;

    [Header("Text elements")] 
    [SerializeField] private GameObject turnText;
    [SerializeField] private List<TextMeshProUGUI> playerScoreText = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI winnerText;

    [Header("Buttons")] 
    [SerializeField] private GameObject restartGameButton;
    
    [Header("Text to display before data")] 
    [SerializeField] private string winsTextString;
    [SerializeField] private string winnerTextString;

    private Coroutine showTextCoroutine;

    private void Awake()
    {
        pauseMenuPanel.SetActive(false);
        hudPanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void InitalizePlayerScoreText(List<PlayerManager> players)
    {
        for (int i = 0; i < playerScoreText.Count; i++)
        {
            if (i >= players.Count)
            {
                Debug.LogError("Not enough players!");
                playerScoreText[^1].text = $"Unknown Player Wins: 0";
                return;
            }
            playerScoreText[i].text = $"{players[i].PlayerName} {winsTextString} \n0";
        }
    }

    public void UpdatePlayerScoreText(List<PlayerManager> players)
    {
        for (int i = 0; i < playerScoreText.Count; i++)
        {
            if (i >= players.Count)
            {
                Debug.LogError("Not enough players!");
                playerScoreText[^1].text = $"Unknown Player Wins: 0";
                return;
            }
            playerScoreText[i].text = $"{players[i].PlayerName} {winsTextString} \n{players[i].Wins}";
        }
    }

    public void SetWinnerText(PlayerManager player)
    {
        winnerText.text = $"{winnerTextString}\n{player.PlayerName}!";
    }

    public void ToggleWaitingForPlayersPanel(bool toggle)
    {
        waitingForPlayersPanel.SetActive(toggle);
    }

    public void TogglePauseMenu(bool toggle)
    {
        pauseMenuPanel.SetActive(toggle);
        hudPanel.SetActive(!toggle);
    }

    public void ToggleHUDPanel(bool toggle)
    {
        hudPanel.SetActive(toggle);
    }

    public void ToggleEndGamePanel(bool toggle)
    {
        endGamePanel.SetActive(toggle);
        restartGameButton.SetActive(NetworkManager.Singleton.IsHost);
    }

    public void ShowTurnText(float seconds)
    {
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
        }
        showTextCoroutine = StartCoroutine(ShowTurnTextCoroutine(seconds));
    }

    private IEnumerator ShowTurnTextCoroutine(float seconds)
    {
        turnText.SetActive(true);
        yield return new WaitForSeconds(seconds);
        turnText.SetActive(false);
    }
}
