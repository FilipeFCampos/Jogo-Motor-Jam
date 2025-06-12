using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Campo para o componente AudioSource anexado a este GameObject
    [Header("Componentes de Áudio")]
    [SerializeField] private AudioSource _bgmAudioSource; // AudioSource para a música de fundo
    [SerializeField] private AudioSource _sfxAudioSource; // AudioSource para efeitos sonoros (SFX)

    [Header("Clipes de Áudio")]
    public AudioClip backgroundMusic;    // Música de fundo para o menu
    public AudioClip buttonClickSound;   // Som ao clicar em um botão
    public AudioClip buttonHoverSound;   // Som ao passar o mouse sobre um botão (opcional)
    public AudioClip footplayerSound;   // Som do caminhar do personagem

    // Acessador estático para facilitar o acesso de outros scripts (Singleton)
    public static AudioManager Instance;

    private void Awake()
    {
        // Garante que só haja uma instância do AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o AudioManager entre as cenas
        }
        else if (Instance != this) // Evita bugs caso haja outra instância errada
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Tenta pegar os componentes AudioSource se não foram atribuídos no Inspector
        if (_bgmAudioSource == null)
        {
            _bgmAudioSource = GetComponent<AudioSource>();
        }
        if (_sfxAudioSource == null)
        {
            _sfxAudioSource = GetComponent<AudioSource>();
        }

        PlayBackgroundMusic();

        // Definir volumes iniciais (ajuste conforme necessário)
        SetMusicVolume(0.5f);
        SetSfxVolume(0.5f);
    }

    // Método para tocar a música de fundo
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && _bgmAudioSource != null)
        {
            _bgmAudioSource.clip = backgroundMusic;
            _bgmAudioSource.loop = true; // A música de fundo deve tocar em loop
            _bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: Problema ao iniciar a música de fundo!");
        }
    }

    // Método para tocar um som de clique de botão
    public void PlayButtonClickSound()
    {
        PlaySfx(buttonClickSound);
    }

    // Método para tocar um som de hover de botão com proteção contra spam
    private float _hoverCooldown = 0.1f;
    private float _lastHoverTime;

    public void PlayButtonHoverSound()
    {
        if (Time.time - _lastHoverTime >= _hoverCooldown)
        {
            PlaySfx(buttonHoverSound);
            _lastHoverTime = Time.time;
        }
    }

    // Método genérico para tocar efeitos sonoros
    private void PlaySfx(AudioClip clip)
    {
        if (clip != null && _sfxAudioSource != null)
        {
            _sfxAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager: Problema ao tocar efeito sonoro!");
        }
    }

    // Métodos para controlar o volume
    public void SetMusicVolume(float volume)
    {
        if (_bgmAudioSource != null)
        {
            _bgmAudioSource.volume = Mathf.Clamp01(volume); // Garante que o valor esteja entre 0 e 1
        }
    }

    public void SetSfxVolume(float volume)
    {
        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.volume = Mathf.Clamp01(volume);
        }
    }
}
