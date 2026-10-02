using System;
using UnityEngine;
using UnityEngine.UI;

public class Exit_PopUp : UIPopUp
{   
    
    [SerializeField]private Button button;

    protected override void Awake()
    {
        base.Awake(); // UIPopUp Awake() omba'apo hagua
        
        if (button != null)
        {
            button.onClick.AddListener(GameClose);
        }
    }
    // 게임종료버튼
    public void GameClose() 
    {   
        Application.Quit();
    }
}
