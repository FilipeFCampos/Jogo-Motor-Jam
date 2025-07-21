using System.Collections;
using Unity.VisualScripting; // Esta linha pode não ser necessária se você não usar Visual Scripting
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    /* Enums */
    private enum Directions { UP, DOWN, LEFT, RIGHT };

    /* Basic player movement variables */
    private Vector2 moveDir;
    [SerializeField] private float moveSpeed = 200f;
    private Directions currentDirection;
    private bool canMove = true; // Flag to control player movement

    /* References to components */
    [SerializeField] private Rigidbody2D rb;
    // Animation related references
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource swordAudio;   // AudioSource para o som da espada
    [SerializeField] private AudioSource pickupKey;    // AudioSource para o som de coletar chave
    [SerializeField] private AudioSource footstepAudio; // AudioSource para os passos
    [SerializeField] private AudioClip[] footstepSounds; // Array de sons variados de passos

    private float footstepCooldown = 0.3f; // Tempo entre cada som de passo
    private float lastFootstepTime;


    /* Combat related variables */
    public double health;
    private bool isInvulnerable = false;
    private bool isDead = false; // Flag to check if the player is dead
    private bool canAttack = true; // Flag to control attack state
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float meleeDamage = 1f;
    [SerializeField] private float meleeRange = 1f;
    private bool canTakeDamage = true; // Flag to control if the player can take damage

    [SerializeField] private LayerMask enemyLayer;
    public Timer timer;

    public bool hasKey = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the PlayerController GameObject.");
        }
        health = maxHealth;
        moveDir = Vector2.zero;
        currentDirection = Directions.DOWN; // Default direction
        canMove = animator.GetBool("CanMove");
        if (animator == null)
        {
            Debug.LogError("Animator component is missing from the PlayerController GameObject.");
        }
        canAttack = animator.GetBool("CanAttack");
        timer = FindFirstObjectByType<Timer>();
        if (timer == null)
        {
            Debug.LogError("Timer component not found in the scene.");
        }

        // === CONTROLE DE VOLUME DOS ÁUDIOS VIA SCRIPT AQUI ===

        // Volume do áudio da espada
        if (swordAudio != null)
        {
            swordAudio.volume = 0.35f; 
            swordAudio.loop = false; // Garante que não está em loop
            swordAudio.playOnAwake = false; // Garante que não toca ao iniciar
        }
        else
        {
            Debug.LogWarning("PlayerController: AudioSource para espada não atribuído!");
        }

        // Volume do áudio de coletar chave
        if (pickupKey != null)
        {
            pickupKey.volume = 0.45f; 
            pickupKey.loop = false;
            pickupKey.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("PlayerController: AudioSource para coletar chave não atribuído!");
        }

        // Volume do áudio de passos (já existente, movido para Awake para centralizar controle)
        if (footstepAudio != null)
        {
            footstepAudio.volume = 0.15f; // 
            footstepAudio.loop = false; // Garante que não está em loop (controlado pelo HandleFootstepSound)
            footstepAudio.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("PlayerController: AudioSource para passos não atribuído!");
        }

        // ======================================================
    }

    private void Attack()
    {
        if (!canAttack)
        {
            Debug.Log("Cannot attack right now.");
            return;
        }

        // AQUI o volume já foi definido em Awake, então não precisa mudar aqui
        if (swordAudio != null && !swordAudio.isPlaying)
        {
            swordAudio.Play();
        }

        Debug.Log("Player attacks with melee damage: " + meleeDamage);
        animator.SetTrigger("Attack");
        canMove = false;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRange, enemyLayer);

        foreach (Collider2D hit in hitEnemies)
        {
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TomarDano(meleeDamage);
                Debug.Log("Boss atingido: " + hit.name);
                continue;
            }

<<<<<<< Updated upstream
            // 2) Se for Slime
