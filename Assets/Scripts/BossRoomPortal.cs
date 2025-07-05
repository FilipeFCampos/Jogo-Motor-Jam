using UnityEngine;
using System.Collections;

public class BossRoomPortal : MonoBehaviour
{
    public Transform bossSpawnPoint;
    public FadePanelController fadePanel; // Referência ao painel de fade


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController inventory = other.GetComponent<PlayerController>();
            if (inventory != null && inventory.hasKey)
            {
                StartCoroutine(EnterBossRoom(other.transform));
            }
            else
            {
                Debug.Log("Você precisa da chave para entrar!");
            }
        }
    }

    private IEnumerator EnterBossRoom(Transform player)
     {

        Debug.Log("Entrei no sala");
        // Fade para preto
        yield return StartCoroutine(fadePanel.FadeOut());

        // Teleporta o player
        player.position = bossSpawnPoint.position;
        Debug.Log("Player entrou na sala do boss!");

        // Espera um pouquinho se quiser dar mais tempo (opcional)
        yield return new WaitForSeconds(0.2f);

        // Fade para o jogo novamente
        yield return StartCoroutine(fadePanel.FadeIn());
    }
}
