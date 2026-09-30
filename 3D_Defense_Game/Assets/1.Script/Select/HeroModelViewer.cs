using TMPro;
using UnityEngine;

public class HeroModelViewer : MonoBehaviour
{
    [Header("3D 영웅들이 생성될 부모 Transform")]
    public Transform heroSpawnParent;
    public TextMeshProUGUI hero_name;

    /// <summary>
    /// 영웅 프리팹을 받아와서 뷰어 위치에 생성하고 반환합니다. (처음 초기화용)
    /// </summary>
    public GameObject SpawnHero(GameObject prefab)
    {
        if (prefab == null) return null;

        GameObject heroObj = Instantiate(prefab, heroSpawnParent);
        heroObj.SetActive(false); // 일단 숨겨두기
        return heroObj;
    }

    /// <summary>
    /// 영웅 이름 텍스트를 바꿔줍니다.
    /// </summary>
    public void SetHeroName(string name)
    {
        if (hero_name != null)
        {
            hero_name.text = name;
        }
    }
}