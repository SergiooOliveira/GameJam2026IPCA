using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Configura��es de Carga")]
    [Tooltip("Ponto vazio (Transform) nas m�os ou costas do jogador onde os objetos v�o acumular")]
    public Transform stackPosition;
    public float verticalOffset = 0.3f;
    public int maxCarryLimit = 5;

    private List<GameObject> carriedObjects = new List<GameObject>();

    // Fun��o que o Spawner vai chamar para dar o objeto ao Jogador
    public bool PickUpObject(GameObject objToPickUp, float prefabYSize)
    {
        if (carriedObjects.Count >= maxCarryLimit)
        {
            Debug.Log("Invent�rio cheio!");
            return false; // N�o conseguiu apanhar
        }

        // 1. Tornar o objeto "filho" do ponto de stack do jogador
        objToPickUp.transform.SetParent(stackPosition);

        // 2. Calcular a posi��o dele na pilha do jogador
        float newY = carriedObjects.Count * (prefabYSize + verticalOffset);
        objToPickUp.transform.localPosition = new Vector3(0, newY, 0);
        objToPickUp.transform.localRotation = Quaternion.identity; // Alinha a rota��o

        // 3. Adicionar � lista do jogador
        carriedObjects.Add(objToPickUp);
        return true; // Sucesso!
    }

    // Fun��o que o Ponto de Recolha vai chamar para retirar o �ltimo objeto da pilha do jogador
    public GameObject RemoveLastObject()
    {
        if (carriedObjects.Count == 0) return null;

        // Pega no �ltimo objeto (o do topo da pilha)
        int lastIndex = carriedObjects.Count - 1;
        GameObject objToRemove = carriedObjects[lastIndex];

        // Remove da lista do jogador
        carriedObjects.RemoveAt(lastIndex);

        // Desvincula o objeto do jogador (deixa de ser "filho" dele)
        objToRemove.transform.SetParent(null);

        return objToRemove;
    }
}