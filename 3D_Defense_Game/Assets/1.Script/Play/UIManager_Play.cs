using UnityEngine;

public class UIManager_Play : MonoBehaviour
{
    [Header("버튼")]
    public SceneType PlayButton = SceneType.PlayScreen;
    public SceneType StartButton = SceneType.StartScreen;




    // 시작씬 돌아가기 버튼용
    public void backButton()
    {
        LoadingManager.LoadScene(StartButton,false);
    }
}
