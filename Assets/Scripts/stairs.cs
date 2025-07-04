using UnityEngine;
using System.Collections;

public class TeleportOnTouch : MonoBehaviour
{
    public Transform targetPosition; // Para onde o player será movido
    public FadePanelController fadePanel; // Arraste aqui o FadePanel no Inspector

    private bool isTeleporting = false;

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

        // Fade para preto
        yield return StartCoroutine(fadePanel.FadeOut());

        // Teleporta o player
        player.position = targetPosition.position;
        Debug.Log("Teleportado para nova sala.");

        // Espera brevemente
        yield return new WaitForSeconds(0.1f);

        // Fade de volta
        yield return StartCoroutine(fadePanel.FadeIn());

        isTeleporting = false;
    }
}
