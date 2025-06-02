using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum Estado { Idle, Perseguindo, Atacando, Dano, Morto }
    public Estado estadoAtual = Estado.Idle;

    public Transform alvo;
    public GameObject barraUI;

    public float alcanceDeAtaque = 3f;
    public float velocidade = 5f;

    public int vidaMaxima = 100;
    public int vida = 100;

    private Animator anim;
    private Rigidbody2D rb;
    private bool podeAtacar = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        vida = vidaMaxima;

        if (barraUI != null)
            barraUI.SetActive(false);
    }

    void Update()
    {
        if (estadoAtual == Estado.Morto) return;

        float distancia = Vector2.Distance(transform.position, alvo.position);

        if (estadoAtual != Estado.Idle && barraUI != null && !barraUI.activeSelf)
            barraUI.SetActive(true);

        switch (estadoAtual)
        {
            case Estado.Idle:
                AtualizarAnimacao(Vector2.zero);
                if (distancia < 5f)
                    estadoAtual = Estado.Perseguindo;
                break;

            case Estado.Perseguindo:
                if (distancia <= alcanceDeAtaque)
                {
                    estadoAtual = Estado.Atacando;
                }
                else
                {
                    MoverAteAlvo();
                }
                break;

            case Estado.Atacando:
                if (podeAtacar)
                    StartCoroutine(Atacar());

                if (distancia > alcanceDeAtaque + 0.5f)
                    estadoAtual = Estado.Perseguindo;
                break;

            case Estado.Dano:
                AtualizarAnimacao(Vector2.zero);
                break;
        }
    }

    void MoverAteAlvo()
    {
        Vector2 direcao = (alvo.position - transform.position).normalized;
        rb.MovePosition(rb.position + direcao * velocidade * Time.deltaTime);
        AtualizarAnimacao(direcao);
    }

    IEnumerator Atacar()
    {
        podeAtacar = false;
        AtualizarAnimacao(Vector2.zero);

        yield return new WaitForSeconds(1.5f);

        podeAtacar = true;

        if (estadoAtual == Estado.Atacando)
            estadoAtual = Estado.Perseguindo;
    }

    public void LevarDano(int dano)
    {
        if (estadoAtual == Estado.Morto) return;

        vida -= dano;
        estadoAtual = Estado.Dano;

        AtualizarAnimacao(Vector2.zero);

        if (vida <= 0)
        {
            vida = 0;
            estadoAtual = Estado.Morto;

            rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            AtualizarAnimacao(Vector2.zero);

            if (barraUI != null)
                barraUI.SetActive(false);

            Destroy(gameObject, 3f);
        }
        else
        {
            Invoke(nameof(VoltarAPerseguir), 0.5f);
        }
    }

    void VoltarAPerseguir()
    {
        if (estadoAtual != Estado.Morto)
            estadoAtual = Estado.Perseguindo;
    }

    void AtualizarAnimacao(Vector2 direcaoMovimento)
    {
        int direcao = 1; // Padrão: Baixo

        if (direcaoMovimento != Vector2.zero)
        {
            if (Mathf.Abs(direcaoMovimento.x) > Mathf.Abs(direcaoMovimento.y))
                direcao = direcaoMovimento.x > 0 ? 3 : 2; // Direita ou Esquerda
            else
                direcao = direcaoMovimento.y > 0 ? 0 : 1; // Cima ou Baixo
        }

        int estado = estadoAtual switch
        {
            Estado.Idle => 0,
            Estado.Perseguindo => 1,
            Estado.Atacando => 2,
            Estado.Dano => 3,
            Estado.Morto => 4,
            _ => 0
        };

        anim.SetInteger("direcao", direcao);
        anim.SetInteger("estado", estado);
    }
}
