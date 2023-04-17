using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;


    private void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        _scoreText.text = string.Format("{0}:{1}", 0, 0);
    }

    private void Start()
    {
        GameModel.Instance.ScoreChangedEvent += OnScoreChanged;
    }

    private void OnScoreChanged(object sender, int[] teamScore)
    {
        _scoreText.text = string.Format("{0}:{1}", teamScore[0], teamScore[1]);
    }
}
