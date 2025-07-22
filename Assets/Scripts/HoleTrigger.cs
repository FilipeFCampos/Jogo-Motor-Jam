using UnityEngine;
using UnityEngine.SceneManagement;

public class HoleTrigger : MonoBehaviour
{
    public string nextSceneName;
    public string phaseText;

    private OrcController orcController;
    public Collider2D solidCollider;
    public Collider2D triggerCollider;

    public string playerSpawnPoint;

    private bool transitioning = false;

    private void Start()
    {
        TryFindOrc();

        if (solidCollider != null)
        {
            solidCollider.enabled = true;
            Debug.Log("[Start] solidCollider habilitado: true");
        }
        else
        {
            Debug.LogWarning("[Start] solidCollider não atribuído!");
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
            Debug.Log("[Start] triggerCollider habilitado: false");
        }
        else
        {
            Debug.LogWarning("[Start] triggerCollider não atribuído!");
        }
    }

    private void Update()
    {
        if (orcController == null)
        {
            TryFindOrc();
            return; // aguarda próximo frame para tentar de novo
        }

        if (orcController.IsDead())
        {
            Debug.Log("[HoleTrigger] Orc está morto. Permitindo passagem.");
            solidCollider.enabled = false;
            triggerCollider.enabled = true;
        }
        else
        {
            solidCollider.enabled = true;
            triggerCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transitioning || orcController == null) return;

        if (collision.CompareTag("Player") && orcController.IsDead())
        {
            Debug.Log("Entrei no buraco vou mudar de cena");
            transitioning = true;
            SceneTransitionManager.Instance.LoadSceneWithFade(nextSceneName, phaseText, playerSpawnPoint);
        }
    }

    private void TryFindOrc()
    {
        orcController = FindFirstObjectByType<OrcController>();

        if (orcController != null)
        {
            Debug.Log("[HoleTrigger] Orc encontrado.");
        }
        else
        {
            Debug.LogWarning("[HoleTrigger] Orc ainda não foi encontrado.");
        }
    }
}
