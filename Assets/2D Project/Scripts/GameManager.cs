using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")] [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject overlayPanel;
    private int score;
    private int highScore;
    private const string HighScoreKey = "HighScore";
    void Start()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        score = 0;
        UpdateUI();

        // Subscribe to enemy death event
        Enemy.OnEnemyDied += OnEnemyDied;
    }
    void OnDestroy()
    {
        // Always unsubscribe to avoid duplicates when reloading scenes / entering play mode again
        Enemy.OnEnemyDied -= OnEnemyDied;
    }

    private void OnEnemyDied(float enemyScore)
    {
        score += Mathf.RoundToInt(enemyScore);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = score.ToString("D4");
        if (highScoreText) highScoreText.text = highScore.ToString("D4");
    }
}