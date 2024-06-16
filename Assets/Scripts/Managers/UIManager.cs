using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
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
        mainMenuPanel.SetActive(false);
        inLobbyPanel.SetActive(true);
        SetLobbyID(ID);
    }
    
    public void LeftLobby(string ID)
    {
        mainMenuPanel.SetActive(true);
        inLobbyPanel.SetActive(false);
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
                break;
            case MenuStates.Lobby:
                inLobbyPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                setupPanel.SetActive(false);
                break;
        }
    }
}
