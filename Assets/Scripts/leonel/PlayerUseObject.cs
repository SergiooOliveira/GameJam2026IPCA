using UnityEngine;

public class PlayerUseObject : MonoBehaviour
{
    private PlayerInventory inventory;
    private bool estaAutilizarExtintor = false;
    private FireExtinguisher extintorAtual = null;

    // Propriedade pública caso queiras ler noutro script se o extintor está ativo
    public bool EstaAutilizarExtintor => estaAutilizarExtintor;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        // Se o extintor estiver ligado, mas o jogador o largar da mão, desliga o uso imediatamente
        if (estaAutilizarExtintor && !inventory.TemExtintorNaMao)
        {
            Debug.Log("[UseObject] Extintor foi largado da mão. Desligando automaticamente.");
            DesligarExtintor();
            return;
        }

        // Lógica contínua do Spray frame a frame (se estiver ligado)
        if (estaAutilizarExtintor && extintorAtual != null)
        {
            float gasUsed = extintorAtual.Spray();

            if (gasUsed > 0f)
            {
                Debug.Log($"[UseObject] Extintor [LIGADO] -> Gás restante: {extintorAtual.currentGas:F1}");

                // [Próxima etapa: Aplicar o gasUsed no PizzaFurnace do teu colega]
            }
            else
            {
                Debug.Log("[UseObject] O extintor ficou sem gás! Desligando.");
                DesligarExtintor();
            }
        }
    }

    // --- Vincula esta função ao "On Click ()" do teu botão da UI ---
    public void BotaoToggleInteract()
    {
        // Se já está a disparar -> CLICOU PARA DESLIGAR
        if (estaAutilizarExtintor)
        {
            Debug.Log("[UseObject] Clique detetado: A desligar o extintor.");
            DesligarExtintor();
        }
        // Se está desligado -> CLICOU PARA LIGAR
        else
        {
            if (inventory != null && inventory.TemExtintorNaMao)
            {
                if (inventory.ExtintorSegurado.TryGetComponent<FireExtinguisher>(out extintorAtual))
                {
                    estaAutilizarExtintor = true;
                    Debug.Log("[UseObject] Clique detetado: Extintor [LIGADO] com sucesso!");
                }
            }
            else
            {
                Debug.LogWarning("[UseObject] Não podes ligar o Interact porque não tens o extintor na mão!");
            }
        }
    }

    private void DesligarExtintor()
    {
        estaAutilizarExtintor = false;
        extintorAtual = null;
        Debug.Log("[UseObject] Extintor [DESLIGADO].");
    }
}