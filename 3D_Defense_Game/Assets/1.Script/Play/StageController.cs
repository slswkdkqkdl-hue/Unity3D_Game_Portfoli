using UnityEngine;

public class StageController : MonoBehaviour
{
    public EnemyCastleHandler enemyCastle;
    public PlayerCastleHandler playerCastle;



    public void Init(int PlayerCastleLevel , EnemyInfoSO enemyInfoSO)
    {
        if (playerCastle == null)
        {
            Debug.LogError("[StageController] PlayerCastleHandler가 연결되지 않았습니다.");
            return;
        }

        if (playerCastle.buildingSlotManager == null)
        {
            Debug.LogError("[StageController] BuildingSlotManager가 연결되지 않았습니다.");
            return;
        }

        // 플레이어 성의 건물 슬롯 초기화
        playerCastle.buildingSlotManager.InitializeSlots(PlayerCastleLevel);

        // 적성 생성 및 초기화
        enemyCastle.Init(enemyInfoSO);
        // 플레이어 성 생성 및 초기화
        playerCastle.Init();
    }
}
