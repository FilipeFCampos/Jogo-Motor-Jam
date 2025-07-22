using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;  // não esqueça de importar

public class FadePanelController : MonoBehaviour
{
     public static FadePanelController Instance { get; private set; }
    public Image fadeImage;
    public float fadeDuration = 1f;

    // Novo campo para o texto
    public TextMeshProUGUI phaseText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o FadePanel não será destruído
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0, 0, 0, 0); // transparente no início
        }

        gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        phaseText.gameObject.SetActive(true);

        float t = 0;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        // Também pode esconder o texto se estiver usando
        if (phaseText != null)
            phaseText.gameObject.SetActive(false);
            
        // Esconde o painel após o fade in
        fadeImage.gameObject.SetActive(false);
 
    }

    // Método para mudar o texto da fase
    public void SetPhaseText(string text)
    {
        if (phaseText != null)
        {
            phaseText.text = text;
        }
    }
}
