using UnityEngine;

public class Bowman : UnitBase
{
    [Header("아처 전용 설정")]
    [SerializeField] private GameObject arrowInHand;     // 손에 쥐고 있는 화살 오브젝트

    public override void Init(UnitData data, Team team)
    {
        base.Init(data, team);

        // 게임 시작 시 평상시에는 손에 든 화살을 꺼둡니다.
        if (arrowInHand != null)
        {
            arrowInHand.SetActive(false);
        }
    }



    /// <summary>
    /// 화살통 꺼낼때 애니메이션이벤트함수 
    /// </summary>
    public void ShowArrowInHand()
    {
        if (arrowInHand != null)
        {
            arrowInHand.SetActive(true);
        }
    }

    /// <summary>
    /// 화살 발사할때 애니메이션이벤트함수 
    /// </summary>
    public void ShootArrowObject()
    {
        // 손에 든 화살 숨기기
        if (arrowInHand != null)
        {
            arrowInHand.SetActive(false);
        }

    }
}