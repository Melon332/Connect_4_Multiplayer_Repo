using System;
using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public enum MenuStates {
    Setup,
    MainMenu,
    Lobby,
    Game
}

public enum MessageType
{
    Player,
    System
}

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    [Header("Misc")]
    [SerializeField] private Camera uiCamera;

    [Header("Main Panels")] 
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject inLobbyPanel;
    [SerializeField] private GameObject characterCustomizationPanel;
    
    [Header("Buttons")] 
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button copyIDButton;
    [SerializeField] private Button startGameButton;
    
    [Header("InputFields and Texts")]
    [SerializeField] private TMP_InputField lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI lobbyIDDisplay;
    [SerializeField] private TextMeshProUGUI waitingForPlayersText;

    [Header("Message System Variables")] 
    [SerializeField] private TMP_InputField messageField;
    [SerializeField] private TextMeshProUGUI messageTemplate;
    [SerializeField] private GameObject messageContainer;
    [SerializeField] private ScrollRect messageScrollView;

    [Space] [Header("Customization Variables")] 
    [SerializeField] private Button increaseTorsoButton;
    [SerializeField] private Button decreaseTorsoButton;
    [SerializeField] private TextMeshProUGUI torsoCounter;
    [Space]
    [SerializeField] private Button increaseHeadButton;
    [SerializeField] private Button decreaseHeadButton;
    [SerializeField] private TextMeshProUGUI headCounter;
    [Space]
    [SerializeField] private Button increaseEyesButton;
    [SerializeField] private Button decreaseEyesButton;
    [SerializeField] private TextMeshProUGUI eyesCounter;
    [Space]
    [SerializeField] private Button increaseEarsButton;
    [SerializeField] private Button decreaseEarsButton;
    [SerializeField] private TextMeshProUGUI earsCounter;
    [Space] 
    [SerializeField] private Button backCustomizationButton;

    private MenuStates currentMenuState;
    private MenuStates previousMenuState;
    
    private void Awake()
    {
        inLobbyPanel.SetActive(false);
        characterCustomizationPanel.SetActive(false);

        CheckCurrentLoadedScene();
        
        InitalizeTorsoButtons(CharacterCustomizationManager.Instance.ModifyTorso);
        InitalizeHeadButtons(CharacterCustomizationManager.Instance.ModifyHead);
        InitalizeEyesButtons(CharacterCustomizationManager.Instance.ModifyEyes);
        InitalizeEarsButtons(CharacterCustomizationManager.Instance.ModifyEars);
        InitalizeBackButton(CharacterCustomizationManager.Instance.SaveBody);
    }

    private void InitalizeTorsoButtons(Action<bool> method)
    {
        increaseTorsoButton.onClick.AddListener(() => method(true));
        decreaseTorsoButton.onClick.AddListener(() => method(false));
    }
    private void InitalizeHeadButtons(Action<bool> method)
    {
        increaseHeadButton.onClick.AddListener(() => method(true));
        decreaseHeadButton.onClick.AddListener(() => method(false));
    }
    private void InitalizeEyesButtons(Action<bool> method)
    {
        increaseEyesButton.onClick.AddListener(() => method(true));
        decreaseEyesButton.onClick.AddListener(() => method(false));
    }
    private void InitalizeEarsButtons(Action<bool> method)
    {
        increaseEarsButton.onClick.AddListener(() => method(true));
        decreaseEarsButton.onClick.AddListener(() => method(false));
    }

    private void InitalizeBackButton(UnityAction method)
    {
        backCustomizationButton.onClick.AddListener(method);
        
    }

    private void CheckCurrentLoadedScene()
    {
        string loadedScene = SceneManager.GetActiveScene().name;
        switch (loadedScene)
        {
            case "MainMenu":
                SetMenuState(MenuStates.MainMenu);
                break;
            case "Gameplay":
                SetMenuState(MenuStates.Game);
                break;
        }
    }

    public void SetBodyPartIndexText(EBodyType bodyType, int index)
    {
        switch (bodyType)
        {
            case EBodyType.Torso:
                torsoCounter.text = index.ToString();
                break;
            case EBodyType.Head:
                headCounter.text = index.ToString();
                break;
            case EBodyType.Eyes:
                eyesCounter.text = index.ToString();
                break;
            case EBodyType.Ears:
                earsCounter.text = index.ToString();
                break;
        }
    }

    public void JoinedLobby(string ID)
    {
        SetMenuState(MenuStates.Lobby);
        SetLobbyID(ID);
    }
    
    public void LeftLobby(string ID)
    {
        //Reset Menu state back to main menu
        SetMenuState(MenuStates.MainMenu);
        SetLobbyID(ID);
        NetworkManager.Singleton.Shutdown();
    }

    public void CopyID()
    {
        TextEditor textEditor = new TextEditor
        {
            text = lobbyIDDisplay.text
        };
        textEditor.SelectAll();
        textEditor.Copy();
    }
    
    public void ToggleStartGameButton()
    {
        ToggleStartGameButton(true);
        ToggleWaitingForPlayerText(false);
        Debug.Log("am I being called?");
    }

    public void ToggleCharacterCustomizationPanel(bool toggle)
    {
        characterCustomizationPanel.SetActive(toggle);
    }

    public void ToggleMainMenuPanel(bool toggle)
    {
        mainMenuPanel.SetActive(toggle);
    }

    public void ToggleWaitingForPlayerText(bool toggle)
    {
        waitingForPlayersText.gameObject.SetActive(toggle);
    }
    
    public string GetLobbyIDText()
    {
        return lobbyIDInputField.text;
    }

    public void SetLobbyID(string ID)
    {
        lobbyIDDisplay.text = ID;
    }

    public void ToggleChatBox()
    {
        //Make sure we cant open the input field while we are not in the lobby scene
        if (currentMenuState != MenuStates.Lobby) return;
        if (messageField.IsActive())
        {
            if (!string.IsNullOrEmpty(messageField.text))
            {
                if (messageField.text.Contains("START"))
                {
                    SceneLoaderManager.Instance.LoadSceneNet("Gameplay");
                    return;
                }
                LobbySaver.CurrentLobby.SendChatString(messageField.text);
            }
            messageField.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            messageField.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(messageField.gameObject);
        }

        messageField.text = "";
    }

    public void ReceivedMessage(string message, string sender = null)
    {
        //Instaniate a new lobby message
        TextMeshProUGUI temp = Instantiate(messageTemplate.gameObject, messageContainer.transform).GetComponent<TextMeshProUGUI>();
        temp.text = $"{sender}: {message}";
        ScrollToBottomOfScroll();
    }

    private void ScrollToBottomOfScroll()
    {
        messageScrollView.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate( (RectTransform)messageScrollView.transform );
    }
    
    public void SystemMessage(string message, string sender = null)
    {
        //Instaniate a new lobby message
        TextMeshProUGUI temp = Instantiate(messageTemplate.gameObject, messageContainer.transform).GetComponent<TextMeshProUGUI>();
        temp.text = $"{sender} {message}";
        ScrollToBottomOfScroll();
    }

    public void DelegateMessage(MessageType messageType, string message, string sender)
    {
        switch (messageType)
        {
            case MessageType.Player:
                ReceivedMessage(message, sender);
                break;
            case MessageType.System:
                SystemMessage(message, sender);
                break;
        }
        messageScrollView.verticalNormalizedPosition = 0;
        Canvas.ForceUpdateCanvases();
    }

    public void ToggleStartGameButton(bool isHost)
    {
        startGameButton.gameObject.SetActive(isHost);
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }

    public void InviteFriendToGame()
    {
        SteamFriends.OpenGameInviteOverlay(LobbySaver.CurrentLobby.Id);
    }


    public void SetMenuState(MenuStates state)
    {
        previousMenuState = currentMenuState;
        switch (state)
        {
            case MenuStates.Setup:
                //setupPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                inLobbyPanel.SetActive(false);
                break;
            case MenuStates.MainMenu:
                mainMenuPanel.SetActive(true);
                inLobbyPanel.SetActive(false);
                //setupPanel.SetActive(false);
                ClearMessageContainer();
                break;
            case MenuStates.Lobby:
                inLobbyPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                //setupPanel.SetActive(false);
                break;
            case MenuStates.Game:
                inLobbyPanel.SetActive(false);
                mainMenuPanel.SetActive(false);
                //setupPanel.SetActive(false);
                //ACTIVATE HUD
                break;
        }

        currentMenuState = state;
    }

    private void ClearMessageContainer()
    {
        for (int i = 0; i < messageContainer.transform.childCount; i++)
        {
            Destroy(messageContainer.transform.GetChild(i).gameObject);
        }
    }
}
