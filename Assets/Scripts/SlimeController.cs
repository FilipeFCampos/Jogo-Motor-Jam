using System.Collections;
using UnityEngine;

// Garante que o GameObject tenha um Rigidbody2D, pois o script depende dele.
[RequireComponent(typeof(Rigidbody2D))]
public class SlimeController : MonoBehaviour
{
    // === Configurações de Status do Slime ===
    [Header("Status")]
    [SerializeField] private float health = 5.0f; // Vida atual do slime
    [SerializeField] private float moveSpeed = 1.5f; // Velocidade de movimento do slime
    [SerializeField] private float playerDetectRange = 5f; // Raio para detecção do jogador

    // === Configurações de Movimento Aleatório (Wander) ===
    [Header("Wander Settings")]
    [SerializeField] private float wanderInterval = 2f; // Tempo entre mudanças de direção no modo wander

    // === Referências a Componentes ===
    private Rigidbody2D rb; // Componente Rigidbody2D do slime
    private Transform player; // Transform do jogador
    private Vector2 wanderDirection; // Direção atual do movimento wander
    private float wanderTimer; // Temporizador para o movimento wander

    // === Controle de Áudio Baseado em Posição ===
    // previousPosition: Armazena a posição do Slime no frame anterior para calcular a distância percorrida.
    private Vector2 previousPosition;
    // minMovementThreshold: O quão pouco o slime precisa se mover para ser considerado "em movimento".
    // Isso evita que o som toque quando o Slime está parado ou tem um movimento mínimo.
    [SerializeField] private float minMovementThreshold = 0.005f;
    [SerializeField] private SlimeType slimeType; // Tipo de slime, usado para identificar diferentes tipos de slimes
    private enum SlimeType { BLACK, BLUE, GREEN, RED }; // Tipos de slimes disponíveis
    private ParticleSystem slimeParticles; // Partículas associadas ao slime, se houver
    private bool isSpecialSLime = false; // Indica se o slime é um slime especial (ex: Slime de Poder)

    // === Outras Referências ===
    Timer timer; // Referência ao componente Timer (assumindo que existe um na cena)
    [SerializeField] private Animator animator; // Componente Animator do slime

    // === Configurações de Áudio de Ataque ===
    [Header("Áudio de Ataque")]
    [SerializeField] private AudioSource attackAudioSource; // AudioSource para o som de ataque
    [SerializeField] private AudioClip attackSound;          // O clipe de áudio do ataque

    // === Configurações de Áudio de Movimento ===
    [Header("Áudio de Movimento")]
    [SerializeField] private AudioSource moveAudioSource; // AudioSource para o som de movimento
    [SerializeField] private AudioClip moveSound;          // O clipe de áudio do movimento

