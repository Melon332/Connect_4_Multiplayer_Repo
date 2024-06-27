using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
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

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }
    
    [Header("Misc")]
    [SerializeField] private Camera uiCamera;

    [Header("Main Panels")] 
    [SerializeField] private GameObject mainMenuPanel;
    //[SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject inLobbyPanel;
    
    [Header("Buttons")] 
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button copyIDButton;
    [SerializeField] private Button startGameButton;
    
    [Header("InputFields and Texts")]
    [SerializeField] private TMP_InputField lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI lobbyIDDisplay;

    [Header("Message System Variables")] 
    [SerializeField] private TMP_InputField messageField;
    [SerializeField] private TextMeshProUGUI messageTemplate;
    [SerializeField] private GameObject messageContainer;
    [SerializeField] private ScrollRect messageScrollView;
    
    

    private MenuStates currentMenuState;
    private MenuStates previousMenuState;
    
    private void Awake()
    {
        Instance = this;
        inLobbyPanel.SetActive(false);

        CheckCurrentLoadedScene();
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

    /*
    public void SetHostLobbyButtonMethod(UnityAction method)
    {
        hostLobbyButton.onClick.AddListener(method);
        
    }

    public void SetJoinLobbyButtonMethod(UnityAction method)
    {
        joinLobbyButton.onClick.AddListener(method);
        Debug.Log("yes2");
    }

    public void SetReturnToMenuButtonMethod(UnityAction method)
    {
        backToMenuButton.onClick.AddListener(method);
    }
    
    public void SetStartGameButtonMethod(UnityAction method)
    {
        startGameButton.onClick.AddListener(method);
    }
    */


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
