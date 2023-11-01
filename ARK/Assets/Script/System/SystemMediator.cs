using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMediator : MonoBehaviour
{

    public PlayerController playerController
    {
        get;
        private set;
    }
    public MySceneManager mySceneManager
    {
        get;
        private set;
    }

    public TeamState teamState
    {
        get;
        private set;
    }

    public TextSystem textSystem
    {
        get;
        private set;
    }

    public UISystem_OnMap uiSystemOnMap
    {
        get;
        private set;
    }

    public EventSystem_OnMap eventSystemOnMap
    {
        get;
        private set;
    }
    private static SystemMediator instance;
    
    public static SystemMediator Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<SystemMediator>().GetComponent<SystemMediator>();
            }

            return instance;
        }
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //getSystem
        GetSystem();

        //setMediator
        setMediator();
        
        //Init
        Init();

    }

    public void Start()
    {
        eventSystemOnMap.NewGame().Forget();
    }

    private void Update()
    {

        playerController.Tick();
    }

    private void GetSystem()
    {
        playerController = FindObjectOfType<PlayerController>();
        mySceneManager = FindObjectOfType<MySceneManager>().GetComponent<MySceneManager>();
        teamState = FindObjectOfType<TeamState>().GetComponent<TeamState>();
        textSystem=FindObjectOfType<TextSystem>().GetComponent<TextSystem>();
        uiSystemOnMap = FindObjectOfType<UISystem_OnMap>().GetComponent<UISystem_OnMap>();
        eventSystemOnMap = FindObjectOfType<EventSystem_OnMap>().GetComponent<EventSystem_OnMap>();

    }

    private void setMediator()
    {
        playerController.setMediator(this);
        mySceneManager.setMediator(this);
        teamState.setMediator(this);
        textSystem.setMediator(this);
        eventSystemOnMap.setMediator(this);
    }

    private void Init()
    {
        
        textSystem.Init();
        teamState.Init();
        mySceneManager.Init();
        playerController.Init();
        eventSystemOnMap.Init();
        uiSystemOnMap.Init();
       

    }




}

    

