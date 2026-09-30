using System.Collections.Generic;
using UnityEngine;

public class UIManager_Select : MonoBehaviour
{
    public static UIManager_Select I;

    [Header("UI 패널 참조")]
    public HeroPenl heroPanel;

    [Header("버튼 관련")]
    public SceneType StartButton = SceneType.LobbyScreen;
    public SceneType backBtn = SceneType.StartScreen;

    [Header("프리팹 및 생성 위치")]
    public GameObject domainSlotPrefab;      // GameObject 대신 컴포넌트 타입으로 받으면 GetComponent 불필요
    public GameObject unitSlotPrefab;
    public Transform domainRoot;
    public Transform unitRoot;

    private readonly List<DomainSlot> domainSlots = new List<DomainSlot>();
    private readonly Dictionary<TerritoryData, List<UnitInfoSlot>> unitSlotCache= new Dictionary<TerritoryData, List<UnitInfoSlot>>(); //  영지에 유닛슬룻 갖고있음

    private TerritoryData currentTerritory;

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

        // 1. 영지 슬롯 전부 생성
        foreach (var territory in territories)
        {
            GameObject dom = Instantiate(domainSlotPrefab, domainRoot);
            var slot = dom.GetComponent<DomainSlot>();
            slot.Init(territory);
            slot.onSlotSelected += OnDomainSlotSelected;
            domainSlots.Add(slot);
        }

        // 2. 생성이 모두 끝난 뒤 첫 번째 영지 자동 선택
        domainSlots[0].OnClickSlot();
    }

    private void OnDomainSlotSelected(TerritoryData territory)
    {
        if (territory == null || territory == currentTerritory) return;
        currentTerritory = territory;

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
    }

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
        // 선택한 영지를 넘겨야 맵 씬에서 domainType을 사용할 수 있음
        DataManager.I.NewGame();
        LoadingManager.LoadScene(StartButton, true);
    }

    public void backButton()
    {
        LoadingManager.LoadScene(backBtn, false);
    }
}