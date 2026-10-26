using UnityEngine;

public class EnemyCastleHandler : MonoBehaviour
{   
    private GameObject mainCastle;
    [SerializeField] private Transform MainCastlePos;
        
    public void Init(EnemyInfoSO enemyInfoSO)
    {
        mainCastle = Instantiate(enemyInfoSO.Enemy_castle , MainCastlePos);
        if (mainCastle.TryGetComponent<EnemyCastle>(out EnemyCastle data))
        {
            data.Init(enemyInfoSO);
        }
    }
}
