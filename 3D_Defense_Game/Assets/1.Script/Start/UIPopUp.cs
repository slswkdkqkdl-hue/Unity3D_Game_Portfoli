using UnityEngine;
using UnityEngine.UI;

public class UIPopUp : MonoBehaviour
{
    [Header("팝업 공통 UI")]
    [SerializeField] private Button closeButton; // 팝업 내부의 닫기/나가기 버튼

    protected virtual void Awake()
    {
        // 자신의 닫기 버튼에 Close() 메서드를 스스로 연결
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    // 팝업 열기
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    // 팝업 닫기
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    // 현재 팝업이 열려있는지 여부
    public bool IsOpen => gameObject.activeSelf;
}