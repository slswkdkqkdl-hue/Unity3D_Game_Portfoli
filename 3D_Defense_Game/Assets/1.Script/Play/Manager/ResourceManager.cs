using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager I;

    [Header("플레이어 자원")]
    [SerializeField] private int money = 0;       
    [SerializeField] private int population = 0;    
    [SerializeField] private int maxPopulation = 0; 

    // 리소스 UI 가받을 이벤트 
    public event Action<int> OnMoneyChanged;
    public event Action<int, int> OnPopulationChanged; 


    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }


    // 현재 돈 증가 함수
    public void ModifyMoney(int amount)
    {
        money += amount; // 양수면 증가, 음수면 감소
        money = Mathf.Max(money, 999999); 

        OnMoneyChanged?.Invoke(money); 
    }
    // 현재 돈 감소 함수 
    public bool UseMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money); 
            return true;
        }
        return false;
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
        
        Debug.Log("인구수가 부족합니다!");
        return false; 
    }
    // 현재 인구수 감소
    public void DecreasePopulation(int amount)
    {
        population = Mathf.Clamp(population - amount, 0, maxPopulation);
        OnPopulationChanged?.Invoke(population, maxPopulation);
    }
}