using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;

    private void Start()
    {
        UpdateMoney(MoneyManager.Instance.CurrentMoney);

        MoneyManager.Instance.OnMoneyChanged += UpdateMoney;
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnMoneyChanged -= UpdateMoney;
    }

    private void UpdateMoney(int money)
    {
        moneyText.text = $"${money}";
    }
}