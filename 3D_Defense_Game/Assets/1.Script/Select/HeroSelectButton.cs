using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectButton : MonoBehaviour
{   
    [Header("영웅 정보 설정")]
    public string heroClass;
    public int heroId;
    public List<int> units = new List<int>();

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private Button button;

    private void Awake()
    {
        // Button 컴포넌트 자동 캐싱
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// 버튼 UI 및 클릭 가능 여부 초기화
    /// </summary>
    private void Init()
    {
        // 0 이하의 ID는 미구현/준비 중 상태로 처리
        bool isReady = heroId > 0;

        if (classNameText != null)
        {
            classNameText.text = isReady ? heroClass : "준비 중";
        }

        // 버튼 컴포넌트가 있다면 '준비 중'일 때 비활성화 처리
        if (button != null)
        {
            button.interactable = isReady;
        }
    }

    /// <summary>
    /// 영웅 버튼 클릭 시 영웅 ID와 보유 유닛 ID 목록을 SelectManager로 전달
    /// </summary>
    public void OnClickSelectHero()
    {
        // ID가 유효하지 않으면 클릭 동작 안 함
        if (heroId <= 0) return;

        if (SelectManager.I != null)
        {
            SelectManager.I.SelectHero(heroId, units);
        }
    }
}