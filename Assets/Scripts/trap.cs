using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject efeitoDaArmadilha;
    public float duracaoAnimacao = 1.0f;
    public int dano = 1;

    [Header("Cooldown")]
    public float tempoRecarga = 2f;
    private bool podeDarDano = true;

    private Animator anim;

    private void Start()
    {
        if (efeitoDaArmadilha != null)
        {
            anim = efeitoDaArmadilha.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && podeDarDano)
        {
            Debug.Log("Armadilha ativada!");

            // Ativa animação da armadilha
            if (anim != null)
            {
                anim.SetTrigger("Ativar");
                StartCoroutine(VoltarParaIdleDepois(duracaoAnimacao));
            }

            // Aplica dano ao jogador
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(dano);
                Debug.Log($"Dano aplicado: {dano} | Vida atual do jogador: {player.health}");

                StartCoroutine(RecarregarArmadilha());
            }
            else
            {
                Debug.LogWarning("O objeto com tag 'Player' não possui o script PlayerController!");
            }
        }
    }

    private IEnumerator VoltarParaIdleDepois(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (anim != null) anim.Play("Idle");
    }

    private IEnumerator RecarregarArmadilha()
    {
        podeDarDano = false;
        yield return new WaitForSeconds(tempoRecarga);
        podeDarDano = true;
    }
}
