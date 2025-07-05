using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Campo para o componente AudioSource anexado a este GameObject
    [Header("Componentes de �udio")]
    [SerializeField] private AudioSource _bgmAudioSource; // AudioSource para a m�sica de fundo
    [SerializeField] private AudioSource _sfxAudioSource; // AudioSource para efeitos sonoros (SFX)

    [Header("Clipes de �udio")]
    public AudioClip backgroundMusic;    // M�sica de fundo para o menu
    public AudioClip buttonClickSound;   // Som ao clicar em um bot�o
    public AudioClip buttonHoverSound;   // Som ao passar o mouse sobre um bot�o (opcional)
    public AudioClip footplayerSound;   // Som do caminhar do personagem

    // Acessador est�tico para facilitar o acesso de outros scripts (Singleton)
    public static AudioManager Instance;

    private void Awake()
    {
        // Garante que s� haja uma inst�ncia do AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mant�m o AudioManager entre as cenas
        }
        else if (Instance != this) // Evita bugs caso haja outra inst�ncia errada
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Tenta pegar os componentes AudioSource se n�o foram atribu�dos no Inspector
        if (_bgmAudioSource == null)
        {
            _bgmAudioSource = GetComponent<AudioSource>();
        }
        if (_sfxAudioSource == null)
        {
            _sfxAudioSource = GetComponent<AudioSource>();
        }

        PlayBackgroundMusic();

        // Definir volumes iniciais (ajuste conforme necess�rio)
        SetMusicVolume(0.5f);
        SetSfxVolume(0.5f);
    }

    // M�todo para tocar a m�sica de fundo
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && _bgmAudioSource != null)
        {
            _bgmAudioSource.clip = backgroundMusic;
            _bgmAudioSource.loop = true; // A m�sica de fundo deve tocar em loop
            _bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: Problema ao iniciar a m�sica de fundo!");
        }
    }

    // M�todo para tocar um som de clique de bot�o
    public void PlayButtonClickSound()
    {
        PlaySfx(buttonClickSound);
    }

    // M�todo para tocar um som de hover de bot�o com prote��o contra spam
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

    // M�todo gen�rico para tocar efeitos sonoros
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

    // M�todos para controlar o volume
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

    public void PlayMusic()
    {
        if (!_bgmAudioSource.isPlaying)
            _bgmAudioSource.Play();
    }

    public void PauseMusic()
    {
        if (_bgmAudioSource.isPlaying)
            _bgmAudioSource.Pause();
    }
}
