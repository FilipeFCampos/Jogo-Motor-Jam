using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public enum Estado { Andando, Atacando, TomandoDano, Morto }

    private Estado estadoAtual = Estado.Andando;

    [SerializeField] private float vidaMaxima = 10f;
    private float vidaAtual;

    public float VidaAtual => vidaAtual;
    public float VidaMaxima => vidaMaxima;

    [SerializeField] private float velocidade = 125.5f;
    [SerializeField] private float distanciaAtaque = 1.5f;
    [SerializeField] private float tempoEntreAtaques = 2f;
    private float tempoUltimoAtaque;

    [Header("Áudio de Passos do Boss")]
    [SerializeField] private AudioSource footstepAudioBoss; // AudioSource para os passos do boss
    [SerializeField] private AudioClip[] footstepSoundsBoss; // Array de sons variados de passos do boss
    [SerializeField] private float footstepCooldownBoss = 0.4f; // Tempo entre cada som de passo do boss
    private float lastFootstepTimeBoss;

    // --- NOVAS VARIÁVEIS PARA O ÁUDIO DE ATAQUE DO BOSS ---
    [Header("Áudio de Ataque do Boss")]
    [SerializeField] private AudioSource attackAudioSourceBoss; // AudioSource para o som de ataque do boss
    [SerializeField] private AudioClip attackSoundBoss;          // O clipe de áudio do ataque do boss
    // --- FIM DAS NOVAS VARIÁVEIS ---

    // --- NOVO: Variáveis para controle de áudio de movimento baseado em posição ---
    private Vector2 previousPositionBoss;
    [SerializeField] private float minBossMovementThreshold = 0.005f; // Limiar de movimento do boss
    // -----------------------------------------------------------------------------

    private Transform jogador;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 direcao; // Esta variável não está sendo usada, pode ser removida se não for necessária
    private int direcaoAtual;

    private bool podeAtacar = true;
    private bool podeMover = true;
    [SerializeField] private GameObject deathEffect;

    [SerializeField] private float danoAtaque = 1f; // Dano que o boss causa ao jogador

    private void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        vidaAtual = vidaMaxima;
        tempoUltimoAtaque = -tempoEntreAtaques;

        // Inicialização do AudioSource de passos
        if (footstepAudioBoss == null)
        {
            footstepAudioBoss = GetComponent<AudioSource>();
            if (footstepAudioBoss == null)
            {
                Debug.LogError("BossController: AudioSource para passos não encontrado no GameObject do boss!");
            }
        }
        lastFootstepTimeBoss = Time.time;

        // --- INICIALIZAÇÃO DO AUDIO SOURCE DE ATAQUE DO BOSS ---
        if (attackAudioSourceBoss == null)
        {
            // Tenta pegar o AudioSource no próprio GameObject do boss
            attackAudioSourceBoss = GetComponent<AudioSource>();
            if (attackAudioSourceBoss == null)
            {
                Debug.LogError("BossController: AudioSource para ataque não encontrado no GameObject do boss.");
            }
        }
        // --- FIM DA INICIALIZAÇÃO ---

        // --- NOVO: Inicializa a posição anterior do boss ---
        previousPositionBoss = rb.position;
        // --------------------------------------------------
    }

    private void HandleBossFootstepSound(float distanceMoved) // Agora recebe a distância movida
    {
        // DEBUG: Mostra a distância percorrida pelo Boss no último FixedUpdate
        Debug.Log($"Boss '{gameObject.name}' Distância Movida: {distanceMoved:F6}");

        // Apenas toque o som se o boss estiver no estado de Andando e se movendo de fato
        if (estadoAtual == Estado.Andando && distanceMoved > minBossMovementThreshold) // Usando a distância movida
        {
            if (Time.time - lastFootstepTimeBoss >= footstepCooldownBoss)
            {
                if (footstepAudioBoss != null && footstepSoundsBoss.Length > 0)
                {
                    AudioClip stepSound = footstepSoundsBoss[Random.Range(0, footstepSoundsBoss.Length)];
                    footstepAudioBoss.PlayOneShot(stepSound);
                    lastFootstepTimeBoss = Time.time;
                    Debug.Log($"Som de passos do boss '{gameObject.name}' tocado! (Distância: {distanceMoved:F6})");
                }
                else
                {
                    Debug.LogWarning("BossController: Footstep AudioSource ou Clipes de passos do boss não atribuídos!");
                }
            }
        }
        else
        {
            // Se o boss não estiver andando ou não se movendo, pare o som de passos se estiver tocando
            if (footstepAudioBoss != null && footstepAudioBoss.isPlaying)
            {
                footstepAudioBoss.Stop();
                Debug.Log($"Boss '{gameObject.name}' parado, som de passos interrompido.");
            }
        }
    }

    private void Update()
    {
        if (estadoAtual == Estado.Morto) return;

        AtualizarDirecao();
        AtualizarFlagsAnimator();

        if (estadoAtual == Estado.Andando)
        {
            float distancia = Vector2.Distance(transform.position, jogador.position);
            if (distancia <= distanciaAtaque && Time.time - tempoUltimoAtaque >= tempoEntreAtaques)
            {
                MudarEstado(Estado.Atacando);
                StartCoroutine(Atacar());
            }
        }

        AtualizarAnimacoes();
    }

    private void FixedUpdate()
    {
        if (estadoAtual == Estado.Andando && podeMover && EstaNaCamera())
        {
            Mover();
            // --- NOVO: Calcula e passa a distância movida para o handler de som ---
            float distanceMoved = Vector2.Distance(rb.position, previousPositionBoss);
            HandleBossFootstepSound(distanceMoved);
            previousPositionBoss = rb.position; // Atualiza a posição anterior
            // ------------------------------------------------------------------
        }
        else
        {
            // Se o boss não estiver andando ou não puder mover, certifique-se de parar o som de passos
            // Passamos 0 para indicar que não há movimento
            HandleBossFootstepSound(0f);
            previousPositionBoss = rb.position; // Ainda atualiza para evitar salto no próximo movimento
        }
    }

    private void Mover()
    {
        Vector2 direcaoMovimento = (jogador.position - transform.position).normalized;
        rb.MovePosition(rb.position + direcaoMovimento * velocidade * Time.fixedDeltaTime);
    }

    private void AtualizarDirecao()
    {
        Vector2 diff = jogador.position - transform.position;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            direcaoAtual = (diff.x > 0) ? 3 : 2; // Right : Left
        else
            direcaoAtual = (diff.y > 0) ? 0 : 1; // Up : Down
    }

    private void AtualizarAnimacoes()
    {
        anim.SetInteger("Direction", direcaoAtual);
        anim.SetBool("IsMoving", estadoAtual == Estado.Andando);
        sr.flipX = (direcaoAtual == 2); // Left = 2
    }

    private IEnumerator Atacar()
    {
        podeAtacar = false;
        podeMover = false;

        // Garante que o som de passos pare durante o ataque
        if (footstepAudioBoss != null && footstepAudioBoss.isPlaying)
        {
            footstepAudioBoss.Stop();
        }

        AtualizarDirecao();
        AtualizarAnimacoes();

        anim.SetTrigger("Attack");
        if (attackAudioSourceBoss != null && attackSoundBoss != null)
        {
            attackAudioSourceBoss.PlayOneShot(attackSoundBoss);
            Debug.Log($"Som de ataque do Boss '{gameObject.name}' tocado!");
        }
        else
        {
            Debug.LogWarning("BossController: AudioSource de ataque ou AudioClip não atribuídos para o ataque do Boss!");
        }
        tempoUltimoAtaque = Time.time;

        yield return new WaitForSeconds(0.5f); // Tempo até o golpe atingir

        TentarCausarDano();

        yield return new WaitForSeconds(0.5f); // Tempo restante da animação

        if (estadoAtual != Estado.Morto)
        {
            MudarEstado(Estado.Andando);
        }

        podeAtacar = true;
        podeMover = true;
    }

    private void TentarCausarDano()
    {
        if (jogador == null) return;

        float distancia = Vector2.Distance(transform.position, jogador.position);
        if (distancia <= distanciaAtaque)
        {
            PlayerController playerController = jogador.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(danoAtaque);
                Debug.Log("Boss causou " + danoAtaque + " de dano ao jogador.");
            }
        }
    }

    public void TomarDano(float dano)
    {
        if (estadoAtual == Estado.Morto) return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(TomarDanoCoroutine());
        }
    }

    private IEnumerator TomarDanoCoroutine()
    {
        MudarEstado(Estado.TomandoDano);
        anim.SetTrigger("Hit");

        podeMover = false;

        // Garante que o som de passos pare ao tomar dano
        if (footstepAudioBoss != null && footstepAudioBoss.isPlaying)
        {
            footstepAudioBoss.Stop();
        }

        yield return new WaitForSeconds(0.5f);

        if (estadoAtual != Estado.Morto)
        {
            MudarEstado(Estado.Andando);
            podeMover = true;
        }
    }

    private void Morrer()
    {
        Debug.Log("Boss derrotado!");
        MudarEstado(Estado.Morto);
        anim.SetTrigger("Die");
        podeMover = false;
        rb.linearVelocity = Vector2.zero; // Corrigido para linearVelocity

        // Garante que o som de passos pare ao morrer
        if (footstepAudioBoss != null && footstepAudioBoss.isPlaying)
        {
            footstepAudioBoss.Stop();
        }

        // Adiciona pontos ao ScoreManager (de forma segura)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(100); // Adiciona pontos da vitória

            // Armazena dados extras no PlayerPrefs
            PlayerPrefs.SetInt("SlimesDefeated", PlayerPrefs.GetInt("SlimesDefeated", 0) + 1);
            PlayerPrefs.SetFloat("PlayTime", Time.timeSinceLevelLoad);

            PlayerPrefs.Save(); // Salva tudo junto
        }
        else
        {
            Debug.LogWarning("ScoreManager não encontrado!");
        }

        StartCoroutine(MorrerEComecarCena());
    }

    private IEnumerator MorrerEComecarCena()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("WinScene");
    }

    private void AtualizarFlagsAnimator()
    {
        // Esta função parece estar pegando o valor dos parâmetros do Animator.
        // Se 'CanMove' e 'CanAttack' são definidos no Animator, então está ok.
        // Caso contrário, você pode remover essas linhas e gerenciar 'podeMover'/'podeAtacar' manualmente.
        // Ex: podeMover = (estadoAtual == Estado.Andando);
        podeMover = anim.GetBool("CanMove");
        podeAtacar = anim.GetBool("CanAttack");
    }

    private void MudarEstado(Estado novoEstado)
    {
        estadoAtual = novoEstado;
    }

    private bool EstaNaCamera()
    {
        if (Camera.main == null) return false;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        float margem = 0.2f;

        return viewportPos.x >= margem && viewportPos.x <= 1f - margem &&
               viewportPos.y >= margem && viewportPos.y <= 1f - margem &&
               viewportPos.z > 0;
    }
}