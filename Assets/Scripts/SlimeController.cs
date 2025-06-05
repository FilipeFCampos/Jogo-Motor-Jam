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
            Debug.LogError("Animator component is missing from the PlayerController GameObject.");
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

        // Registra slime morto
       // int defeated = PlayerPrefs.GetInt("SlimesDefeated", 0);
       // PlayerPrefs.SetInt("SlimesDefeated", defeated + 1);
        animator.SetTrigger("Die");

        StartCoroutine(DestroyAfterAnimation());
    }

    // Wait for Death Animation to finish before destroying the object
    private IEnumerator DestroyAfterAnimation()
    {
        // Disable collider and movement logic while animation plays
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        // Wait for the animation to finish
        // Get the length of the current animation clip
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        timer.ResetTimer();

        yield return new WaitForSeconds(animationLength);
        // Destroy the gameObject after animation completes
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            animator.SetTrigger("Attack");
            if (playerController != null)
            {
                playerController.TakeDamage(5.02);
            }
        }
    }
}
