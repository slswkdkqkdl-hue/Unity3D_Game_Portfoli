using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameLog : MonoBehaviour
{
    public static InGameLog I { get; private set; }

    [Header("UI Component")]
    [SerializeField] private TextMeshProUGUI logText; // 로그를 표시할 TextMeshPro

    [Header("Log Settings")]
    [SerializeField] private int maxLogCount = 7; // 최대 표시할 로그 줄 수

    private Queue<string> logQueue = new Queue<string>();

    private void Awake()
    {
        // 싱글톤 초기화
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 외부에서 로그 메시지를 추가할 때 호출하는 함수
    /// 사용예: InGameLogUI.Instance.AddLog("적 영웅 (히포그리프)가 출격 하였습니다.");
    /// </summary>
    public void AddLog(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        // 큐에 새로운 로그 추가
        logQueue.Enqueue(message);

        // 설정한 최대 줄 수를 초과하면 가장 오래된 로그 제거
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        UpdateLogDisplay();
    }

    /// <summary>
    /// 큐에 남아있는 로그들을 줄바꿈(\n)으로 합쳐서 텍스트에 표시
    /// </summary>
    private void UpdateLogDisplay()
    {
        if (logText == null) return;

        // Queue의 모든 메시지를 줄바꿈으로 연결
        logText.text = string.Join("\n", logQueue);
    }

    /// <summary>
    /// 로그 창 완전히 비우기 (씬 재시작/초기화 용도)
    /// </summary>
    public void ClearLog()
    {
        logQueue.Clear();
        if (logText != null)
        {
            logText.text = string.Empty;
        }
    }
}