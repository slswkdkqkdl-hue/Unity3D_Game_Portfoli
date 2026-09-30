using System.Collections.Generic;
using UnityEngine;

public class UIManager_Select : MonoBehaviour
{
    public static UIManager_Select I;

    [Header("UI 패널 참조")]
    public HeroPenl heroPanel;
    public HeroModelViewer hero_viewer;

    [Header("버튼 관련")]
    public SceneType StartButton = SceneType.LobbyScreen;
    public SceneType backBtn = SceneType.StartScreen;

    [Header("프리팹 및 생성 위치")]
    public GameObject domainSlotPrefab;      // GameObject 대신 컴포넌트 타입으로 받으면 GetComponent 불필요
    public GameObject unitSlotPrefab;
    public Transform domainRoot;
    public Transform unitRoot;

    private readonly List<DomainSlot> domainSlots = new List<DomainSlot>(); // 처음 도메인 선택용
    private readonly Dictionary<TerritoryData, List<UnitInfoSlot>> unitSlotCache= new Dictionary<TerritoryData, List<UnitInfoSlot>>(); // 각 영지 유닛슬룻 저장용
    private readonly Dictionary<TerritoryData, GameObject> heroModelCache = new Dictionary<TerritoryData, GameObject>(); // 각 영지 영웅모델 저장용

    void Awake()
    {
        I = this;
    }

    void OnDestroy()
    {
        foreach (var slot in domainSlots)
        {
            if (slot != null) slot.onSlotSelected -= OnDomainSlotSelected;
        }
    }

    /// <summary>
    /// 영지 데이터 리스트를 받아 선택 화면 전체를 초기화
    /// </summary>
    public void Init(List<TerritoryData> territories)
    {
        if (territories == null || territories.Count == 0)
        {
            Debug.LogWarning("[UIManager_Select] 영지 데이터가 없습니다.");
            return;
        }

        // 1. 영지 슬롯 전부 생성 + 각 영지 영웅 모델생성
        foreach (var territory in territories)
        {
            GameObject dom = Instantiate(domainSlotPrefab, domainRoot);
            var slot = dom.GetComponent<DomainSlot>();
            slot.Init(territory);
            slot.onSlotSelected += OnDomainSlotSelected;
            domainSlots.Add(slot);

            // 영웅 모델 미리 생성
            GameObject createdHero = hero_viewer.SpawnHero(territory.heroDescDatas.prefab);
            heroModelCache.Add(territory, createdHero);
        }

        // 2. 생성이 모두 끝난 뒤 첫 번째 영지 자동 선택
        domainSlots[0].OnClickSlot();
    }

    private void OnDomainSlotSelected(TerritoryData territory)
    {
        if (territory == null) return;

        // 1. 영웅 패널 갱신
        if (heroPanel != null)
            heroPanel.Init(territory.heroDescDatas);

        // 2. 모든 유닛 슬롯 숨김
        foreach (var list in unitSlotCache.Values)
            foreach (var unitSlot in list)
                unitSlot.gameObject.SetActive(false);

        // 3. 선택한 영지의 유닛 슬롯 표시 (없으면 처음 한 번만 생성)
        foreach (var unitSlot in GetOrCreateUnitSlots(territory))
            unitSlot.gameObject.SetActive(true);

        // 4.영웅뷰 표시(활성화) 
        foreach (var pair in heroModelCache)
        {
            bool isSelected = (pair.Key == territory);
            if (pair.Value != null)
            {
                pair.Value.SetActive(isSelected);
            }
        }
        // 5. 뷰어에게 이름 변경 요청
        if (hero_viewer != null && territory.heroDescDatas != null)
        {
            hero_viewer.SetHeroName(territory.heroDescDatas.hero_name);
        }
    }
    // 선택한 영지 유닛 슬룻 가져오기
    private List<UnitInfoSlot> GetOrCreateUnitSlots(TerritoryData territory)
    {
        if (unitSlotCache.TryGetValue(territory, out var cached))
            return cached;

        var created = new List<UnitInfoSlot>();
        foreach (var unitData in territory.unitDescDatas)
        {
            GameObject unit = Instantiate(unitSlotPrefab, unitRoot);
            var slot = unit.GetComponent<UnitInfoSlot>();
            slot.Init(unitData);
            created.Add(slot);
        }

        unitSlotCache.Add(territory, created);
        return created;
    }

    public void NewGameStart()
    {
        DataManager.I.NewGame();
        LoadingManager.LoadScene(StartButton, true);
    }

    public void backButton()
    {
        LoadingManager.LoadScene(backBtn, false);
    }
}