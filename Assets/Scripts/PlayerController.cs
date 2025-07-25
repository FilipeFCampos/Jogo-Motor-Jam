using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    /* Enums */
    private enum Directions { UP, DOWN, LEFT, RIGHT };

    public static PlayerController Instance { get; private set; }
    public enum Ability { BLACK, BLUE, GREEN, RED };

    /* Basic player movement variables */
    private Vector2 moveDir;
    [SerializeField] private float moveSpeed;
    private Directions currentDirection;
    private bool canMove = true; // Flag to control player movement

    /* References to components */
    public PowerTimer powerBar;
    [SerializeField] private Rigidbody2D rb;
    // Animation related references
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource swordAudio;
    [SerializeField] private AudioSource pickupKey;
    [SerializeField] private AudioSource footstepAudio; // AudioSource para os passos
    [SerializeField] private AudioClip[] footstepSounds; // Array de sons variados de passos

    private float footstepCooldown = 0.3f; // Tempo entre cada som de passo
    private float lastFootstepTime;


    /* Combat related variables */
    public double health;
    private bool isInvulnerable = false;
    private bool isDead = false; // Flag to check if the player is dead
    private bool canAttack = true; // Flag to control attack state
    [SerializeField] private float maxHealth;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeRange;
    private bool canTakeDamage = true; // Flag to control if the player can take damage

    [SerializeField] private LayerMask enemyLayer;
    public Timer timer;

    // Awake is called when the script instance is being loaded

    public bool hasKey = false;
    private float defaultHealth;
    private float defaultSpeed;
    private float defaultAttackDamage;
    private float defaultMeleeRange;

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
        powerBar = FindFirstObjectByType<PowerTimer>();

        // Set default values for variables
        defaultHealth = maxHealth;
        defaultSpeed = moveSpeed;
        defaultAttackDamage = meleeDamage;
        defaultMeleeRange = meleeRange;
    }

    private void Attack()
    {
        if (!canAttack)
        {
            Debug.Log("Cannot attack right now.");
            return;
        }

        if (swordAudio != null && !swordAudio.isPlaying)
        {
            swordAudio.Play();
        }

        Debug.Log("Player attacks with melee damage: " + meleeDamage);
        animator.SetTrigger("Attack");
        canMove = false;

        // Detecta todos os colliders da camada "enemyLayer" dentro do raio meleeRange
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRange, enemyLayer);

        foreach (Collider2D hit in hitEnemies)
        {
            // 1) Se for Boss
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TomarDano(meleeDamage);
                Debug.Log("Boss atingido: " + hit.name);
                continue;
            }

            OrcController greenOrc = hit.GetComponent<OrcController>();
            if(greenOrc != null)
            {
                greenOrc.TomarDano(meleeDamage);
                Debug.Log("Green Orc atingido: " + hit.name);
                continue;
            }

            // 2) Se for Slime
            SlimeController slime = hit.GetComponent<SlimeController>();
            if (slime != null)
            {
                slime.TakeDamage(meleeDamage);
                Debug.Log("Slime atingido: " + hit.name);
                continue;
            }

            // 3) Se você tiver outros inimigos, cheque aqui também
            // Exemplo: OutroInimigo outro = hit.GetComponent<OutroInimigo>();
            // if (outro != null) { outro.TakeDamage(meleeDamage); ... }
        }
    }



    // Handles player input
    private void GetInput()
    {
        moveDir.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // Might change to Input.GetAxisRaw for instant response
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
                TakeDamage(1f); // Take damage from the trap
                //canTakeDamage = false; // Disable further damage until reset
            }
        }
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(other.gameObject); // Remove a chave da cena
            pickupKey.Play();
            Debug.Log("Chave coletada!");
        }
    }

    // Handles player death and game over logic
    private void Die()
    {
        // Trigger da animação
        animator.SetTrigger("Die");

        // Para o movimento e ações
        canMove = false;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        ResetAbilities(); // Reseta habilidades do jogador
        powerBar.ResetTimer(0f); // Reseta a barra de poder

        // Desabilita colisões se necessário
        GetComponent<Collider2D>().enabled = false;
        isDead = true;

        // Mostra Game Over
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        Time.timeScale = 0f; // pausa o jogo, mas mantém o jogador visível
    }




    // Handles player damage taking logic
    public void TakeDamage(double damage)
    {
        if (isInvulnerable)
        {
            Debug.Log("Jogador está invulnerável. Ignorando dano.");
            return;
        }

        health -= damage;

        // Atualiza os corações na tela
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.UpdateHearts((int)health);
        }

        if (health <= 0)
        {
            Die();
        }

        // Ativa invulnerabilidade temporária
        isInvulnerable = true;
        StartCoroutine(ResetInvulnerability(1.0f)); // 1 segundo de invulnerabilidade
    }

    IEnumerator ResetInvulnerability(float time)
    {
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
    }

    void Start()
    {
        // Inicializa o HUD com vida máxima
        if (HealthSystem.Instance != null)
            HealthSystem.Instance.UpdateHearts((int)maxHealth);
        isDead = false; // Reset dead state at start
    }

    // Move the player based on move direction and speed
    private void MovePlayer()
    {
        rb.linearVelocity = (canMove ? 1 : 0) * moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity.Normalize();
    }
    //w
    private void HandleFootstepSound()
    {
        if (moveDir.magnitude == 0)
        {
            footstepAudio.Stop(); // Para o som quando parar
            return;
        }

        if (Time.time - lastFootstepTime >= footstepCooldown)
        {
            if (footstepAudio != null && footstepSounds.Length > 0)
            {
                footstepAudio.volume = 0.10f; // Ajuste de volume
                AudioClip stepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
                footstepAudio.PlayOneShot(stepSound);
                lastFootstepTime = Time.time;
            }
        }
    }

    public float GetMaxHealth()
    {
        return maxHealth;
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
        // This function can be used to handle animations based on the player's state
        // For now, we will just set the animator's speed based on movement
        animator.SetBool("IsMoving", moveDir.magnitude != 0);
        // Update animator parameters based on direction
        animator.SetInteger("Direction", (int)currentDirection);

        if (currentDirection == Directions.LEFT || currentDirection == Directions.RIGHT)
        {
            spriteRenderer.flipX = currentDirection == Directions.RIGHT;
        }
        else
        {
            spriteRenderer.flipX = false; // Reset flip for vertical movement
        }
    }


    // Update is called once per frame
    void Update()
    {


        // Get horizontal input
        GetInput();
        HandleFootstepSound(); // Chamado dentro de Update()
        CalculateFacingDirection();
        HandleAnimations();
        if (timer.timeElapsed <= 0 && isDead == false)
        {
            Die();
        }
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    private void FixedUpdate()
    {
        // Move the player based on input
        MovePlayer();
        canMove = animator.GetBool("CanMove");
        canAttack = animator.GetBool("CanAttack");
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    public void AbsorveAbility(int ability)
    {

        Color color = Color.yellow; // Default color
        switch (ability)
        {
            case (int)Ability.BLACK:
                ResetRange();
                ResetSpeed();
                if (meleeDamage < 30f)
                {
                    meleeDamage *= 1.5f; // Aumenta o dano do ataque corpo a corpo
                }
                if (meleeDamage > 30f)
                {
                    meleeDamage = 30f; // Garante que o dano não ultrapasse o máximo
                }
                color = Color.black;
                powerBar.ChangeColor(color); // Muda a cor da barra de poder
                Debug.Log("Absorbed BLACK ability. Melee damage increased to: " + meleeDamage);
                powerBar.ResetTimer(10f); // Reseta o timer da barra de poder
                break;
            case (int)Ability.BLUE:
                ResetAttackDamage();
                ResetSpeed();
                if (meleeRange < 5f)
                {
                    meleeRange *= 2f; // Aumenta o alcance do ataque corpo a corpo
                }
                if (meleeRange > 5f)
                {
                    meleeRange = 5f; // Garante que o alcance não ultrapasse o máximo
                }
                color = Color.blue;
                powerBar.ChangeColor(color); // Muda a cor da barra de poder
                Debug.Log("Absorbed BLUE ability. Melee range increased to: " + meleeRange);
                powerBar.ResetTimer(15f); // Reseta o timer da barra de poder
                break;
            case (int)Ability.GREEN:
                health += 40; // Restaura parte da vida do jogador
                if (health > maxHealth) health = maxHealth; // Garante que a vida não ultrapasse o máximo
                color = Color.green;
                powerBar.ChangeColor(color); // Muda a cor da barra de poder
                Debug.Log("Absorbed GREEN ability. Health increased to: " + health);
                powerBar.ResetTimer(2.5f); // Reseta o timer da barra de poder
                break;
            case (int)Ability.RED:
                ResetAttackDamage();
                ResetRange();
                if (moveSpeed < 400f)
                {
                    moveSpeed *= 1.5f; // Aumenta a velocidade de movimento
                }
                if (moveSpeed > 400f)
                {
                    moveSpeed = 400f; // Garante que a velocidade não ultrapasse o máximo
                }
                color = Color.red;
                powerBar.ChangeColor(color); // Muda a cor da barra de poder
                Debug.Log("Absorbed RED ability. Move speed increased to: " + moveSpeed);
                powerBar.ResetTimer(10f); // Reseta o timer da barra de poder
                break;
            default:
                Debug.LogError("Unknown ability type: " + ability);
                break;
        }
    }

    public void ResetAbilities()
    {
        // Reseta todas as habilidades do jogador para os valores padrão
        meleeDamage = defaultAttackDamage;
        moveSpeed = defaultSpeed;
        maxHealth = defaultHealth;
        meleeRange = defaultMeleeRange;

        // Reset power bar color to default
        powerBar.ChangeColor(Color.white);
        Debug.Log("All abilities reset to default values.");
    }

    public void ResetRange()
    {
        meleeRange = defaultMeleeRange;
        Debug.Log("Melee range reset to default value: " + meleeRange);
    }
    public void ResetAttackDamage()
    {
        meleeDamage = defaultAttackDamage;
        Debug.Log("Melee damage reset to default value: " + meleeDamage);
    }
    public void ResetSpeed()
    {
        moveSpeed = defaultSpeed;
        Debug.Log("Move speed reset to default value: " + moveSpeed);
    }
}