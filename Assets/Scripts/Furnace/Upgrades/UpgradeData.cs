using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades/New Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea] public string description;

    [Header("Furnace Buffs")]
    public float temperatureIncreaseMultiplier = 1f;
    public float cooldownSpeedMultiplier = 1f;
    public float fuelEfficiencyMultiplier = 1f;

    [Header("Extinguisher Buffs")]
    public float gasEfficiencyMultiplier = 1f;
    public float coolingPowerMultiplier = 1f;

    [Header("Debuffs (opcional)")]
    public float overheatRiskMultiplier = 1f;
}