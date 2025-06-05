using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int valor = 1;
    private AudioSource audioSource;
    private bool coletado = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!coletado && other.CompareTag("Player"))
        {
            coletado = true;

            ScoreManager.Instance.AddScore(valor);

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
                Destroy(gameObject, audioSource.clip.length); // espera o som terminar
            }
            else
            {
                Destroy(gameObject); // destrói imediatamente se não houver som
            }
        }
    }
}
