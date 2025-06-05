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

    private void Die()
    {
        SlimeSpawner.slimeCount--;
        
        // Registra slime morto
       // int defeated = PlayerPrefs.GetInt("SlimesDefeated", 0);
       // PlayerPrefs.SetInt("SlimesDefeated", defeated + 1);
        
        timer.ResetTimer(); // Reseta o timer
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(5.02);
            }
        }
    }
}
