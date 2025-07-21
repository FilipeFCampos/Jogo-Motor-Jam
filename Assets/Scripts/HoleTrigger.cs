using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class HoleTrigger : MonoBehaviour
{
    public string nextSceneName;
    public string phaseText;
    private BossController bossController;
    public Collider2D solidCollider;
    public Collider2D triggerCollider;
    private bool transitioning = false;

    private void Start()
    {
        bossController = Object.FindFirstObjectByType<BossController>();

        if (solidCollider != null)
        {
            solidCollider.enabled = true;
            Debug.Log($"[Start] solidCollider habilitado: {solidCollider.enabled}");
        }
        else
        {
            Debug.LogWarning("[Start] solidCollider não está atribuído!");
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
            Debug.Log($"[Start] triggerCollider habilitado: {triggerCollider.enabled}");
        }
        else
        {
            Debug.LogWarning("[Start] triggerCollider não está atribuído!");
        }
    }

    private void Update()
    {
        if (bossController == null)
        {
            bossController = Object.FindFirstObjectByType<BossController>();
            if (bossController == null)
            {
                Debug.Log("[HoleTrigger] Boss não encontrado (morto). Permitindo passagem.");
                solidCollider.enabled = false;
                triggerCollider.enabled = true;
                return;
            }
            else
            {
                Debug.Log("Achou pelo Find");
            }
        }

        if (bossController.IsDead())
        {
            Debug.Log("[HoleTrigger] Boss está morto. Permitindo passagem.");
            solidCollider.enabled = false;
            triggerCollider.enabled = true;
        }
        else
        {
            Debug.Log("[HoleTrigger] Boss ainda está vivo.");
            solidCollider.enabled = true;
            triggerCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transitioning) return;

        if (collision.CompareTag("Player") && bossController != null && bossController.IsDead())
        {
            transitioning = true;
            SceneTransitionManager.Instance.LoadSceneWithFade(nextSceneName, phaseText);
        }
    }
}
