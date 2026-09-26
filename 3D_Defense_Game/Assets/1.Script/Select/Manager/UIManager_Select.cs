using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_Select : MonoBehaviour
{   
    public static UIManager_Select I;



    [Header("게임시작버튼")]
    public  SceneType StartButton = SceneType.LobbyScreen;


    void Awake()
    {
        I = this;
    }
    void Start()
    {
   
    }



    // 게임시작 버튼 
    public void NewGameStart()
    {
        DataManager.I.NewGame();
        LoadingManager.LoadScene(StartButton,true);
    }
}