using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Configurações da Pilha (Lenha/Objs)")]
    [Tooltip("Ponto onde os objetos normais vão acumular (ex: costas)")]
    public Transform stackPosition;
    public float verticalOffset = 0.3f;
    public int maxCarryLimit = 5;

    [Header("Configuração do Extintor (Mão)")]
    [Tooltip("Ponto específico onde o extintor vai ficar preso (ex: mão do jogador)")]
    public Transform extintorSpawnPoint;

    [Header("Configurações Gerais Mobile")]
    [SerializeField] float maxInteractDistance = 4f;

    // Armazenamento separado
    private List<GameObject> carriedObjects = new List<GameObject>(); // Pilha de lenha/objs
    private GameObject extintorSegurado = null; // Apenas o extintor

    // Propriedades públicas para o PlayerUseObject consultar
    public bool TemExtintorNaMao => extintorSegurado != null;
    public GameObject ExtintorSegurado => extintorSegurado;


    // --- Vincula esta função ao botão "Pick Up" da UI Mobile ---
    public void BotaoTogglePickUp()
    {
        Debug.Log("[Inventory] Botão Pick Up clicado!");

        // Se já temos um extintor na mão, o botão de Pick Up serve primeiro para o LARGAR
        if (extintorSegurado != null)
        {
            Debug.Log($"[Inventory] Jogador já tem o extintor [{extintorSegurado.name}] na mão. Prioridade: Largar.");
            LargerExtintor();
            return;
        }

        // Procura o objeto com a tag "TogglePickUp" mais próximo no chão
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("TogglePickUp");

        if (interactableObjects.Length == 0)
        {
            Debug.LogWarning("[Inventory] ATENÇÃO: Nenhum objeto encontrado na cena com a tag 'TogglePickUp'!");
        }

        GameObject closestObj = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in interactableObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObj = obj;
            }
        }

        // Se encontrou um objeto dentro do alcance
        if (closestObj != null && closestDistance <= maxInteractDistance)
        {
            Debug.Log($"[Inventory] Objeto mais próximo detetado: [{closestObj.name}] a {closestDistance:F2}m de distância.");

            // VERIFICAÇÃO: É um extintor?
            if (closestObj.GetComponent<FireExtinguisher>() != null)
            {
                Debug.Log($"[Inventory] O objeto [{closestObj.name}] tem o script FireExtinguisher. Alvo: Mão.");
                PegarExtintor(closestObj);
            }
            else // Se não for extintor, assume que é lenha/objeto de pilha
            {
                Debug.Log($"[Inventory] O objeto [{closestObj.name}] NÃO é um extintor. Alvo: Pilha de Carga.");

                if (carriedObjects.Count < maxCarryLimit)
                {
                    // Prepara física do objeto de pilha
                    if (closestObj.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
                    if (closestObj.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

                    PickUpObject(closestObj);
                }
                else
                {
                    Debug.LogWarning($"[Inventory] Falha ao pegar: Pilha de objetos cheia! ({carriedObjects.Count}/{maxCarryLimit})");
                }
            }
        }
        // Se detetou o objeto mas está fora do alcance máximo
        else if (closestObj != null && closestDistance > maxInteractDistance)
        {
            Debug.Log($"[Inventory] Objeto [{closestObj.name}] está muito longe! Distância: {closestDistance:F2}m | Limite máximo: {maxInteractDistance}m.");
            TentarLargarUltimoDaPilha();
        }
        // Se não há absolutamente nenhum objeto por perto, tenta largar o que está no inventário
        else
        {
            Debug.Log("[Inventory] Nenhum objeto físico por perto para interagir.");
            TentarLargarUltimoDaPilha();
        }
    }

    // Função auxiliar para evitar repetição de código nos logs
    private void TentarLargarUltimoDaPilha()
    {
        if (carriedObjects.Count > 0)
        {
            Debug.Log($"[Inventory] Ação: Largar o último objeto da pilha de carga (Total atual: {carriedObjects.Count}).");
            GameObject largado = RemoveLastObject();
            if (largado != null)
            {
                if (largado.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = false;
                if (largado.TryGetComponent<Collider>(out Collider col)) col.enabled = true;
            }
        }
        else
        {
            Debug.Log("[Inventory] Nada para fazer: Mãos vazias e pilha de carga vazia.");
        }
    }

    // --- LÓGICA EXCLUSIVA DO EXTINTOR ---
    private void PegarExtintor(GameObject extintor)
    {
        extintorSegurado = extintor;

        if (extintorSegurado.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
        if (extintorSegurado.TryGetComponent<Collider>(out Collider col)) col.enabled = false;

        // Prende no ponto exclusivo do extintor
        extintorSegurado.transform.SetParent(extintorSpawnPoint);
        extintorSegurado.transform.localPosition = Vector3.zero;
        extintorSegurado.transform.localRotation = Quaternion.identity;

        Debug.Log($"[Inventory] SUCESSO: Extintor [{extintorSegurado.name}] posicionado e travado no 'extintorSpawnPoint'.");
    }

    private void LargerExtintor()
    {
        if (extintorSegurado == null) return;

        Debug.Log($"[Inventory] Ação: A largar o extintor [{extintorSegurado.name}] no chão.");

        extintorSegurado.transform.SetParent(null);

        if (extintorSegurado.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = false;
        if (extintorSegurado.TryGetComponent<Collider>(out Collider col)) col.enabled = true;

        extintorSegurado = null;
    }


    // --- LÓGICA ORIGINAL DA PILHA (Mantida intacta para o teu spawner/recolha) ---
    public bool PickUpObject(GameObject objToPickUp)
    {
        if (carriedObjects.Count >= maxCarryLimit)
        {
            Debug.LogWarning($"[Inventory] Erro de Stack: Limite máximo atingido! ({carriedObjects.Count}/{maxCarryLimit})");
            return false;
        }

        objToPickUp.transform.SetParent(stackPosition);

        float newY = carriedObjects.Count * verticalOffset;
        objToPickUp.transform.localPosition = new Vector3(0, newY, 0);
        objToPickUp.transform.localRotation = Quaternion.identity;

        carriedObjects.Add(objToPickUp);
        Debug.Log($"[Inventory] SUCESSO: [{objToPickUp.name}] empilhado. Itens na pilha: {carriedObjects.Count}/{maxCarryLimit}");
        return true;
    }

    public GameObject RemoveLastObject()
    {
        if (carriedObjects.Count == 0)
        {
            Debug.Log("[Inventory] Erro de Desempilhamento: A lista de carregamento já está vazia!");
            return null;
        }

        int lastIndex = carriedObjects.Count - 1;
        GameObject objToRemove = carriedObjects[lastIndex];

        carriedObjects.RemoveAt(lastIndex);
        objToRemove.transform.SetParent(null);

        Debug.Log($"[Inventory] SUCESSO: [{objToRemove.name}] removido da pilha. Restantes: {carriedObjects.Count}");
        return objToRemove;
    }
}