using UnityEngine;
using UnityEngine.UI;

public class ToggleMusicButton : MonoBehaviour
{
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Image buttonImage;
    public AudioManager audioManager;

    private bool isMusicOn = true;

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        // Trocar o sprite do botão
        buttonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;

        // Tocar ou pausar a música
        if (isMusicOn)
            audioManager.PlayMusic();
        else
            audioManager.PauseMusic();
    }
}
