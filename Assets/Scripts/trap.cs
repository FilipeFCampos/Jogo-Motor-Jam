using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public GameObject efeitoDaArmadilha; // O GameObject que tem o Animator
    public float duracaoAnimacao = 1.0f; // Tempo da anima��o Ativar em segundos

    private Animator anim;

    private void Start()
    {
        if (efeitoDaArmadilha != null)
        {
            anim = efeitoDaArmadilha.GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Animator n�o encontrado no efeitoDaArmadilha!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && anim != null)
        {
            Debug.Log("Jogador ativou a armadilha!");

            anim.SetTrigger("Ativar"); // Dispara a anima��o

            // Come�a coroutine para voltar para o estado Idle depois da anima��o
            StartCoroutine(VoltarParaIdleDepois(duracaoAnimacao));
        }
    }

    private IEnumerator VoltarParaIdleDepois(float delay)
    {
        yield return new WaitForSeconds(delay);

        anim.Play("Idle"); // Volta para o estado inicial "Idle"
    }
}
