using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlotClick : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public UpgradeManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.SelectUpgrade(slotIndex);
    }
}