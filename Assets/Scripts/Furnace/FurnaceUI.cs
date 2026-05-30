using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    [SerializeField] private PizzaFurnace furnace;

    [Header("UI")]
    [SerializeField] private RawImage gradientImage;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private TMP_Text temperatureText;

    private void Awake()
    {
        CreateTemperatureGradient();
    }

    private void Update()
    {
        UpdateTemperature();
    }

    private void UpdateTemperature()
    {
        temperatureSlider.minValue = 0f;
        temperatureSlider.maxValue = furnace.maxTemperature;

        temperatureSlider.value = furnace.currentTemperature;

        temperatureText.text = $"{Mathf.RoundToInt(furnace.currentTemperature)}°C";
    }


    private void CreateTemperatureGradient()
    {
        int width = 512;

        Texture2D texture = new Texture2D(width, 1);

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new(Color.blue, 20f / furnace.maxTemperature),
                new(Color.yellow, 150f / furnace.maxTemperature),
                new(Color.green, furnace.minCookingTemperature / furnace.maxTemperature),
                new(Color.green, furnace.maxCookingTemperature / furnace.maxTemperature),
                new(new Color(1f, 0.5f, 0f), furnace.overheatTemperature / furnace.maxTemperature),
                new(Color.red, 1f)
            },
            new GradientAlphaKey[]
            {
                new(1f,0f),
                new(1f,1f)
            }
        );

        for (int x = 0; x < width; x++)
        {
            float t = (float)x / (width - 1);

            texture.SetPixel(x, 0, gradient.Evaluate(t));
        }

        texture.Apply();

        gradientImage.texture = texture;
    }
}