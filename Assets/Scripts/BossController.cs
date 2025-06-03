using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public enum Estado { Andando, Atacando, TomandoDano, Morto }

    private Estado estadoAtual = Estado.Andando;

    [SerializeField] private float vidaMaxima = 10f;
    private float vidaAtual;

    public float VidaAtual => vidaAtual;
    public float VidaMaxima => vidaMaxima;

    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float distanciaAtaque = 1.5f;
    [SerializeField] private float tempoEntreAtaques = 2f;
    private float tempoUltimoAtaque;

    private Transform jogador;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 direcao;
    private int direcaoAtual;

    private bool podeAtacar = true;
    private bool podeMover = true;

    private void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        vidaAtual = vidaMaxima;
        tempoUltimoAtaque = -tempoEntreAtaques;
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
        sr.flipX = (direcaoAtual == 3); // Flip se for Right
    }

    private IEnumerator Atacar()
    {
        podeAtacar = false;
        podeMover = false;

        anim.SetTrigger("Attack");
        tempoUltimoAtaque = Time.time;

        yield return new WaitForSeconds(1f);

        if (estadoAtual != Estado.Morto)
        {
            MudarEstado(Estado.Andando);
        }

        podeAtacar = true;
        podeMover = true;
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

        yield return new WaitForSeconds(0.5f);

        if (estadoAtual != Estado.Morto)
        {
            MudarEstado(Estado.Andando);
            podeMover = true;
        }
    }

    private void Morrer()
    {
        MudarEstado(Estado.Morto);
        anim.SetTrigger("Die");
        podeMover = false;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    private void AtualizarFlagsAnimator()
    {
        podeMover = anim.GetBool("CanMove");
        podeAtacar = anim.GetBool("CanAttack");
    }

    private void MudarEstado(Estado novoEstado)
    {
        estadoAtual = novoEstado;
    }

    // ✅ Novo método: verifica se o boss está visível na câmera
    private bool EstaNaCamera()
    {
        if (Camera.main == null) return false;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // Margem interna da tela (aumente para reduzir o campo de visão do boss)
        float margem = 0.2f;

        return viewportPos.x >= margem && viewportPos.x <= 1f - margem &&
               viewportPos.y >= margem && viewportPos.y <= 1f - margem &&
               viewportPos.z > 0;
    }
}
