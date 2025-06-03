using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timeElapsed = 25.1f;

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed > 0) {
            UpdateTimer();
        }
        else {
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
}
