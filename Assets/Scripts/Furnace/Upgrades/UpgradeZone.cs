using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private int upgradeCost = 100;

    [Header("UI")]
    [SerializeField] private GameObject ui;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text costText;

    private float timer;
    private bool playerInside;

    private void Start()
    {
        costText.text = $"{upgradeCost} $";
        progressSlider.value = 0f;
    }

    private void Update()
    {
        if (!playerInside)
            return;

        timer += Time.deltaTime;

        float progress = timer / holdTime;

        progressSlider.value = progress;

        if (progress >= 1f)
        {
            TryBuyUpgrade();
        }
    }

    private void TryBuyUpgrade()
    {
        if (!MoneyManager.Instance.SpendMoney(upgradeCost))
        {
            Debug.Log("Dinheiro insuficiente!");
            ResetZone();
            return;
        }

        UpgradeManager.Instance.OpenUpgradeSelection();
        MoneyManager.Instance.SpendMoney(100);

        ResetZone();
    }

    private void ResetZone()
    {
        timer = 0f;
        playerInside = false;

        progressSlider.value = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        ResetZone();
    }
}