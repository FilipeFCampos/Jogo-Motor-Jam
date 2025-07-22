using UnityEngine;

public class HUDPersistence : MonoBehaviour
{
    public static HUDPersistence Instance { get; private set; } // Propriedade pública para acessar a instância

    public GameObject hudGameObject; // Arraste aqui o objeto que controla o HUD (ex: o Canvas do HUD)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Faz o HUD persistir
        }
        else
        {
            Destroy(gameObject); // Evita duplicatas
        }
    }

    public void SetActiveHUD(bool active)
    {
        if (hudGameObject != null)
        {
            hudGameObject.SetActive(active);
        }
        else
        {
            Debug.LogWarning("[HUDPersistence] hudGameObject não está atribuído!");
        }
    }
}
