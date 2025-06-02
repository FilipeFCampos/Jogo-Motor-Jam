using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Image foregroundImage;  // imagem vermelha, com Image Type = Filled e Fill Method = Horizontal
    public BossController boss;    // seu boss

    void Update()
    {
        if (boss != null && foregroundImage != null)
        {
            float ratio = (float)boss.vida / boss.vidaMaxima;
            ratio = Mathf.Clamp01(ratio);

            foregroundImage.fillAmount = ratio;
        }
    }
}
