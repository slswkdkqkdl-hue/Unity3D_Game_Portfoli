using UnityEngine;


[System.Serializable]
public struct CombatEntityData
{
    public string entityName;
    public int currentHp;
    public int currentMp;
    public int currentAttack;
    public int currentDefense;
    public float currentAttackSpeed;
    public float currentMoveSpeed;
    public float detectRange;
    public float attackRange;
    public Sprite image;
}

public interface ICombatEntity
{
    Transform transform { get; }
    Team team { get; }
    GameObject gameObject { get; }
    CombatEntityData GetEntityData();
}