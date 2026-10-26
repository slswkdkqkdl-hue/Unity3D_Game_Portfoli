using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager I;

    [Header("플레이어 자원")]
    [SerializeField] private int money = 0;       
    [SerializeField] private int population = 0;    
    [SerializeField] private int maxPopulation = 0; 

    // 외부에서 필요할때 사용할 프로퍼티
    public int Money => money;
    public int Population => population;
    public int MaxPopulation => maxPopulation;

    // 리소스 UI 가받을 이벤트 
    public event Action<int> OnMoneyChanged;
    public event Action<int, int> OnPopulationChanged; 


    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    public int moneyChak()
    {
        return money;
    }

    // 현재 돈 증가/감소 함수
    public void ModifyMoney(int amount)
    {
        money += amount; // 양수면 증가, 음수면 감소
        
        // 최소 0원, 최대 999,999원으로 제한하고 싶을 때:
        money = Mathf.Clamp(money, 0, 999999); 

        OnMoneyChanged?.Invoke(money); 
    }


    // 최대 인구수 변경 
    public void ChangeMaxPopulation(int amount)
    {
        maxPopulation += amount;

        OnPopulationChanged?.Invoke(population, maxPopulation);
    }
    // 현재 인구수 증가 
    public bool TryAddPopulation(int amount)
    {
        // 현재 인구 + 추가될 인구 <= 최대 인구 인지 검사
        if (population + amount <= maxPopulation)
        {
            population += amount;
            OnPopulationChanged?.Invoke(population, maxPopulation);
            Debug.Log("현재 인구수 증가");
            return true; 
        }
        return false; 
    }
    // 현재 인구수 감소
    public void DecreasePopulation(int amount)
    {
        population = Mathf.Clamp(population - amount, 0, maxPopulation);
        OnPopulationChanged?.Invoke(population, maxPopulation);
    }
}