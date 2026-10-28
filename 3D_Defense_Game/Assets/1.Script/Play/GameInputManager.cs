using UnityEngine;

public class GameInputManager : MonoBehaviour
{   
    [Header("건물 전용 레이어")]
    [SerializeField] private LayerMask groundLayer;
    [Header("엔티티 감지 레이어")]
    [SerializeField] private LayerMask interactableLayer; 


    // 현재우선순위는 엔티지 정보 열기인데 추후 개선해야함

    private void Update()
    {
        // 1. 마우스 왼쪽 클릭: 유닛 / 영웅 / 적 / 성 공격 포인트 정보 팝업 열기
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // UI를 클릭했을 씬 관통 방지가 필요하다면 EventSystem 체크를 추가할 수 있습니다.
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                // 맞은 오브젝트에서 ICombatEntity 인터페이스 가져오기
                ICombatEntity combatEntity = hit.collider.GetComponent<ICombatEntity>();

                if (combatEntity != null)
                {
                    // PopupManager를 통해 정보창 열기
                    if (PopupManager.I != null)
                    {
                        PopupManager.I.OpenTargetInfoPopup(combatEntity);
                        Debug.Log($"[Commander] 전투 개체 클릭됨: {combatEntity.GetEntityData().entityName}");
                    }
                    return;
                }
            }
        }

        // 2. 마우스 우클릭: 기존 영웅 이동 명령
        if (Input.GetMouseButtonDown(1) && HeroBase.ActiveHero != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                HeroBase.ActiveHero.MoveTo(hit.point);
            }
        }
    }
}