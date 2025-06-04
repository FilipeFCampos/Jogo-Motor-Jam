using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    
    [Header("Cenas")]
    public string mainMenu = "MenuPrincipal";

    void Start()
    {
        // Carrega dados do jogador
        int slimesDefeated = PlayerPrefs.GetInt("SlimesDefeated", 0);
        float playTime = PlayerPrefs.GetFloat("PlayTime", 0f);
        
        // Atualiza UI
        scoreText.text = $"Slimes Derrotados: {slimesDefeated}";
        timeText.text = $"Tempo: {FormatTime(playTime)}";
        
        // Opcional: Toca som de vitória
        // AudioManager.Instance.Play("Victory");
    }

    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}