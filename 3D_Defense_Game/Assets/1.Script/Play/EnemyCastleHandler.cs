using UnityEngine;

public class EnemyCastleHandler : MonoBehaviour
{   
    private GameObject mainCastle;
    [SerializeField] private Transform MainCastlePos;
    // 여기에 적 성 스크립트 갖고 여기서 컨트롤하기
    
    public void Init(EnemyInfoSO enemyInfoSO)
    {
        mainCastle = Instantiate(enemyInfoSO.Enemy_castle , MainCastlePos);

    }
}
