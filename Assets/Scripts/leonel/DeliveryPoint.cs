using UnityEngine;
using System.Collections.Generic;

public class DeliveryPoint : MonoBehaviour
{
    [Header("Configurações de Entrega")]
    [Tooltip("Tempo em segundos entre cada entrega (ex: descarregar 1 item a cada 0.2s)")]
    public float deliveryInterval = 0.2f;

    [Header("Configurações de Armazenamento (Opcional)")]
    [Tooltip("Se ativado, os objetos entregues vão empilhar aqui também")]
    public bool stackDeliveredObjects = true;
    public Transform stackPosition;
    public float verticalOffset = 0.3f;

    // Lista interna caso queiras guardar os objetos visualmente no ponto de recolha
    private List<GameObject> deliveredObjects = new List<GameObject>();
    private float timer = 0.0f;

    private void OnTriggerStay(Collider other)
    {
        // Enquanto o jogador estiver na área de entrega
        if (other.TryGetComponent<PlayerInventory>(out PlayerInventory inventory))
        {
            timer += Time.deltaTime;

            if (timer >= deliveryInterval)
            {
                TryTakeObjectFromPlayer(inventory);
                timer = 0.0f; // Reinicia o tempo para a próxima entrega
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Quando o jogador sai, reinicia o temporizador
        if (other.CompareTag("Player"))
        {
            timer = 0.0f;
        }
    }

    void TryTakeObjectFromPlayer(PlayerInventory playerInventory)
    {
        // Tenta retirar o último objeto do jogador
        GameObject takenObj = playerInventory.RemoveLastObject();

        if (takenObj != null)
        {
            // --- CÓDIGO DE SUCESSO DE ENTREGA ---
            Debug.Log($"Objeto {takenObj.name} entregue com sucesso!");

            // Se quisermos que eles empilhem no ponto de recolha (tipo um armazém)
            if (stackDeliveredObjects && stackPosition != null)
            {
                takenObj.transform.SetParent(stackPosition);
                float newY = deliveredObjects.Count * verticalOffset;
                takenObj.transform.localPosition = new Vector3(0, newY, 0);
                takenObj.transform.localRotation = Quaternion.identity;

                deliveredObjects.Add(takenObj);
            }
            else
            {
                // Se não queres acumular no cenário, podes simplesmente destruí-lo 
                // ou desativá-lo para poupar performance (Object Pooling seria o ideal, mas para Jam isto serve!)
                Destroy(takenObj);
            }

            // EXTRAS DA JAM: Podes adicionar aqui chamadas para o teu GameManager 
            // para somar moedas ou pontos! ex: GameManager.Instance.AddScore(10);
        }
    }
}