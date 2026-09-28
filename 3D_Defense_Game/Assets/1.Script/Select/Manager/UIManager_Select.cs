using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_Select : MonoBehaviour
{   
    public static UIManager_Select I;



    [Header("버튼")]
    public  SceneType StartButton = SceneType.LobbyScreen;
    public  SceneType backBtn = SceneType.StartScreen;


    void Awake()
    {
        I = this;
    }




    // 게임시작 버튼 
    public void NewGameStart()
    {    ;
        DataManager.I.NewGame();
        LoadingManager.LoadScene(StartButton,true);
    }
    public void backButton()
    {   
       LoadingManager.LoadScene(backBtn,false);
    }
}