=======
            orcController greenOrc = hit.GetComponent<orcController>();
            if (greenOrc != null)
            {
                greenOrc.TomarDano(meleeDamage);
                Debug.Log("Green Orc atingido: " + hit.name);
                continue;
            }

>>>>>>> Stashed changes
            SlimeController slime = hit.GetComponent<SlimeController>();
            if (slime != null)
            {
                slime.TakeDamage(meleeDamage);
                Debug.Log("Slime atingido: " + hit.name);
                continue;
            }
        }
    }

    private void GetInput()
    {
        moveDir.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Debug.Log("Player has entered a trap!");

            if (canTakeDamage)
            {
                TakeDamage(1f);
            }
        }
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(other.gameObject);
            // AQUI o volume já foi definido em Awake, então não precisa mudar aqui
            if (pickupKey != null)
            {
                pickupKey.Play();
            }
            Debug.Log("Chave coletada!");
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        canMove = false;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        isDead = true;

        // Parar todos os sons do player ao morrer
        if (swordAudio != null && swordAudio.isPlaying) swordAudio.Stop();
        if (pickupKey != null && pickupKey.isPlaying) pickupKey.Stop();
        if (footstepAudio != null && footstepAudio.isPlaying) footstepAudio.Stop();

        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    public void TakeDamage(double damage)
    {
        if (isInvulnerable)
        {
            Debug.Log("Jogador está invulnerável. Ignorando dano.");
            return;
        }

        health -= damage;

        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.UpdateHearts((int)health);
        }

        if (health <= 0)
        {
            Die();
        }

        isInvulnerable = true;
        StartCoroutine(ResetInvulnerability(1.0f));
    }

    IEnumerator ResetInvulnerability(float time)
    {
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
    }

    void Start()
    {
        if (HealthSystem.Instance != null)
            HealthSystem.Instance.UpdateHearts((int)maxHealth);
        isDead = false;
    }

    private void MovePlayer()
    {
        rb.linearVelocity = (canMove ? 1 : 0) * moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity.Normalize();
    }

    private void HandleFootstepSound()
    {
        if (moveDir.magnitude == 0)
        {
            if (footstepAudio != null && footstepAudio.isPlaying) // Adicionado verificação para evitar NullReference se footstepAudio for nulo
            {
                footstepAudio.Stop(); // Para o som quando parar
            }
            return;
        }

        if (Time.time - lastFootstepTime >= footstepCooldown)
        {
            if (footstepAudio != null && footstepSounds.Length > 0)
            {
                // O volume agora é definido em Awake(), então remova a linha daqui
                // footstepAudio.volume = 0.9f; 

                AudioClip stepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
                footstepAudio.PlayOneShot(stepSound);
                lastFootstepTime = Time.time;
            }
            else
            {
                Debug.LogWarning("PlayerController: Footstep AudioSource ou Clipes de passos não atribuídos!");
            }
        }
    }



    private void CalculateFacingDirection()
    {
        if (moveDir.x > 0)
        {
            currentDirection = Directions.RIGHT;
        }
        else if (moveDir.x < 0)
        {
            currentDirection = Directions.LEFT;
        }
        if (moveDir.y > 0)
        {
            currentDirection = Directions.UP;
        }
        else if (moveDir.y < 0)
        {
            currentDirection = Directions.DOWN;
        }
    }

    private void HandleAnimations()
    {
        animator.SetBool("IsMoving", moveDir.magnitude != 0);
        animator.SetInteger("Direction", (int)currentDirection);

        if (currentDirection == Directions.LEFT || currentDirection == Directions.RIGHT)
        {
            spriteRenderer.flipX = currentDirection == Directions.RIGHT;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        GetInput();
        HandleFootstepSound();
        CalculateFacingDirection();
        HandleAnimations();
        if (timer != null && timer.timeElapsed <= 0 && isDead == false) // Adicionado verificação de nulo para timer
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        canMove = animator.GetBool("CanMove");
        canAttack = animator.GetBool("CanAttack");
    }
}