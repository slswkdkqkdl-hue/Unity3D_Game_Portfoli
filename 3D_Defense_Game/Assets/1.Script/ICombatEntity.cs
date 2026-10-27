using UnityEngine;

public interface ICombatEntity
{
    Transform transform { get; }
    Team team { get; }
    GameObject gameObject { get; }
}