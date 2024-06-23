using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject waitingForPlayersPanel;

    public void ToggleWaitingForPlayersPanel(bool toggle)
    {
        waitingForPlayersPanel.SetActive(toggle);
    }
}
