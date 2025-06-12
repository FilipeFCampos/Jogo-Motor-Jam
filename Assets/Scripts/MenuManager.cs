using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen; // Painel de loading
    [SerializeField] private Image progressBar;       // Barra de progresso (UI Image)
    [SerializeField] private TMP_Text percentage;     // Texto de carregamento

    private void Start()
    {
        // Toca a música do menu ao iniciar
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("CurrentScore", 0); // Reset do score temporário
        PlayerPrefs.Save();

        // Destrói o ScoreManager se ele existir
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }

        // Toca som de clique ao iniciar o jogo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        loadingScreen.SetActive(true); // Exibe tela de carregamento
        StartCoroutine(LoadLevelAsync(1)); // Índice da cena Level1
    }

    IEnumerator LoadLevelAsync(int levelIndex)
    {
        // Reset da UI de carregamento
        progressBar.fillAmount = 0;
        percentage.text = "CARREGANDO: 0%";
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false;

        float minDisplayTime = 2f;
        float elapsedTime = 0f;

        while (!operation.isDone || elapsedTime < minDisplayTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.fillAmount = progress;
            percentage.text = $"CARREGANDO: {progress * 100:F0}%";

            if (operation.progress >= 0.9f && elapsedTime >= minDisplayTime)
            {
                percentage.text = "PRESSIONE QUALQUER TECLA";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;

                    // Som ao avançar da tela de loading
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayButtonClickSound();
                    }
                }
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        Application.Quit();
    }

    public void OpenCredits()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        SceneManager.LoadScene("Credits");
    }

    public void OpenMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        SceneManager.LoadScene("MenuPrincipal");
    }
}
