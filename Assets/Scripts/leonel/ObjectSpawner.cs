using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Configura��es do Objeto")]
    public GameObject objectToSpawn;

    [Header("Configura��es de Tempo")]
    public float spawnInterval = 2.0f;

    [Header("Configura��es da Pilha (Spawner)")]
    public float verticalOffset = 0.5f;
    public int maxStackSize = 10;

    private List<GameObject> objectStack = new List<GameObject>();
    private float timer = 0.0f;

    void Update()
    {
        // L�gica de Spawn Cont�nuo
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            if (objectStack.Count < maxStackSize)
            {
                SpawnInStack();
            }
            timer = 0.0f;
        }
    }

    void SpawnInStack()
    {
        if (objectToSpawn == null) return;

        Vector3 spawnPosition = transform.position;

        // Se j� houver itens na pilha, calcula a nova altura (Y)
        if (objectStack.Count > 0)
        {
            GameObject lastObject = objectStack[objectStack.Count - 1];
            spawnPosition = new Vector3(transform.position.x, lastObject.transform.position.y + verticalOffset, transform.position.z);
        }

        GameObject newObj = Instantiate(objectToSpawn, spawnPosition, transform.rotation);

        // Desativa a f�sica 3D para n�o desmanchar a pilha
        if (newObj.TryGetComponent<Rigidbody>(out Rigidbody rb3D))
        {
            rb3D.isKinematic = true;
        }

        objectStack.Add(newObj);
    }

    void TryGiveObjectToPlayer(PlayerInventory playerInventory)
    {
        // Se houver objetos na banca
        if (objectStack.Count > 0)
        {
            // Espreita o �ltimo objeto
            GameObject targetObj = objectStack[objectStack.Count - 1];

            // Tenta dar ao jogador
            bool success = playerInventory.PickUpObject(targetObj);

            if (success)
            {
                // Remove da lista se o jogador conseguiu apanhar
                objectStack.RemoveAt(objectStack.Count - 1);
            }
        }
    }

    // --- DETE��O DE PROXIMIDADE 3D (AUTOM�TICA) ---

    private void OnTriggerStay(Collider other)
    {
        // Enquanto o jogador estiver dentro do Trigger, tenta entregar objetos continuamente
        if (other.TryGetComponent<PlayerInventory>(out PlayerInventory inventory))
        {
            TryGiveObjectToPlayer(inventory);
        }
    }
}