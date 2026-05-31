using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    [Header("Restaurant Settings")]
    public List<RatCustomer> allSeats;
    public int maxSimultaneousCustomers = 5;
    public float timeBetweenSpawns = 4f;

    [Header("Order Settings")]
    public int minPizzasPerOrder = 1;
    public int maxPizzasPerOrder = 3;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            TrySpawnCustomer();
            timer = 0f;
        }
    }

    private void TrySpawnCustomer()
    {
        Debug.Log("Trying to Spawn");
        List<RatCustomer> emptySeats = new List<RatCustomer>();
        int currentActiveCustomers = 0;

        foreach (RatCustomer seat in allSeats)
        {
            if (seat.isOccupied)
                currentActiveCustomers++;
            else
                emptySeats.Add(seat);
        }

        if (currentActiveCustomers >= maxSimultaneousCustomers || emptySeats.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, emptySeats.Count);
        emptySeats[randomIndex].ActivateCostumer(minPizzasPerOrder, maxPizzasPerOrder);
    }
}
