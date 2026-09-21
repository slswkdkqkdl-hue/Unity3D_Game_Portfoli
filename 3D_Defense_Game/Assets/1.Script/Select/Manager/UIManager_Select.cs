using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_Select : MonoBehaviour
{   
    public static UIManager_Select I;

    [Header("각 UI 데이터")]
    public HeroArt heroArt;
    public HeroDescription heroDescription;
    public UnitDescription unitDescription;

    [Header("게임시작버튼")]
    public  SceneType StartButton;

    [Header("영웅 선택버튼모음")]
    public List<HeroSelectButton> heroSelectButtons;

    [Header("유닛 박스 동적 생성 설정")]
    public Transform unitArrayParent; 
    public GameObject unitBoxPrefab;  

    private List<GameObject> activeUnitBoxes = new List<GameObject>();

    void Awake()
    {
        I = this;
    }
    void Start()
    {
        StartInit(); // 맨처음 영웅선택버튼중 0 번째 선택되어있게함
    }

    private void StartInit()
    {
        // 리스트가 Null이 아니고, 최소 1개 이상 들어있으며, 0번 요소가 Null이 아닐 때만 실행
        if (heroSelectButtons != null && heroSelectButtons.Count > 0 && heroSelectButtons[0] != null)
        {
            heroSelectButtons[0].OnClickSelectHero();
        }
        else
        {
            Debug.LogWarning("heroSelectButtons 리스트가 비어있거나 0번 엘리먼트가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// 영웅 선택 시 전체 UI 데이터 세팅
    /// </summary>
    public void SelectData(HeroDescData heroDescData, List<UnitDescData> unitDescDatas)
    {
        // 이전 유닛 박스들은 어떤 상황에서든 항상 먼저 제거
        ClearUnitBoxes();

        // 1. 영웅 정보가 유효한 경우
        if (heroDescData != null)
        {
            if (heroDescription != null)
            {
                heroDescription.SetHeroInfo(heroDescData);
            }
            if (heroArt != null)
            {
                // 추후 추가예정
            }

            // 유닛 박스 생성 및 갱신
            if (unitDescDatas != null && unitDescDatas.Count > 0)
            {
                for (int i = 0; i < unitDescDatas.Count; i++)
                {
                    GameObject newBox = Instantiate(unitBoxPrefab, unitArrayParent);
                    activeUnitBoxes.Add(newBox);

                    UnitBox boxScript = newBox.GetComponent<UnitBox>();
                    if (boxScript != null)
                    {
                        boxScript.SetData(unitDescDatas[i]);
                    }
                }

                // 첫 번째 유닛 자동 선택
                SelectUnit(unitDescDatas[0]);
            }
            else
            {
                // 유닛이 없는 영웅일 경우 유닛 설명창 초기화
                if (unitDescription != null) unitDescription.ClearUI();
            }
        }
        // 2. 영웅 정보가 없는 경우 (전체 UI 초기화)
        else
        {
            if (heroDescription != null) heroDescription.ClearUI();
            if (unitDescription != null) unitDescription.ClearUI();
        }
    }
    // 모든 유닛박스 삭제
    private void ClearUnitBoxes()
    {
        foreach (GameObject box in activeUnitBoxes)
        {
            if (box != null) Destroy(box);
        }
        activeUnitBoxes.Clear();
    }
    // 선택한 유닛 정보 갱신
    public void SelectUnit(UnitDescData unitDescData)
    {
        if (unitDescription != null && unitDescData != null)
        {
            unitDescription.SetUnitInfo(unitDescData);
        }
    }

    // 게임시작 버튼 
    public void NewGameStart()
    {
       var Heor = SelectManager.I.GetSelectedHero();
       var units = SelectManager.I.GetSelectedUnits();

        if (Heor == null || units == null) return;

        DataManager.I.NewGame();
        LoadingManager.LoadScene(StartButton,true);
    }
}