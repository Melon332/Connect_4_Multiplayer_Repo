using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuStates {
    Setup,
    MainMenu,
    Lobby,
    Game
}

public class UIManager : MonoBehaviourSingletonPersistent<UIManager>
{
    [Header("Misc")]
    [SerializeField] private Camera uiCamera;

    [Header("Main Panels")] 
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject inLobbyPanel;
    
    [Header("Buttons")] 
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button copyIDButton;
    
    [Header("InputFields and Texts")]
    [SerializeField] private TMP_InputField lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI lobbyIDDisplay;

    [Header("Message System Variables")] 
    [SerializeField] private TMP_InputField messageField;
    [SerializeField] private TextMeshProUGUI messageTemplate;
    [SerializeField] private GameObject messageContainer;
    
    

    private MenuStates currentMenuState;
    private MenuStates previousMenuState;
    
    public override void Awake()
    {
        base.Awake();
        uiCamera.gameObject.SetActive(false);
        
        mainMenuPanel.SetActive(false);
        inLobbyPanel.SetActive(false);
        setupPanel.SetActive(true);
        SetMenuState(MenuStates.MainMenu);
    }

    public void JoinedLobby(string ID)
    {
        SetMenuState(MenuStates.Lobby);
        SetLobbyID(ID);
    }
    
    public void LeftLobby(string ID)
    {
        SetMenuState(MenuStates.MainMenu);
        SetLobbyID(ID);
    }

    public void CopyID()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = lobbyIDDisplay.text;
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
        if (currentMenuState != MenuStates.Lobby) return;
        if (messageField.IsActive())
        {
            if (!string.IsNullOrEmpty(messageField.text))
            {
                LobbySaver.CurrentLobby?.SendChatString(messageField.text);
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
        TextMeshProUGUI temp = Instantiate(messageTemplate.gameObject, messageContainer.transform).GetComponent<TextMeshProUGUI>();
        temp.text = $"{sender}: {message}";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetHostLobbyButtonMethod(UnityAction method)
    {
        hostLobbyButton.onClick.AddListener(method);
    }

    public void SetJoinLobbyButtonMethod(UnityAction method)
    {
        joinLobbyButton.onClick.AddListener(method);
    }

    public void SetReturnToMenuButtonMethod(UnityAction method)
    {
        backToMenuButton.onClick.AddListener(method);
    }

    public void SetMenuState(MenuStates state)
    {
        previousMenuState = currentMenuState;
        switch (state)
        {
            case MenuStates.Setup:
                setupPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                inLobbyPanel.SetActive(false);
                break;
            case MenuStates.MainMenu:
                mainMenuPanel.SetActive(true);
                inLobbyPanel.SetActive(false);
                setupPanel.SetActive(false);
                ClearMessageContainer();
                break;
            case MenuStates.Lobby:
                inLobbyPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                setupPanel.SetActive(false);
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
