using UnityEngine;
using System.Collections; // Necessário para IEnumerator se você usar corrotinas

public class AudioManager : MonoBehaviour
{
    // === Componentes de Áudio ===
    [Header("Componentes de Áudio")]
    [SerializeField] private AudioSource _bgmAudioSource; // AudioSource para a música de fundo (Background Music)
    [SerializeField] private AudioSource _sfxAudioSource; // AudioSource para efeitos sonoros (Sound Effects)

    // === Clipes de Áudio ===
    [Header("Clipes de Áudio")]
    public AudioClip defaultBackgroundMusic;    // Música padrão (ex: do menu principal)
    public AudioClip buttonClickSound;          // Som ao clicar em um botão
    public AudioClip buttonHoverSound;          // Som ao passar o mouse sobre um botão (opcional)
    public AudioClip level1Music;               // Música específica para a Fase 1
    public AudioClip level2Music;               // Música específica para a Fase 2
    public AudioClip level3Music;               // Música específica para a Fase 3
    // public AudioClip footplayerSound; // Este clipe não está sendo usado diretamente neste script, mas pode ser adicionado em PlaySfx()

    // === Acessador Estático (Singleton Pattern) ===
    // Permite que outros scripts acessem o AudioManager facilmente: AudioManager.Instance.PlayButtonClickSound();
    public static AudioManager Instance;

    // === Variáveis de Controle de Cooldown para SFX ===
    private float _hoverCooldown = 0.1f; // Tempo mínimo entre sons de hover para evitar spam
    private float _lastHoverTime;        // Último momento em que um som de hover foi tocado

