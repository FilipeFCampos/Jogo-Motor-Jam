using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;       
using TMPro;   

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen; // Painel de loading
    [SerializeField] private Image progressBar;       // Barra de progresso (UI Image)
    [SerializeField] private TMP_Text percentage;   // Texto (opcional)

    public void StartGame()
    {
        PlayerPrefs.SetInt("CurrentScore", 0); // Se usar um score temporário
        PlayerPrefs.Save();

        // Destrói o ScoreManager se ele existir (vindo da tela de vitória)
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }
        loadingScreen.SetActive(true); // Ativa o painel
        StartCoroutine(LoadLevelAsync(1)); // Índice da cena Level1
        Debug.Log($"Percentage is child of LoadingScreen: {percentage.transform.parent == loadingScreen.transform}");
    }

    IEnumerator LoadLevelAsync(int levelIndex)
    {
        // Garante que tudo está resetado
        progressBar.fillAmount = 0;
        percentage.text = "CARREGANDO: 0%";
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false;

        // Tempo mínimo de loading (2 segundos)
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
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}