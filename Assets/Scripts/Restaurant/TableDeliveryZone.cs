using UnityEngine;
using System.Collections;

public class TableDeliveryZone : MonoBehaviour
{
    [Header("Delivery Setup")]
    public RatCustomer tableCustomer;
    public Transform mouthTarget; // Place an Empty object near the rat's face

    [Header("Speed Dials")]
    public float timeBetweenPizzas = 0.5f;
    public float magnetFlightSpeed = 0.2f; // How fast it flies through the air

    private bool playerInZone = false;
    private Coroutine deliveryRoutine;

    private void OnTriggerEnter(Collider other)
    {
        // If the player parks here, and the rat actually wants pizza...
        if (other.CompareTag("Player") && tableCustomer.isOccupied && tableCustomer.pizzasWanted > 0)
        {
            playerInZone = true;
            deliveryRoutine = StartCoroutine(SuckPizzasFromCar());
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

    private IEnumerator SuckPizzasFromCar()
    {
        while (playerInZone && tableCustomer.isOccupied && tableCustomer.pizzasWanted > 0)
        {
            // 1. Find all attached pizzas in the entire scene
            GameObject[] allPizzas = GameObject.FindGameObjectsWithTag("SpineBox");
            if (allPizzas.Length == 0) yield break; // Player is out of pizza!

            // 2. Find the TOP pizza so we don't break the tower's base
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
                topPizza.tag = "Untagged"; // Take it off the menu immediately

                Rigidbody rb = topPizza.GetComponent<Rigidbody>();
                rb.isKinematic = true; // Turn off gravity and collisions!
                rb.detectCollisions = false; // Stop it from crashing the tower on its way out

                // 4. Animate it flying to the rat
                yield return StartCoroutine(FlyPizza(topPizza.transform));

                // 5. Update the Rat's stomach
                tableCustomer.pizzasWanted--;
                if (tableCustomer.pizzasWanted <= 0)
                {
                    tableCustomer.CompleteOrder();
                }
            }

            // Wait a moment before sucking the next one off the tower
            yield return new WaitForSeconds(timeBetweenPizzas);
        }
    }

    private IEnumerator FlyPizza(Transform pizza)
    {
        Vector3 startPos = pizza.position;
        Vector3 endPos = mouthTarget.position;
        float percent = 0;

        // Smoothly animate the pizza through the air
        while (percent < 1)
        {
            percent += Time.deltaTime / magnetFlightSpeed;
            if (pizza != null)
            {
                pizza.position = Vector3.Lerp(startPos, endPos, percent);
                // Optional: Make it spin like a frisbee while it flies
                pizza.Rotate(0, 720 * Time.deltaTime, 0);
            }
            yield return null;
        }

        // The rat ate it!
        if (pizza != null) Destroy(pizza.gameObject);
    }
}
