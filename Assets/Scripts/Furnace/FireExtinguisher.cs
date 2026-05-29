using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Gas")]
    public float currentGas = 100f;
    public float maxGas = 100f;

    [Header("Usage")]
    public float gasConsumptionRate = 10f;

    [Header("Cooling")]
    public float temperatureReductionPerGas = 0.8f;

    public bool IsEmpty => currentGas <= 0f;

    public float Spray()
    {
        if (IsEmpty)
            return 0f;

        float gasUsed = gasConsumptionRate * Time.deltaTime;

        currentGas -= gasUsed;
        currentGas = Mathf.Max(0f, currentGas);

        return gasUsed;
    }

    public float GasPercentage => currentGas / maxGas;

}