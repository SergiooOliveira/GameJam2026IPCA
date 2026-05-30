using UnityEngine;
using UnityEngine.InputSystem;

public class PizzaFurnace : MonoBehaviour
{
    [Header("Temperature")]
    public float currentTemperature = 20f;
    public float maxTemperature = 500f;
    public float overheatTemperature = 400f;
    public float stableTemperature = 370f;

    [Header("Settings")]
    public float fuelIncrease = 50f;
    public float cooldownSpeed = 25f;

    [Header("Extinguisher Efficiency")]
    public float lowTempEfficiency = 0.2f;
    public float mediumTempEfficiency = 0.4f;
    public float highTempEfficiency = 0.7f;
    public float overheatEfficiency = 1f;
    public float criticalEfficiency = 1.2f;

    [Header("Fuel")]
    public float currentFuel;
    public float maxFuel = 100f;
    public float fuelBurnRate = 2f;
    public float heatPerFuel = 8f;
    public float fuelPerLog = 10f;

    [Header("Cooking Range")]
    public float minCookingTemperature = 250f;
    public float maxCookingTemperature = 350f;

    [Header("Overheat")]
    public float overheatHeatingRate = 30f;

    [Header("States")]
    public bool isOverheated = false;
    private bool previousCookingState;

    [SerializeField] private FireExtinguisher testExtinguisher;
    private bool usingExtinguisher;

    private void Update()
    {
        HandleTemperature();
        HandleFuel();

        bool currentCookingState = IsInCookingRange;

        if (currentCookingState != previousCookingState)
        {
            previousCookingState = currentCookingState;

            Debug.Log(
                currentCookingState
                    ? "Temperatura ideal para pizzas."
                    : "Temperatura n�o ideal para pizzas."
            );
        }

        if (usingExtinguisher)
        {
            TestUseExtinguisher();
        }
    }

    public void TestUseExtinguisher()
    {
        if (testExtinguisher == null)
        {
            Debug.LogWarning("Nenhum extintor atribu�do.");
            return;
        }

        float gasUsed = testExtinguisher.Spray();

        if (gasUsed > 0)
        {
            CoolDown(
                gasUsed,
                testExtinguisher.temperatureReductionPerGas
            );

            Debug.Log(
                $"Gas: {testExtinguisher.currentGas:F1} | Temp: {currentTemperature:F1}"
            );
        }
    }

    public void TestExtinguisher(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            usingExtinguisher = true;

        if (ctx.canceled)
            usingExtinguisher = false;
    }

    private void HandleTemperature()
    {
        if (isOverheated)
        {
            currentTemperature += overheatHeatingRate * Time.deltaTime;

            currentTemperature = Mathf.Clamp(currentTemperature, 0f, maxTemperature);

            return;
        }

        if (currentTemperature > 20f)
        {
            float ambientTemperature = 20f;

            currentTemperature = Mathf.MoveTowards(currentTemperature, ambientTemperature, cooldownSpeed * Time.deltaTime);
        }

        currentTemperature = Mathf.Clamp(currentTemperature, 0f, maxTemperature);

        if (currentTemperature >= overheatTemperature && !isOverheated)
        {
            StartOverheat();
        }
    }

    private void HandleFuel()
    {
        if (currentFuel <= 0)
            return;

        float burnedFuel = fuelBurnRate * Time.deltaTime;

        currentFuel -= burnedFuel;

        currentTemperature += burnedFuel * heatPerFuel;
    }

    public void AddFuel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (isOverheated)
        {
            Debug.Log("FORNALHA EM OVERHEAT!");
            return;
        }

        currentFuel += fuelPerLog;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        Debug.Log($"Lenha adicionada. Fuel: {currentFuel}");
    }

    //� a mesma fun�ao de cima, mas esta � para funcionar com unity event
    // assim podes utilizar o teclado para debbug ou esta fun�ao :)
    public void AddFuelDirect()
    {
        

        if (isOverheated)
        {
            Debug.Log("FORNALHA EM OVERHEAT!");
            return;
        }

        currentFuel += fuelPerLog;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        Debug.Log($"Lenha adicionada. Fuel: {currentFuel}");
    }

    public float GetExtinguisherEfficiency()
    {
        if (currentTemperature >= 500f)
            return criticalEfficiency;

        if (currentTemperature >= 400f)
            return overheatEfficiency;

        if (currentTemperature >= 250f)
            return highTempEfficiency;

        if (currentTemperature >= 150f)
            return mediumTempEfficiency;

        return lowTempEfficiency;
    }

    public void CoolDown(float gasUsed, float temperatureReductionPerGas)
    {
        float efficiency = GetExtinguisherEfficiency();

        float coolingAmount = gasUsed * temperatureReductionPerGas * efficiency;

        currentTemperature -= coolingAmount;

        currentTemperature = Mathf.Max(currentTemperature, 20f);

        if (currentTemperature < overheatTemperature)
        {
            isOverheated = false;
        }
    }

    public void DebugTemperature(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Debug.Log($"Temperatura Atual: {currentTemperature}");
    }

    private void StartOverheat()
    {
        isOverheated = true;

        Debug.Log("!!! OVERHEAT !!!");
        Debug.Log("A temperatura est� fora de controlo!");
    }

    public bool IsInCookingRange => currentTemperature >= minCookingTemperature && currentTemperature <= maxCookingTemperature;
}