using UnityEngine;
using UnityEngine.UI;
using System;

public class Infirmary : Building
{   
    [Header("회복 관련")]
    [SerializeField] private float recoveryCoolTime = 50;    // 회복 시간초 
    [SerializeField] private float recoveryValue; // 회복량

    public bool isReady = false; // 회복 사용 가능여부 
    public Action<bool> OnStateChanged;

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;

    public float RecoveryCoolTime =>recoveryCoolTime;
    private float timer;

    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Active;
    }

    private void Update()
    {
        if (buildingData == null) return;
        if(isReady) return;

        timer += Time.deltaTime;

        if (fillImage != null)
        {
            // 프로퍼티 혹은 내부 필드 사용
            fillImage.fillAmount = Mathf.Clamp01(timer / recoveryCoolTime);
        }

        if (timer >= recoveryCoolTime)
        {
            timer = 0f;
            isReady = true;
            OnStateChanged?.Invoke(isReady); 
        }
    }

    // 유저가 치료소 팝업의 버튼을 눌렀을 때 호출될 함수
    public void OnClickHealButton()
    {
        if (!isReady) return; // 준비 안 됐으면 무시

        // TODO: 아군 체력 회복 로직 실행
        Logmsg = $"[{buildingData.buildingName}] 부상병 치료 발동!";
        InGameLog.I.AddLog(Logmsg);

        // 사용 완료 후 초기화
        isReady = false;
        timer = 0f;
    }
}
