using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData", order = 1)]
public class StageData : ScriptableObject 
{
    [Header("스테이지 기본 정보")]
    public int stageNumber;       // 스테이지 번호
    public string stageName;      // 스테이지 이름 
    [TextArea] 
    public string stageDescription; // 스테이지 설명
    [Header("스테이지 오브젝트")]
    public GameObject StagePrefab;
    [Header("리소스 및 연출  // 아직안씀")]
    public Sprite backgroundSprite; // 스테이지 배경 이미지
    public AudioClip stageBGM;      // 스테이지 배경음악

    [Header("게임플레이 설정 // 아직 안씀")]
    public float timeLimit;         // 제한 시간 (0이면 제한 없음)
    public int targetScore;         // 클리어 목표 점수 또는 조건 수치

}