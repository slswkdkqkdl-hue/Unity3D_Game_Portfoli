using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Header("스카이박스 머터리얼")]
    public Material skyboxMaterial;

    [Header("회전 속도 설정")]
    public float bgRotateSpeed = 0.2f;    // 배경 (더 천천히)
    public float midRotateSpeed = 0.5f;   // 중간경 (더 빠르게)

    private float currentBgRotation;
    private float currentMidRotation;

    void Start()
    {
        if (skyboxMaterial == null)
        {
            skyboxMaterial = RenderSettings.skybox;
        }

        if (skyboxMaterial != null)
        {
            // 현재 머터리얼에 설정된 초기 회전값 가져오기
            currentBgRotation = skyboxMaterial.GetFloat("_BackgroundRotation");
            currentMidRotation = skyboxMaterial.GetFloat("_MidgroundRotation");
        }
    }

    void Update()
    {
        if (skyboxMaterial == null) return;

        // 각자 설정된 속도로 매 프레임 동시에 회전값 누적
        currentBgRotation += bgRotateSpeed * Time.deltaTime;
        currentMidRotation += midRotateSpeed * Time.deltaTime;

        // 0~360도 범위 유지
        currentBgRotation %= 360f;
        currentMidRotation %= 360f;

        // 셰이더 프로퍼티에 적용
        skyboxMaterial.SetFloat("_BackgroundRotation", currentBgRotation);
        skyboxMaterial.SetFloat("_MidgroundRotation", currentMidRotation);
    }
}