using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // 씬에 있는 메인 카메라를 자동으로 찾아옵니다.
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            transform.rotation = mainCameraTransform.rotation;
        }
    }
}