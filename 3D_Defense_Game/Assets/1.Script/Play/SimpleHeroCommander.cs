using UnityEngine;

public class SimpleHeroCommander : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {   
    
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