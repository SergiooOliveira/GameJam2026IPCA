using UnityEngine;
using UnityEngine.Events; // 1. Necessário para usar UnityEvents

public class DeliveryPoint : MonoBehaviour
{
    [Header("Configurações de Aceitação")]
    [Tooltip("Arraste o Prefab que este ponto aceita (ex: prefab da Madeira ou da Pizza)")]
    public GameObject allowedPrefab;

    [Header("Configurações de Entrega")]
    [Tooltip("Tempo em segundos entre cada entrega (ex: descarregar 1 item a cada 0.2s)")]
    public float deliveryInterval = 0.2f;

    [Header("Eventos da Unity")]
    [Tooltip("O que acontece quando o item correto é entregue? Configura visualmente no Inspector!")]
    public UnityEvent OnDeliverySuccess;

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
        // 1. "Espreita" o último objeto removendo-o temporariamente para validação
        GameObject takenObj = playerInventory.RemoveLastObject();

        if (takenObj != null)
        {
            // Limpa o nome do objeto clonado para bater certo com o Prefab original
            string cleanObjName = takenObj.name.Replace("(Clone)", "").Trim();
            string cleanPrefabName = allowedPrefab.name.Trim();

            // 2. Verifica se o item retirado é o que este ponto aceita
            if (cleanObjName == cleanPrefabName)
            {
                // --- SUCESSO DE ENTREGA ---
                Debug.Log($"[DeliveryPoint] {cleanObjName} entregue com sucesso!");

                // 3. Dispara o evento visual que configuraste na Unity!
                OnDeliverySuccess?.Invoke();

                // Destrói o objeto físico imediatamente
                Destroy(takenObj);
            }
            else
            {
                // --- FALHA DE ENTREGA ---
                // Se o item não for o correto para este local, devolvemo-lo ao jogador
                playerInventory.PickUpObject(takenObj);
            }
        }
    }
}