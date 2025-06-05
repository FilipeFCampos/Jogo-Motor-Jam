using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timeElapsed = 25.1f;

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed > 0)
        {
            UpdateTimer();
        }
        else
        {
            timerText.text = "0.0";
            enabled = false;
        }
    }

    // Updates the remaining time and updates the UI text
    void UpdateTimer()
    {
        timeElapsed -= Time.deltaTime;
        timerText.text = timeElapsed.ToString("F1");
    }

    public void ResetTimer()
    {
        timeElapsed += 5.2f;
        if (timeElapsed > 25.1f)
        {
            timeElapsed = 25.1f; // Cap the timer at 25.1 seconds
        }
        timerText.text = timeElapsed.ToString("F1");
        enabled = true; // Re-enable the timer if it was disabled
    }
}
