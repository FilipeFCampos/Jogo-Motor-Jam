using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    

    /* Combat related variables */
    public float health;
    private bool isInvulnerable = false;

    private bool canAttack = true; // Flag to control attack state
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float meleeDamage = 1f;
    [SerializeField] private float meleeRange = 1f;
    private bool canTakeDamage = true; // Flag to control if the player can take damage

    [SerializeField] private LayerMask enemyLayer;

    // Awake is called when the script instance is being loaded
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
    }

    // Handles player attack logic
    private void Attack()
    {
        if (!canAttack)
        {
            Debug.Log("Cannot attack right now.");
            return; // Exit if the player cannot attack
        }

        Debug.Log("Player attacks with melee damage: " + meleeDamage);
        animator.SetTrigger("Attack");
        canMove = false; // Disable movement during attack

        // Novo: detectar inimigos na �rea de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Verifica se o inimigo tem o script do boss
            BossController boss = enemy.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TomarDano(meleeDamage);
                Debug.Log("Boss atingido: " + enemy.name);
            }
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
        if (other.CompareTag("Trap")) {
            Debug.Log("Player has entered a trap!");

            if (canTakeDamage) {
                TakeDamage(1f); // Take damage from the trap
                //canTakeDamage = false; // Disable further damage until reset
            }
        }
    }

    // Handles player death and game over logic
    private void Die()
    {
        gameObject.SetActive(false); // Deactivate the player GameObject
        canTakeDamage = true; // Reset damage taking ability
        rb.linearVelocity = Vector2.zero; // Stop player movement
        animator.SetTrigger("Die"); // Trigger death animation
        canMove = true; // Disable movement on death
        canAttack = true; // Disable attack on death
        currentDirection = Directions.DOWN;
        health = maxHealth;
        FindFirstObjectByType<GameManager>().PlayerDied(); // Notify the GameManager that the player has died
    }

    // Handles player damage taking logic
    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            Debug.Log("Jogador está invulnerável. Ignorando dano.");
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
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
    }

    // Move the player based on move direction and speed
    private void MovePlayer()
    {
        rb.linearVelocity = (canMove ? 1 : 0) * moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity.Normalize();
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
        CalculateFacingDirection();
        HandleAnimations();
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    private void FixedUpdate()
    {
        // Move the player based on input
        MovePlayer();
        canMove = animator.GetBool("CanMove");
        canAttack = animator.GetBool("CanAttack");
    }

}