    /// <summary>
    /// Chamado quando o script é carregado ou quando uma instância é criada.
    /// Usado para implementar o padrão Singleton e inicializar AudioSources.
    /// </summary>
    private void Awake()
    {
        // Implementação do padrão Singleton: garante que só haja uma instância do AudioManager.
        if (Instance == null)
        {
            Instance = this;
            // Mantém o AudioManager ativo entre as cenas. Essencial para músicas de fundo contínuas.
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Se já existe uma instância, destrói esta nova para evitar duplicatas.
            Destroy(gameObject);
            return; // Sai do Awake para evitar processamento adicional.
        }

        // --- ATENÇÃO: É ALTAMENTE RECOMENDADO ATRIBUIR _bgmAudioSource e _sfxAudioSource
        // --- DIRETAMENTE NO INSPECTOR DO GAMEOBJECT "AudioManager" NA CENA.
        // --- Se você não os atribuir no Inspector, o script tentará pegá-los,
        // --- mas pode haver conflitos se o GameObject tiver apenas um AudioSource.

        // Tentativa de pegar o AudioSource para BGM se não atribuído no Inspector
        if (_bgmAudioSource == null)
        {
            Debug.LogWarning("AudioManager: _bgmAudioSource não atribuído no Inspector. Tentando pegar o primeiro AudioSource do GameObject.");
            _bgmAudioSource = GetComponent<AudioSource>();
            if (_bgmAudioSource == null)
            {
                Debug.LogError("AudioManager: _bgmAudioSource não encontrado no GameObject! Por favor, adicione um componente AudioSource ao GameObject e/ou atribua-o no Inspector.");
            }
        }
        // Configurações padrão para o AudioSource de BGM
        if (_bgmAudioSource != null)
        {
            _bgmAudioSource.playOnAwake = false; // A música é iniciada via script
            _bgmAudioSource.loop = true;         // Músicas de fundo geralmente fazem loop
            _bgmAudioSource.spatialBlend = 0f;   // Música de fundo é geralmente 2D (não atenua por distância)
        }

        // Tentativa de pegar o AudioSource para SFX se não atribuído no Inspector
        if (_sfxAudioSource == null)
        {
            Debug.LogWarning("AudioManager: _sfxAudioSource não atribuído no Inspector. Tentando pegar outro AudioSource no GameObject (se existir).");
            AudioSource[] allAudioSources = GetComponents<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                if (source != _bgmAudioSource) // Encontra um AudioSource que não seja o de BGM
                {
                    _sfxAudioSource = source;
                    break; // Sai do loop após encontrar
                }
            }
            if (_sfxAudioSource == null)
            {
                Debug.LogError("AudioManager: _sfxAudioSource não encontrado no GameObject! Por favor, adicione um segundo componente AudioSource ao GameObject e/ou atribua-o no Inspector.");
            }
            else if (_sfxAudioSource == _bgmAudioSource && allAudioSources.Length == 1)
            {
                Debug.LogWarning("AudioManager: O mesmo AudioSource está sendo usado para BGM e SFX. Isso pode causar conflitos de reprodução. Considere adicionar outro AudioSource para SFX.");
            }
        }
        // Configurações padrão para o AudioSource de SFX
        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.playOnAwake = false; // SFX são tocados sob demanda
            _sfxAudioSource.loop = false;        // SFX geralmente não fazem loop (PlayOneShot)
            _sfxAudioSource.spatialBlend = 0f;   // SFX de UI ou gerais são geralmente 2D
        }
    }

    /// <summary>
    /// Chamado no primeiro frame em que o script está ativo.
    /// Usado para iniciar a música de fundo padrão e definir volumes iniciais.
    /// </summary>
    void Start()
    {
        // Ao iniciar o jogo/cena, toca a música de fundo padrão (ex: do menu)
        // Certifique-se de ter atribuído 'defaultBackgroundMusic' no Inspector.
        PlayDefaultBackgroundMusic();

        // Define os volumes iniciais para Música e SFX.
        // Estes valores serão os "volumes base" para todo o jogo, a menos que sejam alterados.
        // Experimente valores entre 0.0f (mudo) e 1.0f (máximo).
        SetMusicVolume(0.45f); // Volume inicial da música (50%)
        SetSfxVolume(0.05f);   // Volume inicial dos efeitos sonoros (70%)
    }

    /// <summary>
    /// Toca a música de fundo especificada.
    /// GARANTE QUE A MÚSICA ANTERIOR SEJA PARADA ANTES DE INICIAR UMA NOVA.
    /// </summary>
    /// <param name="musicClip">O clipe de áudio da música a ser tocada.</param>
    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("AudioManager: Tentativa de tocar música de fundo com AudioClip nulo. Verifique a atribuição.");
            return;
        }
        if (_bgmAudioSource == null)
        {
            Debug.LogError("AudioManager: _bgmAudioSource não está atribuído ou foi destruído. Não é possível tocar música.");
            return;
        }

        // Se a música que queremos tocar já é a que está tocando E o AudioSource não está pausado,
        // então não faz nada para evitar reiniciar a mesma música desnecessariamente.
        if (_bgmAudioSource.clip == musicClip && _bgmAudioSource.isPlaying)
        {
            return;
        }

        // Se já estiver tocando uma música (seja a mesma ou diferente), pare-a.
        if (_bgmAudioSource.isPlaying)
        {
            _bgmAudioSource.Stop(); // <<< ESSA É A LINHA CRÍTICA PARA PARAR A MÚSICA ANTERIOR
            Debug.Log($"AudioManager: Parando música anterior: {_bgmAudioSource.clip?.name ?? "N/A"}");
        }

        // Atribui e toca a nova música.
        _bgmAudioSource.clip = musicClip;
        _bgmAudioSource.loop = true; // A música de fundo deve tocar em loop
        _bgmAudioSource.Play();
        Debug.Log($"AudioManager: Iniciando nova música de fundo: {musicClip.name}");
    }

    /// <summary>
    /// Toca a música de fundo padrão (definida como 'defaultBackgroundMusic').
    /// Útil para retornar ao menu, por exemplo.
    /// </summary>
    public void PlayDefaultBackgroundMusic()
    {
        PlayBackgroundMusic(defaultBackgroundMusic);
    }

    /// <summary>
    /// Toca o som de clique de botão.
    /// </summary>
    public void PlayButtonClickSound()
    {
        PlaySfx(buttonClickSound);
    }

    /// <summary>
    /// Toca o som de hover de botão com um cooldown para evitar spam.
    /// </summary>
    public void PlayButtonHoverSound()
    {
        if (Time.time - _lastHoverTime >= _hoverCooldown)
        {
            PlaySfx(buttonHoverSound);
            _lastHoverTime = Time.time;
        }
    }

    /// <summary>
    /// Método genérico e privado para tocar efeitos sonoros.
    /// Usado internamente pelo AudioManager.
    /// </summary>
    /// <param name="clip">O clipe de áudio do SFX a ser tocado.</param>
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Tentativa de tocar efeito sonoro com AudioClip nulo. Verifique a atribuição.");
            return;
        }
        if (_sfxAudioSource == null)
        {
            Debug.LogError("AudioManager: _sfxAudioSource não está atribuído ou foi destruído. Não é possível tocar SFX.");
            return;
        }

        _sfxAudioSource.PlayOneShot(clip); // PlayOneShot é ideal para SFX (não interfere na reprodução de outros sons no mesmo AudioSource)
    }

    /// <summary>
    /// Controla o volume da música de fundo (BGM).
    /// </summary>
    /// <param name="volume">O volume desejado, de 0.0f a 1.0f.</param>
    public void SetMusicVolume(float volume)
    {
        if (_bgmAudioSource != null)
        {
            _bgmAudioSource.volume = Mathf.Clamp01(volume); // Garante que o valor esteja entre 0 e 1
            Debug.Log($"Volume da Música ajustado para: {_bgmAudioSource.volume:F2}"); // Para depuração
        }
    }

    /// <summary>
    /// Controla o volume dos efeitos sonoros (SFX).
    /// </summary>
    /// <param name="volume">O volume desejado, de 0.0f a 1.0f.</param>
    public void SetSfxVolume(float volume)
    {
        if (_sfxAudioSource != null)
        {
            _sfxAudioSource.volume = Mathf.Clamp01(volume);
            Debug.Log($"Volume do SFX ajustado para: {_sfxAudioSource.volume:F2}"); // Para depuração
        }
    }

    /// <summary>
    /// Retoma a reprodução da música de fundo se estiver pausada.
    /// </summary>
    public void PlayMusic()
    {
        if (_bgmAudioSource != null && !_bgmAudioSource.isPlaying)
            _bgmAudioSource.Play();
    }

    /// <summary>
    /// Pausa a reprodução da música de fundo.
    /// </summary>
    public void PauseMusic()
    {
        if (_bgmAudioSource != null && _bgmAudioSource.isPlaying)
            _bgmAudioSource.Pause();
    }

    /// <summary>
    /// Para a reprodução da música de fundo completamente.
    /// </summary>
    public void StopMusic()
    {
        if (_bgmAudioSource != null && _bgmAudioSource.isPlaying)
            _bgmAudioSource.Stop();
    }
}