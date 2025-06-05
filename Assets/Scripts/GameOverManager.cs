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
        PlayerPrefs.SetInt("CurrentScore", 0); // Se usar um score temporário
        PlayerPrefs.Save();

        // Destrói o ScoreManager se ele existir (vindo da tela de vitória)
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }
        // Destrói o ScoreManager (se existir)
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }
        Time.timeScale = 1f; // retoma o jogo
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMainMenu()
    {
            // Destrói o ScoreManager (se existir)
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    // Opcional: Ativar/desativar objetos com animação
    public void ShowButtons(float delay)
    {
        Invoke("EnableButtons", delay);
    }

    private void EnableButtons()
    {
        // Ativar botões após animação
        foreach (var btn in GetComponentsInChildren<Button>(true))
        {
            btn.gameObject.SetActive(true);
        }
    }
}