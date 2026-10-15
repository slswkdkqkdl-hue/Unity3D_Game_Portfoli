using UnityEngine;

public class UIManager_Play : MonoBehaviour
{



    public void BackButton()
    {
        LoadingManager.LoadScene(SceneType.LobbyScreen,true);
    }
}
