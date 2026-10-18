using UnityEngine;

public class UIManager_Play : MonoBehaviour
{



    public void BackButton()
    {   
        BattleEntryData.ReleaseCurrentStage();
        LoadingManager.LoadScene(SceneType.LobbyScreen,true);
    }
}
