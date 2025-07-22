using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Referências")]
    public string mainMenuScene = "MenuPrincipal";
    public string gameScene = "Level1";

    public void RetryGame()
    {
        PlayerPrefs.SetInt("CurrentScore", 0); // zera pontuação temporária
        PlayerPrefs.Save();

        // Destrói o player se ele ainda existir
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        // Destroi objetos persistentes
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }

        if (HUDPersistence.Instance != null)
        {
            Destroy(HUDPersistence.Instance.gameObject);
        }

        if (SceneTransitionManager.Instance != null)
        {
            DontDestroyOnLoad(SceneTransitionManager.Instance.gameObject);
        }

        Time.timeScale = 1f; // volta ao tempo normal do jogo

        // Reinicia a cena
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMainMenu()
    {
         // Destrói o player se ele ainda existir
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }

        if (HUDPersistence.Instance != null)
        {
            Destroy(HUDPersistence.Instance.gameObject);
        }

        if (FadePanelController.Instance != null)
        {
            Destroy(FadePanelController.Instance.gameObject);
        }

        if (SceneTransitionManager.Instance != null)
        {
            DontDestroyOnLoad(SceneTransitionManager.Instance.gameObject);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void ShowButtons(float delay)
    {
        Invoke("EnableButtons", delay);
    }

    private void EnableButtons()
    {
        foreach (var btn in GetComponentsInChildren<Button>(true))
        {
            btn.gameObject.SetActive(true);
        }
    }
}
