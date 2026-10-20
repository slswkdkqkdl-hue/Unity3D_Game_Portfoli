using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("플레이어 카메라")]
    [SerializeField] private Transform virtualCameraTransform;

    [Header("이동 속도 + 가장자리 인식범위")]
    [SerializeField] private float moveSpeed = 20f; // 카메라 이동 속도
    [SerializeField] private float edgeSize = 20f;  // 화면 가장자리 인식 범위 (픽셀 단위)

    [Header("카메라 이동 인식 범위 x축")]
    [SerializeField] private float minX = 50f;
    [SerializeField] private float maxX = 140f;

    [Header("카메라 이동 인식 범위 z축")]
    [SerializeField] private float minZ = 23f;
    [SerializeField] private float maxZ = 60f;

    private void Update()
    {
        HandleEdgeMovement();
    }

    private void HandleEdgeMovement()
    {
        if (virtualCameraTransform == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x >= Screen.width - edgeSize)
        {
            moveX = 1f;
        }
        else if (mousePos.x <= edgeSize)
        {
            moveX = -1f;
        }
        if (mousePos.y >= Screen.height - edgeSize)
        {
            moveZ = 1f;
        }
        else if (mousePos.y <= edgeSize)
        {
            moveZ = -1f;
        }


        if (moveX == 0f && moveZ == 0f) return;

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        Vector3 currentPos = virtualCameraTransform.position;

        currentPos += moveDir * moveSpeed * Time.deltaTime;

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);

        virtualCameraTransform.position = currentPos;
    }
}