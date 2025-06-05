using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score;
    
    private TMP_Text scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Atualiza o scoreText apenas em cenas que o contenham
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        AtualizarUI();
    }

    public void AddScore(int valor)
    {
        score += valor;
        SaveScore();
        AtualizarUI();
    }

    public void ResetScore()
    {
        score = 0;
        SaveScore();
        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.SetInt("TotalScore", score);
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        score = PlayerPrefs.GetInt("CurrentScore", 0);
    }

    public void ClearUI()
    {
        if (scoreText != null)
            scoreText.text = "";
    }

    public int GetScore() => score;
}
