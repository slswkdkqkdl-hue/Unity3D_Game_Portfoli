using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    
    public Material skyboxMaterial;

    [Header("회전 속도 설정")]
    public float bgRotateSpeed = 0.2f;    
    public float midRotateSpeed = 0.5f;  

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
        
            currentBgRotation = skyboxMaterial.GetFloat("_BackgroundRotation");
            currentMidRotation = skyboxMaterial.GetFloat("_MidgroundRotation");
        }
    }

    void Update()
    {
        if (skyboxMaterial == null) return;

     
        currentBgRotation += bgRotateSpeed * Time.deltaTime;
        currentMidRotation += midRotateSpeed * Time.deltaTime;


        currentBgRotation %= 360f;
        currentMidRotation %= 360f;

      
        skyboxMaterial.SetFloat("_BackgroundRotation", currentBgRotation);
        skyboxMaterial.SetFloat("_MidgroundRotation", currentMidRotation);
    }
}