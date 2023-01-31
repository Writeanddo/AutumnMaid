using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [Header("Components")]
    public GameObject m_PressStartText;
    public GameObject m_NGIO;
    public GameObject m_LogOutButton;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    
    [Header("Main Menu")]
    public GameObject MainMenuWrapper;
    public Button NewGameButton;
    public Button OptionsButton;

    private bool m_Started;
    private bool m_Paused;
    private bool m_LogOutActive;
    
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.UI.Enable();
    }

    void Start()
    {
        NewGameButton.onClick.AddListener(this.StartGame);
        OptionsButton.onClick.AddListener(this.OnOptionsButtonClicked);

        m_PressStartText.SetActive(true);
        HideMenus();
<<<<<<< HEAD
        GameManager.instance.SetTouchControls(false);
=======

        if(GameManager.instance.m_GameStarted)
        {
            if(GameManager.instance.m_HappyLevel >= 10)
            {
                StartCoroutine(NGIO.UnlockMedal(72297));
            }
            else if(GameManager.instance.m_HappyLevel < 1)
            {
                StartCoroutine(NGIO.UnlockMedal(72298));
            }
        }
>>>>>>> parent of c44333c (v2.0.4)
    }

    void Update()
    {
        if(m_Started) return;
        if(playerInputActions.Player.Interact.WasPressedThisFrame() || playerInputActions.UI.Submit.WasPressedThisFrame() || 
        playerInputActions.UI.Start.WasPressedThisFrame() || playerInputActions.UI.Click.WasPressedThisFrame())
        {
            m_NGIO.SetActive(true);
            m_Started = true;
            GameManager.instance.m_GameStarted = true;
        }
    }

    public void ExitOptions()
    {
        ShowMainMenu();
        if(m_LogOutActive)
        {
            m_LogOutButton.SetActive(true);
        }
    }

    // this will be called once the API has finished loading everything
    public void OnNewgroundsIOReady(BaseEventData e)
    {
        StartCoroutine(NGIO.UnlockMedal(72296));
        ShowMainMenu();
    }

    public void SetMenuVisibility(bool mainMenu=true, bool continueMenu=true)
    {
        MainMenuWrapper.SetActive(mainMenu);
    }

    public void HideMenus()
    {
        SetMenuVisibility(false,false);
    }

    public void ShowMainMenu() 
    {
        SetMenuVisibility(true,false);
        m_PressStartText.SetActive(false);
        EventSystem.current.SetSelectedGameObject(NewGameButton.gameObject);
        //EventSystem.current.firstSelectedGameObject = NewGameButton.gameObject;
    }

    public void ShowOptionsMenu()
    {
        FindObjectOfType<PauseManager>().PauseGame(true);
    }

    public void StartGame() 
    {
        GameManager.instance.ResetGameStats();
        GameManager.instance.TransferPlayer(2, Vector2.zero, true);
    }

    public void OnOptionsButtonClicked() 
    {
        m_LogOutActive = m_LogOutButton.activeSelf;
        if(m_LogOutActive)
        {
            m_LogOutButton.SetActive(false);
        }
        ShowOptionsMenu();
        HideMenus();
    }
}
