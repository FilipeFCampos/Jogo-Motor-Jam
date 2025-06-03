using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Image foregroundImage;  // imagem vermelha, Image Type = Filled, Fill Method = Horizontal
    public BossController boss;    // referência ao boss

    void Update()
    {
        if (boss != null && foregroundImage != null)
        {
            float ratio = boss.VidaAtual / boss.VidaMaxima;
            ratio = Mathf.Clamp01(ratio);

            foregroundImage.fillAmount = ratio;
        }
    }
}
