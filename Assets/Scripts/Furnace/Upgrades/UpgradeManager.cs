using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Furnace")]
    [SerializeField] private PizzaFurnace pizzaFurnace;

    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Slots UI")]
    [SerializeField] private UpgradeSlotUI[] slots;

    [Header("Database")]
    [SerializeField] private UpgradeData[] allUpgrades;

    private List<UpgradeData> currentOptions = new List<UpgradeData>();

    private void Awake()
    {
        Instance = this;
    }

    public void OpenUpgradeSelection()
    {
        upgradePanel.SetActive(true);

        playerMovement.SetMovementEnabled(false);

        CursorManager.SetMenuMode();

        GenerateCards();

        Debug.Log("Upgrade Menu Aberto");
    }

    public void CloseUpgradeSelection()
    {
        upgradePanel.SetActive(false);

        playerMovement.SetMovementEnabled(true);

        CursorManager.SetGameplayMode();

        Debug.Log("Upgrade Menu Fechado");
    }

    private void GenerateCards()
    {
        if (allUpgrades == null || allUpgrades.Length == 0)
        {
            Debug.LogError("allUpgrades está vazio!");
            return;
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("slots não estão atribuídos!");
            return;
        }

        currentOptions.Clear();

        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < slots.Length; i++)
        {
            if (pool.Count == 0)
            {
                Debug.LogWarning("Sem upgrades suficientes para preencher todos os slots!");
                break;
            }

            int index = Random.Range(0, pool.Count);
            UpgradeData selected = pool[index];

            pool.RemoveAt(index);

            slots[i].assignedUpgrade = selected;

            slots[i].upgradeText.text = selected.upgradeName;
        }
    }

    public void Reroll(int index)
    {
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        // remove upgrades já visíveis (opcional mas recomendado)
        foreach (var slot in slots)
        {
            pool.Remove(slot.assignedUpgrade);
        }

        int randomIndex = Random.Range(0, pool.Count);
        UpgradeData newUpgrade = pool[randomIndex];

        slots[index].assignedUpgrade = newUpgrade;
        slots[index].upgradeText.text = newUpgrade.upgradeName;

        slots[index].rerollCanvasGroup.alpha = 0.4f;
        slots[index].rerollCanvasGroup.interactable = false;
        slots[index].rerollCanvasGroup.blocksRaycasts = false;
    }

    public void SelectUpgrade(int index)
    {
        UpgradeData upgrade = slots[index].assignedUpgrade;

        Debug.Log("Escolhido: " + upgrade.upgradeName);

        pizzaFurnace.ApplyUpgrade(upgrade);

        CloseUpgradeSelection();
    }
}