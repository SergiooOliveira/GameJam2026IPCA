using System.Collections;
using UnityEngine;
using TMPro;

public class RatCustomer : MonoBehaviour
{
    [Header("Order info")]
    public int pizzasWanted = 0;
    public bool isOccupied = false;

    [Header("Delivery Setup")]
    public Transform mouthTarget; // Drag an empty object here for where the pizza flies to
    public float timeBetweenPizzas = 0.5f;
    public float magnetFlightSpeed = 0.2f;

    [Header("UI Setup")]
    public GameObject balloonContainer;
    public TextMeshProUGUI quantityText;

    private bool playerInZone = false;
    private Coroutine deliveryRoutine;

    private void Awake()
    {
        mouthTarget = transform.Find("Mouth");
    }

    public void ActivateCostumer(int minPizzas, int maxPizzas)
    {
        isOccupied = true;
        pizzasWanted = Random.Range(minPizzas, maxPizzas);
        
        if (balloonContainer != null) balloonContainer.SetActive(true);
        UpdateBalloonUI();

        gameObject.SetActive(true);

        // TODO: UI Balloon
    }

    public void CompleteOrder()
    {
        if (balloonContainer != null) balloonContainer.SetActive(false);
        isOccupied = false;
        pizzasWanted = 0;
        playerInZone = false;
        gameObject.SetActive(false);
    }

    private void UpdateBalloonUI()
    {
        if (quantityText != null)
        {
            quantityText.text = pizzasWanted.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the car parked in the Rat's personal trigger zone
        if (other.CompareTag("Player") && isOccupied && pizzasWanted > 0)
        {
            playerInZone = true;
            deliveryRoutine = StartCoroutine(SuckPizzasFromPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (deliveryRoutine != null) StopCoroutine(deliveryRoutine);
        }
    }

    private IEnumerator SuckPizzasFromPlayer()
    {
        while (playerInZone && isOccupied && pizzasWanted > 0)
        {
            // 1. Find all attached pizzas
            GameObject[] allPizzas = GameObject.FindGameObjectsWithTag("SpineBox");
            if (allPizzas.Length == 0) yield break; // Player is out of pizza!

            // 2. Find the TOP pizza
            SpineLink topPizza = null;
            int highestIndex = -1;

            foreach (GameObject box in allPizzas)
            {
                SpineLink link = box.GetComponent<SpineLink>();
                if (link != null && link.stackIndex > highestIndex)
                {
                    highestIndex = link.stackIndex;
                    topPizza = link;
                }
            }

            if (topPizza != null)
            {
                // 3. Sever the physics connection safely
                Destroy(topPizza.GetComponent<ConfigurableJoint>());
                topPizza.tag = "Untagged";

                Rigidbody rb = topPizza.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.detectCollisions = false;

                // 4. Animate it flying
                yield return StartCoroutine(FlyPizza(topPizza.transform));

                // 5. Update the Rat's stomach
                pizzasWanted--;
                UpdateBalloonUI();

                if (pizzasWanted <= 0)
                {
                    CompleteOrder();
                }
            }

            yield return new WaitForSeconds(timeBetweenPizzas);
        }
    }

    private IEnumerator FlyPizza(Transform pizza)
    {
        Vector3 startPos = pizza.position;
        // If you forget to assign a mouth target, it just flies to the rat's center point
        Vector3 endPos = mouthTarget != null ? mouthTarget.position : transform.position + Vector3.up;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime / magnetFlightSpeed;
            if (pizza != null)
            {
                pizza.position = Vector3.Lerp(startPos, endPos, percent);
                pizza.Rotate(0, 720 * Time.deltaTime, 0); // Cool frisbee spin
            }
            yield return null;
        }

        if (pizza != null) Destroy(pizza.gameObject);
    }
}