    private PlayerController playerController;
    /// <summary>
    /// Chamado quando o script é carregado ou quando uma instância é criada.
    /// Usado para inicializar referências e configurações.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("SlimeController: Rigidbody2D component is missing from the Slime GameObject.");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            playerController = PlayerController.Instance;
            if (playerController != null)
            {
                player = playerController.transform;
            }
            else
            {
                Debug.LogWarning("Slime não encontrou objeto com tag 'Player'. Certifique-se de que o jogador tenha a tag 'Player'.");
            }
        }

        wanderTimer = wanderInterval;
        wanderDirection = Random.insideUnitCircle.normalized;

        timer = FindFirstObjectByType<Timer>(); // Melhor usar FindObjectOfType<Timer>() ou injetar via Inspector
        if (timer == null)
        {
            Debug.LogError("SlimeController: Timer não encontrado na cena. Adicione um Timer ou remova a referência.");
        }
        if (animator == null)
        {
            Debug.LogError("SlimeController: Animator component is missing from the Slime GameObject.");
        }

        // === Calcula a lógica de habilidades ===
        slimeParticles = GetComponentInChildren<ParticleSystem>();
        if (slimeParticles == null)
        {
            Debug.LogError("SlimeController: ParticleSystem não encontrado no Slime.");
        }
        else
        {
            if (Random.Range(0, 100) < 25) // 25% de chance de ser um slime especial
            {
                isSpecialSLime = true;
                slimeParticles.Play(); // Inicia as partículas se existirem
            }
        }

        // === Inicialização e Configuração dos AudioSources ===
        // É ALTAMENTE recomendado que você arraste os AudioSources para esses campos
        // no Inspector do Prefab do Slime para evitar problemas de referência.

        // Inicialização do AudioSource de ataque
        if (attackAudioSource == null)
        {
            Debug.LogWarning("SlimeController: Attack AudioSource não atribuído no Inspector. Tentando pegar o primeiro AudioSource disponível.");
            attackAudioSource = GetComponent<AudioSource>(); // Tenta pegar qualquer AudioSource no objeto
            if (attackAudioSource == null)
            {
                Debug.LogError("SlimeController: AudioSource para ataque não encontrado no GameObject do Slime. Por favor, adicione um AudioSource ou atribua-o no Inspector.");
            }
        }
        // Define o volume do áudio de ataque APÓS garantir que attackAudioSource não seja nulo.
        if (attackAudioSource != null)
        {
            attackAudioSource.volume = 0.10f; // Volume do ataque (ajuste este valor conforme sua preferência)
            // Certifique-se de que o AudioSource de ataque NÃO esteja em loop ou "Play On Awake" no Inspector.
            attackAudioSource.loop = false;
            attackAudioSource.playOnAwake = false;
        }


        // Inicialização do AudioSource de movimento
        if (moveAudioSource == null)
        {
            Debug.LogWarning("SlimeController: Move AudioSource não atribuído no Inspector. Tentando pegar outro AudioSource.");
            // Tenta encontrar um AudioSource que não seja o de ataque, se houver mais de um.
            // Esta lógica é uma tentativa, a atribuição manual no Inspector é sempre a mais segura.
            AudioSource[] allAudioSources = GetComponents<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                if (source != attackAudioSource) // Pega um AudioSource que não seja o de ataque
                {
                    moveAudioSource = source;
                    break; // Sai do loop após encontrar um
                }
            }
            if (moveAudioSource == null)
            {
                Debug.LogError("SlimeController: AudioSource para movimento não encontrado no GameObject do Slime. Por favor, adicione outro AudioSource ou atribua-o no Inspector.");
            }
            else if (moveAudioSource == attackAudioSource)
            {
                Debug.LogWarning("SlimeController: O mesmo AudioSource está sendo usado para ataque e movimento. Isso pode causar problemas. Considere adicionar outro AudioSource para um dos sons.");
            }
        }
        // Define o volume do áudio de movimento APÓS garantir que moveAudioSource não seja nulo.
        if (moveAudioSource != null)
        {
            moveAudioSource.volume = 0.05f; // Volume do movimento (geralmente mais baixo, ajuste este valor)
            // Certifique-se de que o AudioSource de movimento NÃO esteja em loop ou "Play On Awake" no Inspector.
            // O loop será controlado pelo script (HandleMoveSound).
            moveAudioSource.loop = false;
            moveAudioSource.playOnAwake = false;
        }

        // === Inicializa a posição anterior para o cálculo de movimento ===
        // Essencial para que o cálculo da distância no primeiro FixedUpdate seja preciso.
        previousPosition = rb.position;
    }

    void Start()
    {
        playerController = PlayerController.Instance;
        if (playerController != null)
        {
            player = playerController.transform;
        }
    }

    /// <summary>
    /// Chamado uma vez por frame. Usado para lógica de jogo que não depende da física.
    /// </summary>
    private void Update()
    {
        // Lógica de temporizador para o movimento aleatório (wander)
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            wanderDirection = Random.insideUnitCircle.normalized; // Nova direção aleatória
            wanderTimer = wanderInterval; // Reseta o temporizador
        }
    }

    /// <summary>
    /// Chamado em intervalos fixos para cálculos de física.
    /// </summary>
    private void FixedUpdate()
    {
        if (player == null) return; // Não faz nada se o jogador não for encontrado

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Lógica de perseguição ou movimento aleatório
        if (distanceToPlayer <= playerDetectRange)
        {
            // Persegue o jogador
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Movimento aleatório (wander)
            rb.MovePosition(rb.position + wanderDirection * moveSpeed * Time.fixedDeltaTime);
        }

        // === Calcula a distância percorrida e gerencia o som de movimento ===
        // Calcula a distância que o Slime percorreu desde o último FixedUpdate.
        float distanceMoved = Vector2.Distance(rb.position, previousPosition);
        // Chama a função que gerencia o som de movimento, passando a distância.
        HandleMoveSound(distanceMoved);
        // Atualiza a posição anterior para o próximo FixedUpdate.
        previousPosition = rb.position;
    }

    /// <summary>
    /// Aplica dano ao slime.
    /// </summary>
    /// <param name="damage">Quantidade de dano a ser aplicada.</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die(); // Slime morre se a vida for <= 0
        }
    }

    /// <summary>
    /// Lógica de morte do slime.
    /// </summary>
    private void Die()
    {
        SlimeSpawner.slimeCount--; // Decrementa a contagem de slimes no spawner
        animator.SetTrigger("Die"); // Ativa a animação de morte

        // Para o som de movimento ao morrer
        if (moveAudioSource != null && moveAudioSource.isPlaying)
        {
            moveAudioSource.Stop();
            //Debug.Log($"Som de movimento do Slime '{gameObject.name}' parado ao morrer.");
        }

        if (isSpecialSLime)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                playerObj.GetComponent<PlayerController>().AbsorveAbility((int)slimeType); // Adiciona o poder do slime especial ao jogador
            }
            
        }

        StartCoroutine(DestroyAfterAnimation()); // Inicia corrotina para destruir após animação
    }

    /// <summary>
    /// Espera a animação de morte terminar antes de destruir o objeto.
    /// </summary>
    private IEnumerator DestroyAfterAnimation()
    {
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false; // Desabilita o collider para evitar novas interações

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length; // Pega a duração da animação atual
        timer.ResetTimer(); // Assumindo que o timer deve ser resetado aqui

        yield return new WaitForSeconds(animationLength); // Espera a duração da animação
        Destroy(gameObject); // Destroi o GameObject do slime
    }

    /// <summary>
    /// Detecta colisão com o jogador para causar dano e tocar som de ataque.
    /// </summary>
    /// <param name="collision">Informações da colisão.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            animator.SetTrigger("Attack"); // Ativa a animação de ataque

            // Toca o som de ataque
            if (attackAudioSource != null && attackSound != null)
            {
                attackAudioSource.PlayOneShot(attackSound);
                Debug.Log($"Som de ataque do Slime '{gameObject.name}' tocado!");
            }
            else
            {
                Debug.LogWarning("SlimeController: AudioSource de ataque ou AudioClip não atribuídos para o ataque do Slime!");
            }

            // Causa dano ao jogador
            if (playerController != null)
            {
                playerController.TakeDamage(5.02);
            }
        }
    }

    /// <summary>
    /// Gerencia a reprodução do som de movimento do Slime baseado na distância percorrida.
    /// </summary>
    /// <param name="distanceMoved">Distância que o Slime moveu no último FixedUpdate.</param>
    private void HandleMoveSound(float distanceMoved)
    {
        // DEBUG: Mostra a distância percorrida pelo Slime no último FixedUpdate.
        // Isso é crucial para depurar e ajustar 'minMovementThreshold'.
        // Descomente a linha abaixo se precisar depurar a distância
        // Debug.Log($"Slime '{gameObject.name}' Distância Movida: {distanceMoved:F6}"); 

        // Se o slime estiver se movendo (distância percorrida > um pequeno limiar)
        if (distanceMoved > minMovementThreshold) // Usa o parâmetro minMovementThreshold para verificar o movimento.
        {
            if (moveAudioSource != null && moveSound != null)
            {
                // Se o som de movimento não estiver tocando, inicie-o.
                // Isso evita que o som seja reiniciado a cada frame, causando ruído.
                if (!moveAudioSource.isPlaying)
                {
                    moveAudioSource.clip = moveSound; // Atribui o clipe de áudio de movimento.
                    moveAudioSource.loop = true;      // Define para que o som repita em loop.
                    moveAudioSource.Play();           // Inicia a reprodução do som.
                    Debug.Log($"Som de movimento do Slime '{gameObject.name}' iniciado. (Distância: {distanceMoved:F6})");
                }
            }
            else
            {
                Debug.LogWarning("SlimeController: AudioSource de movimento ou AudioClip de movimento não atribuídos para '" + gameObject.name + "'!");
            }
        }
        // Se o slime estiver parado (distância percorrida <= limiar)
        else
        {
            // Se o som de movimento estiver tocando, pare-o.
            if (moveAudioSource != null && moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop();
                //Debug.Log($"Som de movimento do Slime '{gameObject.name}' parado. (Distância: {distanceMoved:F6})");
            }
        }
    }
}