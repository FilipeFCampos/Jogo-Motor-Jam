using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum Estado { Idle, Andando, Atacando, Dano, Morto }
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

    private int direcaoAtual = 1; // 0=cima, 1=baixo, 2=esquerda, 3=direita

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
        if (estadoAtual == Estado.Morto)
            return;

        float distancia = Vector2.Distance(transform.position, alvo.position);

        if (estadoAtual != Estado.Idle && barraUI != null && !barraUI.activeSelf)
            barraUI.SetActive(true);

        switch (estadoAtual)
        {
            case Estado.Idle:
                AtualizarAnimacao(Vector2.zero);
                if (distancia < 5f)
                    MudarEstado(Estado.Andando);
                break;

            case Estado.Andando:
                if (distancia <= alcanceDeAtaque)
                {
                    if (podeAtacar)
                    {
                        MudarEstado(Estado.Atacando);
                    }
                    else
                    {
                        PararMovimento();
                    }
                }
                else
                {
                    MoverAteAlvo();
                }
                break;

            case Estado.Atacando:
                // Durante ataque não faz nada para evitar reiniciar ataque
                break;

            case Estado.Dano:
                // Durante dano não faz nada para evitar conflito
                break;
        }
    }

    void MoverAteAlvo()
    {
        Vector2 direcao = (alvo.position - transform.position).normalized;
        AtualizarDirecao(direcao);
        rb.MovePosition(rb.position + direcao * velocidade * Time.deltaTime);
        AtualizarAnimacao(direcao);
    }

    void PararMovimento()
    {
        AtualizarAnimacao(Vector2.zero);
    }

    void AtualizarDirecao(Vector2 direcaoMovimento)
    {
        if (direcaoMovimento != Vector2.zero)
        {
            if (Mathf.Abs(direcaoMovimento.x) > Mathf.Abs(direcaoMovimento.y))
                direcaoAtual = direcaoMovimento.x > 0 ? 3 : 2;
            else
                direcaoAtual = direcaoMovimento.y > 0 ? 0 : 1;
        }
    }

    IEnumerator Atacar()
    {
        podeAtacar = false;

        anim.SetInteger("direcao", direcaoAtual);
        anim.SetInteger("estado", 2); // ataque

        float tempoAnimacaoAtaque = 1.2f; // ajuste conforme sua animação

        yield return new WaitForSeconds(tempoAnimacaoAtaque);

        if (estadoAtual != Estado.Morto)
            MudarEstado(Estado.Andando);

        podeAtacar = true;
    }

    public void LevarDano(int dano)
    {
        if (estadoAtual == Estado.Morto)
            return;

        vida -= dano;
        if (vida <= 0)
        {
            vida = 0;
            MudarEstado(Estado.Morto);
            rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            if (barraUI != null)
                barraUI.SetActive(false);
            anim.SetInteger("estado", 4); // morto
            Destroy(gameObject, 3f);
        }
        else
        {
            MudarEstado(Estado.Dano);
            StartCoroutine(VoltarAoAndarDepoisDano());
        }
    }

    IEnumerator VoltarAoAndarDepoisDano()
    {
        anim.SetInteger("direcao", direcaoAtual);
        anim.SetInteger("estado", 3); // dano
        yield return new WaitForSeconds(0.7f);
        if (estadoAtual != Estado.Morto)
            MudarEstado(Estado.Andando);
    }

    void MudarEstado(Estado novoEstado)
    {
        if (estadoAtual == novoEstado)
            return;

        estadoAtual = novoEstado;

        switch (estadoAtual)
        {
            case Estado.Idle:
                anim.SetInteger("estado", 0);
                break;

            case Estado.Andando:
                anim.SetInteger("estado", 1);
                break;

            case Estado.Atacando:
                StartCoroutine(Atacar());
                break;

            case Estado.Dano:
                // animação e lógica no coroutine VoltarAoAndarDepoisDano
                break;

            case Estado.Morto:
                anim.SetInteger("estado", 4);
                break;
        }

        anim.SetInteger("direcao", direcaoAtual);
    }

    void AtualizarAnimacao(Vector2 direcaoMovimento)
    {
        AtualizarDirecao(direcaoMovimento);

        int estadoAnimacao = estadoAtual switch
        {
            Estado.Idle => 0,
            Estado.Andando => 1,
            Estado.Atacando => 2,
            Estado.Dano => 3,
            Estado.Morto => 4,
            _ => 0
        };

        anim.SetInteger("direcao", direcaoAtual);
        anim.SetInteger("estado", estadoAnimacao);
    }
}
