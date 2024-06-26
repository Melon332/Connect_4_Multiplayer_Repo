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

    [Header("Buttons")] 
    [SerializeField] private GameObject restartGameButton;

    private void Awake()
    {
        pauseMenuPanel.SetActive(false);
        hudPanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void ToggleWaitingForPlayersPanel(bool toggle)
    {
        waitingForPlayersPanel.SetActive(toggle);
    }

    public void TogglePauseMenu(bool toggle)
    {
        pauseMenuPanel.SetActive(toggle);
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
        StartCoroutine(ShowTurnTextCoroutine(seconds));
    }

    private IEnumerator ShowTurnTextCoroutine(float seconds)
    {
        turnText.SetActive(true);
        yield return new WaitForSeconds(seconds);
        turnText.SetActive(false);
    }
}
