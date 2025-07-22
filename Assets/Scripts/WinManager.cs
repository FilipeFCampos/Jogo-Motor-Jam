using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Data.Common;

public class WinManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    [Header("Settings")]
    [SerializeField] private string scorePrefix = "TOTAL SCORE: ";
    [SerializeField] private string timePrefix = "PLAYING TIME: ";

    void Start()
    {
        LoadAndDisplayGameData();
    }

    private void LoadAndDisplayGameData()
    {
        // Carrega dados persistentes
        GameData data = new GameData
        {
            score = PlayerPrefs.GetInt("TotalScore", 0),
            playTime = PlayerPrefs.GetFloat("PlayTime", 0f)
        };

        // Atualiza UI
        UpdateUI(data);
    }

    private void UpdateUI(GameData data)
    {
        scoreText.text = $"{scorePrefix}{data.score}";
        timeText.text = $"{timePrefix}{FormatTime(data.playTime)}";
    }

    private string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void ReturnToMenu()
    {
            // Destrói o ScoreManager (se existir)
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
        SceneManager.LoadScene("MenuPrincipal");
    }
    
    // Classe auxiliar para organização dos dados
    private class GameData
    {
        public int score;
        public float playTime;
    }

}