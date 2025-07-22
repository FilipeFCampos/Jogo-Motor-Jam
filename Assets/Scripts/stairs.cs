using UnityEngine;
using System.Collections;

public class TeleportOnTouch : MonoBehaviour
{
    public Transform targetPosition; // Para onde o player será movido
    private FadePanelController fadePanel;

    private bool isTeleporting = false;
    void Start()
    {
        fadePanel = FadePanelController.Instance;
        if (fadePanel == null)
        {
            Debug.LogWarning("FadePanelController.Instance é null! Verifique se o FadePanel está presente e foi inicializado.");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFade(other.transform));
        }
    }

    private IEnumerator TeleportWithFade(Transform player)
    {
        isTeleporting = true;

        fadePanel.SetPhaseText(null);

        // Desativa o controle do jogador
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }

        // Fade para preto
        yield return StartCoroutine(fadePanel.FadeOut());

        // Teleporta o player
        player.position = targetPosition.position;
        Debug.Log("Teleportado para nova sala.");

        // Espera brevemente
        yield return new WaitForSeconds(0.2f);

        // Fade de volta
        yield return StartCoroutine(fadePanel.FadeIn());

        playerController.SetMovementEnabled(true);

        isTeleporting = false;
    }
}
