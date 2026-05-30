using UnityEngine;

[System.Serializable]
public class UpgradeSlotUI
{
    public GameObject container;
    public TMPro.TMP_Text upgradeText;
    public CanvasGroup rerollCanvasGroup;

    [HideInInspector] public UpgradeData assignedUpgrade;
}