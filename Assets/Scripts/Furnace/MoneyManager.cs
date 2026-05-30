using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [SerializeField] private int currentMoney = 0;

    public int CurrentMoney => currentMoney;

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);

        return true;
    }

    public bool HasMoney(int amount)
    {
        return currentMoney >= amount;
    }

    public void SetMoney(int amount)
    {
        currentMoney = Mathf.Max(0, amount);
        OnMoneyChanged?.Invoke(currentMoney);
    }
}