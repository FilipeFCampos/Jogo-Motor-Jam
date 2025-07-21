using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen; // Painel de loading
    [SerializeField] private Image progressBar;        // Barra de progresso (UI Image)
    [SerializeField] private TMP_Text percentage;      // Texto de carregamento

    [Header("Músicas das Fases")]
    [SerializeField] private AudioClip fase1Music; // Música específica para a Fase 1
    // NOVO: Adicione as referências para as músicas das fases 2 e 3 aqui
    [SerializeField] private AudioClip fase2Music; // Música específica para a Fase 2
    [SerializeField] private AudioClip fase3Music; // Música específica para a Fase 3

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefaultBackgroundMusic();
        }
        else
        {
            Debug.LogWarning("MenuManager: AudioManager.Instance não encontrado em Start(). A música do menu não será tocada.");
        }
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("CurrentScore", 0);
        PlayerPrefs.Save();

        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        // CHAMANDO PARA A FASE 1
        StartCoroutine(LoadLevelAsync(1)); // Índice da cena Level1
    }

    // Se você tiver botões específicos para cada fase, pode criar métodos como este:
    public void LoadFase2Button()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        StartCoroutine(LoadLevelAsync(2)); // Índice da cena Level2
    }

    public void LoadFase3Button()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        StartCoroutine(LoadLevelAsync(3)); // Índice da cena Level3
    }


    IEnumerator LoadLevelAsync(int levelIndex)
    {
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
                    // === LÓGICA DE ÁUDIO PARA TRANSIÇÃO DE FASE AQUI ===
                    if (AudioManager.Instance != null)
                    {
                        AudioClip musicToPlay = null; // Variável para armazenar a música correta

                        // Seleciona a música baseada no índice da fase
                        switch (levelIndex)
                        {
                            case 1:
                                musicToPlay = fase1Music;
                                break;
                            case 2:
                                musicToPlay = fase2Music;
                                break;
                            case 3:
                                musicToPlay = fase3Music;
                                break;
                            // Adicione mais cases se tiver mais fases
                            default:
                                Debug.LogWarning($"MenuManager: Nenhuma música definida para o índice de fase {levelIndex}. Usando música padrão.");
                                musicToPlay = AudioManager.Instance.defaultBackgroundMusic; // Fallback para a música padrão
                                break;
                        }

                        // 1. Troca a música de fundo para a da fase selecionada
                        if (musicToPlay != null)
                        {
                            AudioManager.Instance.PlayBackgroundMusic(musicToPlay);
                            Debug.Log($"Música da Fase {levelIndex} ({musicToPlay.name}) solicitada ao AudioManager.");
                        }
                        else
                        {
                            Debug.LogWarning($"MenuManager: AudioClip da música para a Fase {levelIndex} não atribuído ou nulo após seleção!");
                        }

                        // 2. Ajusta o volume da música e dos SFX para os valores específicos da fase
                        // Você pode criar uma lógica mais complexa aqui (e.g., um array de volumes por fase)
                        // Por simplicidade, usaremos valores fixos aqui, mas você pode ajustá-los.
                        AudioManager.Instance.SetMusicVolume(0.6f); // Exemplo: Música um pouco mais alta na fase
                        AudioManager.Instance.SetSfxVolume(0.05f);   // Exemplo: SFX mais proeminente na fase
                        Debug.Log($"Volumes de áudio ajustados para a Fase {levelIndex}.");

                        // 3. Toca som de clique ao avançar da tela de loading
                        AudioManager.Instance.PlayButtonClickSound();
                    }
                    else
                    {
                        Debug.LogError("MenuManager: AudioManager.Instance não encontrado ao tentar ajustar áudio antes da transição da cena!");
                    }
                    // ====================================================

                    operation.allowSceneActivation = true; // Permite que a cena carregada seja ativada
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
        Debug.Log("Saindo do jogo...");
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
            AudioManager.Instance.PlayDefaultBackgroundMusic();
        }
        SceneManager.LoadScene("MenuPrincipal");
    }
}