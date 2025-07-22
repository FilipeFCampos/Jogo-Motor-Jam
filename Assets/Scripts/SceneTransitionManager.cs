using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private FadePanelController fadePanel;



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Apenas se for o único
    }

    void Start()
    {
         fadePanel = FadePanelController.Instance;
        if (fadePanel == null)
        {
            Debug.LogWarning("FadePanelController.Instance é null! Verifique se o FadePanel está presente e foi inicializado.");
        }
    }

    public void LoadSceneWithFade(string sceneName, string phaseText, string playerSpawnPoint)
    {
        Debug.Log($"[SceneTransitionManager] Iniciando transição para a cena '{sceneName}' com texto '{phaseText}'.");
        StartCoroutine(DoTransition(sceneName, phaseText, playerSpawnPoint));
    }

    private IEnumerator DoTransition(string sceneName, string phaseText, string playerSpawn)
    {
        if (fadePanel == null)
        {
            Debug.LogError("[SceneTransitionManager] fadePanel não está atribuído!");
            yield break;
        }

        fadePanel.gameObject.SetActive(true);


        /*GameObject menuIncial = GameObject.FindWithTag("MenuInicial");
        if (menuIncial != null)
        {
            menuIncial.SetActive(false);
        }*/

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            DontDestroyOnLoad(player);
        }

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

        // Atualiza o texto da fase e ativa o texto
        if (phaseText != null)
        {
            fadePanel.SetPhaseText(phaseText);
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

        //Move o player pro spawnPointer do level
        if (player != null && !string.IsNullOrEmpty(playerSpawn))
        {
            GameObject spawnPoint = GameObject.Find(playerSpawn);
            if (spawnPoint != null)
            {
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.MoveGameObjectToScene(player, activeScene);
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"[SceneTransitionManager] Player posicionado no spawn '{playerSpawn}'.");
            }
            else
            {
                Debug.LogWarning($"[SceneTransitionManager] SpawnPoint '{playerSpawn}' não encontrado.");
            }
        }

        // Fade in (texto some ao final do fade in)
        Debug.Log("[SceneTransitionManager] Iniciando FadeIn...");
        yield return StartCoroutine(fadePanel.FadeIn());
        Debug.Log("[SceneTransitionManager] FadeIn concluído.");

        // Reativa o HUD após a transição
        if (HUDPersistence.Instance != null)
        {
            if (sceneName != "WinScene")
            {
                HUDPersistence.Instance.SetActiveHUD(true);
                Debug.Log("[SceneTransitionManager] HUD reativado.");
            }
            else
            {
                HUDPersistence.Instance.SetActiveHUD(false);
            }
        }else{
            Debug.LogWarning("[SceneTransitionManager] HUDPersistence.Instance é null ao tentar reativar o HUD.");
        }
    }

    public void LoadSceneNoFade(string sceneName)
    {
        Debug.Log("oioii");
        SceneManager.LoadScene(sceneName);
    }
    

}
