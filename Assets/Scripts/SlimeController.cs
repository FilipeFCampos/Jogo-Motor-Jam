using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private float health = 5.02f;
    [SerializeField] private float moveSpeed = 1.5f;
    private Vector2 moveDirection;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Random.insideUnitCircle.normalized; // movimento aleatório inicial
    }

    private void Update()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
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
        Destroy(gameObject); // Remove o slime da cena
    }

    // Exemplo: se for atingido por ataque do player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(1f); // valor de dano fixo ou baseado no player
        }
    }
}
