using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public FadePanelController fadePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneTransitionManager] Instância criada e marcada como DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("[SceneTransitionManager] Instância duplicada detectada. Destruindo esta.");
            Destroy(gameObject);
        }
    }

    public void LoadSceneWithFade(string sceneName, string phaseText)
    {
        Debug.Log($"[SceneTransitionManager] Iniciando transição para a cena '{sceneName}' com texto '{phaseText}'.");
        StartCoroutine(DoTransition(sceneName, phaseText));
    }

    private IEnumerator DoTransition(string sceneName, string phaseText)
    {
        if (fadePanel == null)
        {
            Debug.LogError("[SceneTransitionManager] fadePanel não está atribuído!");
            yield break;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            DontDestroyOnLoad(player);
        }

        // Atualiza o texto da fase e ativa o texto
        fadePanel.SetPhaseText(phaseText);
        Debug.Log("[SceneTransitionManager] Texto da fase definido no FadePanel.");

        // Desativa o HUD no início da transição
        if (HUDPersistence.Instance != null)
        {
            HUDPersistence.Instance.SetActiveHUD(false);
            Debug.Log("[SceneTransitionManager] HUD desativado.");
        }
        else
        {
            Debug.LogWarning("[SceneTransitionManager] HUDPersistence.Instance é null ao tentar desativar o HUD.");
        }

        // Fade out (com texto visível)
        Debug.Log("[SceneTransitionManager] Iniciando FadeOut...");
        yield return StartCoroutine(fadePanel.FadeOut());
        Debug.Log("[SceneTransitionManager] FadeOut concluído.");

        // Carrega a nova cena
        Debug.Log($"[SceneTransitionManager] Carregando cena '{sceneName}'...");
        SceneManager.LoadScene(sceneName);

        // Pequena espera para garantir que a cena carregou
        yield return new WaitForSeconds(0.1f);
        Debug.Log("[SceneTransitionManager] Espera após carregar a cena concluída.");

        // Fade in (texto some ao final do fade in)
        Debug.Log("[SceneTransitionManager] Iniciando FadeIn...");
        yield return StartCoroutine(fadePanel.FadeIn());
        Debug.Log("[SceneTransitionManager] FadeIn concluído.");

        // Reativa o HUD após a transição
        if (HUDPersistence.Instance != null)
        {
            HUDPersistence.Instance.SetActiveHUD(true);
            Debug.Log("[SceneTransitionManager] HUD reativado.");
        }
        else
        {
            Debug.LogWarning("[SceneTransitionManager] HUDPersistence.Instance é null ao tentar reativar o HUD.");
        }
    }
}
