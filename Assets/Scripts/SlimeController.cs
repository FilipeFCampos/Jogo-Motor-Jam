using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private float health = 5.0f;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float playerDetectRange = 5f;

    [Header("Wander Settings")]
    [SerializeField] private float wanderInterval = 2f;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 wanderDirection;
    private float wanderTimer;

    Timer timer;
    [SerializeField] private Animator animator;

    [Header("Áudio de Ataque")]
    [SerializeField] private AudioSource attackAudioSource; // AudioSource para o som de ataque
    [SerializeField] private AudioClip attackSound;         // O clipe de áudio do ataque

 
    [Header("Áudio de Movimento")]
    [SerializeField] private AudioSource moveAudioSource; // AudioSource para o som de movimento
    [SerializeField] private AudioClip moveSound;         // O clipe de áudio do movimento



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Slime não encontrou objeto com tag 'Player'.");
        }

        wanderTimer = wanderInterval;
        wanderDirection = Random.insideUnitCircle.normalized;
        timer = FindFirstObjectByType<Timer>();
        if (timer == null)
        {
            Debug.LogError("Timer não encontrado na cena.");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component is missing from the Slime GameObject.");
        }

        // Inicialização do AudioSource de ataque
        if (attackAudioSource == null)
        {
            attackAudioSource = GetComponent<AudioSource>();
            if (attackAudioSource == null)
            {
                Debug.LogError("SlimeController: AudioSource para ataque não encontrado no GameObject do Slime.");
            }
        }

        //INICIALIZAÇÃO DO AUDIO SOURCE DE MOVIMENTO
        if (moveAudioSource == null)
        {
            moveAudioSource = GetComponent<AudioSource>();
            if (moveAudioSource == null)
            {
                Debug.LogError("SlimeController: AudioSource para movimento não encontrado no GameObject do Slime.");
            }
        }
     
    }

    private void Update()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            wanderDirection = Random.insideUnitCircle.normalized;
            wanderTimer = wanderInterval;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 currentVelocity = rb.linearVelocity; // Pega a velocidade atual para verificar movimento

        if (distanceToPlayer <= playerDetectRange)
        {
            // Persegue o player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Movimento aleatório
            rb.MovePosition(rb.position + wanderDirection * moveSpeed * Time.fixedDeltaTime);
        }

        // 
        HandleMoveSound(currentVelocity.magnitude); // Passa a magnitude da velocidade atual

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    // Die
    private void Die()
    {
        SlimeSpawner.slimeCount--;
        animator.SetTrigger("Die");

        //Parar som de movimento ao morrer
        if (moveAudioSource != null && moveAudioSource.isPlaying)
        {
            moveAudioSource.Stop();
        }
   
        StartCoroutine(DestroyAfterAnimation());
    }

    // Wait for Death Animation to finish before destroying the object
    private IEnumerator DestroyAfterAnimation()
    {
        // Disable collider and movement logic while animation plays
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        timer.ResetTimer();

        yield return new WaitForSeconds(animationLength);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            animator.SetTrigger("Attack");

            if (attackAudioSource != null && attackSound != null)
            {
                attackAudioSource.PlayOneShot(attackSound);
                Debug.Log("Som de ataque do Slime tocado!");
            }
            else
            {
                Debug.LogWarning("SlimeController: AudioSource de ataque ou AudioClip não atribuídos para o ataque do Slime!");
            }

            if (playerController != null)
            {
                playerController.TakeDamage(5.02);
            }
        }
    }

    //MÉTODO PARA GERENCIAR O SOM DE MOVIMENTO 
    private void HandleMoveSound(float currentSpeed)
    {
        // Se o slime estiver se movendo (velocidade > um pequeno limiar)
        if (currentSpeed > 0.1f) // Use um limiar pequeno para garantir que ele esteja realmente se movendo
        {
            if (moveAudioSource != null && moveSound != null)
            {
                // Se o som não estiver tocando, comece a tocar em loop
                if (!moveAudioSource.isPlaying)
                {
                    moveAudioSource.clip = moveSound; // Atribui o clipe
                    moveAudioSource.loop = true;      // Define para loop
                    moveAudioSource.Play();           // Começa a tocar
                    // Debug.Log("Som de movimento do Slime iniciado."); // Para depuração
                }
            }
            else
            {
                Debug.LogWarning("SlimeController: AudioSource de movimento ou AudioClip de movimento não atribuídos!");
            }
        }
        // Se o slime estiver parado
        else
        {
            if (moveAudioSource != null && moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop(); // Para o som
                // Debug.Log("Som de movimento do Slime parado."); // Para depuração
            }
        }
    }
    
}