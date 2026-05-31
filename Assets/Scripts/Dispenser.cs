using UnityEngine;
using UnityEngine.UI;

public class Dispenser : MonoBehaviour
{
    [Header("Dispenser Settings")]
    public GameObject prefab;
    public Transform dropPoint;
    public float timeBetweenDrops = 0.5f;
    public float heightStepPerBox = 0.2f;

    [Header("Cargo Rules")]
    public string dispenserItemType = "Pizza";

    [Header("Shader Hook")]
    [Tooltip("Reads from 0 to 1. Feed this into your shader!")]
    public float dropProgress = 0f;
    public GameObject canvas;
    public Image progressImage;

    private float timer = 0f;
    private bool isPlayerWaiting = false;
    private Vector3 originalDropLocalPos;

    private void Start()
    {
        canvas.SetActive(false);

        if (dropPoint != null)
        {
            originalDropLocalPos = dropPoint.localPosition;
        }
    }

    private void Update()
    {
        if (!isPlayerWaiting) return;

        timer += Time.deltaTime;
        dropProgress = timer / timeBetweenDrops;

        if (progressImage != null)
        {
            progressImage.fillAmount = dropProgress;
        }

        if (timer >= timeBetweenDrops)
        {
            DispensePizza();
            timer = 0f;
            dropProgress = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CargoManager cargo = other.GetComponent<CargoManager>();

            if (cargo != null && cargo.currentItemType != "" && cargo.currentItemType != dispenserItemType)
            {
                return; // Do absolutely nothing. Ignore the player.
            }

            canvas.SetActive(true);
            isPlayerWaiting = true;
            timer = 0f;
            dropProgress = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvas.SetActive(false);
            isPlayerWaiting = false;
            timer = 0f;
            dropProgress = 0f;

            if (dropPoint != null)
            {
                dropPoint.localPosition = originalDropLocalPos;
            }

            // TODO: Maybe create a system that the Player needs to wait some 
            // time for the pizza to be ready, if they leave the place the pizza
            // progress goes back

            // Cool shader to indicate the pizza making - Green bar going up
        }
    }

    private void DispensePizza()
    {
        Instantiate(prefab, dropPoint.position, prefab.transform.rotation);
        dropPoint.position += new Vector3(0, heightStepPerBox, 0);
    }
}
