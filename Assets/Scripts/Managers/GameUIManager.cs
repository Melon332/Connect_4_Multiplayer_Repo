using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject waitingForPlayersPanel;

    [SerializeField] private GameObject pauseMenuPanel;

    private void Awake()
    {
        pauseMenuPanel.SetActive(false);
    }

    public void ToggleWaitingForPlayersPanel(bool toggle)
    {
        waitingForPlayersPanel.SetActive(toggle);
    }

    public void TogglePauseMenu(bool toggle)
    {
        pauseMenuPanel.SetActive(toggle);
    }
}
