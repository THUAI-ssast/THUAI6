using TMPro;
using UnityEngine;

public class TimeLeftView : MonoBehaviour
{
    private TextMeshProUGUI _timeLeftText;

    private void Awake()
    {
        _timeLeftText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        float timeLeft = GameModel.Instance.timeLeft; // unit: second
        OnTimeLeftChanged(this, timeLeft);
        GameModel.Instance.TimeLeftChangedEvent += OnTimeLeftChanged;
    }

    private void OnTimeLeftChanged(object sender, float timeLeft)
    {
        _timeLeftText.text = string.Format("{0:00}:{1:00}", (int)timeLeft / 60, (int)timeLeft % 60);
    }
}
