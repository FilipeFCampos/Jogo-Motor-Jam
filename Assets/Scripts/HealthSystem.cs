using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite fullHeart; // hearts_0
    public Sprite emptyHeart; // hearts_4

    [Header("Configuração")]
    public int maxHearts = 5;
    public Image[] heartImages; // Heart1, Heart2... Heart5

    public static HealthSystem Instance;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeHearts();
    }

    // Chamado pelo PlayerController ao tomar dano
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
        StartCoroutine(FlashHearts());
    }

    // Inicializa todos os corações como cheios
    private void InitializeHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = fullHeart;
            heartImages[i].enabled = (i < maxHearts); // Desativa corações extras
        }
    }

    IEnumerator FlashHearts()
    {
        foreach (var heart in heartImages)
        {
            if (heart.sprite == fullHeart) // Só pisca corações cheios
                heart.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        foreach (var heart in heartImages)
        {
            heart.color = Color.white;
        }
    }
}