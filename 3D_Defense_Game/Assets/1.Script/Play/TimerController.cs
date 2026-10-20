using TMPro;
using UnityEngine;
using System;

public class TimerController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI Timer; // 플레이 시간을 표시할 텍스트 (예: "01:30")

    [Header("Time Settings")]
    private float currentTime = 0f;  // 현재 누적된 게임 플레이 시간
    private bool isRunning = false;  // 타이머 작동 여부

    void Update()
    {
        if (!isRunning) return;
        currentTime += Time.deltaTime;
        UpdateTimerText(currentTime);
    }

    /// <summary>
    /// 외부에서 호출하여 타이머 초기화 및 시작/정지 제어
    /// </summary>
    public void Init(bool isStart)
    {
        currentTime = 0f;
        isRunning = isStart; // 전달받은 값으로 작동 여부 결정
        UpdateTimerText(currentTime);
    }

    /// <summary>
    /// TimeSpan을 사용한 간결한 타이머 텍스트 갱신
    /// </summary>
    private void UpdateTimerText(float timeToDisplay)
    {
        if (Timer == null) return;

        if (timeToDisplay < 0) timeToDisplay = 0;

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        Timer.text = string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
    }
}