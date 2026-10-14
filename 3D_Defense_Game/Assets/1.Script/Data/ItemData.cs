using UnityEngine;

public enum ItemType
{
    Weapon,     // 무기
    Armor,      // 방어구
    None, // 아직생각중
}

public enum ItemRarity
{
    Common = 0,       // 일반
    Uncommon = 1,     // 고급
    Rare = 2,         // 희귀
    Epic = 3,         // 서사
    Mythic = 4,       // 신화
    Transcendent = 5  // 초월
}

[CreateAssetMenu(fileName = "RunItemData", menuName = "Scriptable Objects/RunItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int item_Id;
    public string itemName;
    [TextArea] public string itemDescription; // 아이템 설명
    public Sprite icon;                 
    public ItemType itemType;
    public ItemRarity itemRank;
    [Header("장비 스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;   
    [Header("드랍/생성용 프리팹")]
    public GameObject itemPrefab; // 3D 필드에 떨어질 때 사용
    [Header("드랍 설정")]
    [Range(0f, 100f)]
    public float dropChance = 50f; // 드랍 확률 (%)
}
