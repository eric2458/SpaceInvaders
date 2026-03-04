using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Overlay (assign a full-screen panel and a child TMP text)")]
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private TextMeshProUGUI overlayMessage;
    [SerializeField] private float overlayAutoHideSeconds = 3f;

    private int score;
    private int highScore;
    private const string HighScoreKey = "HighScore";

    // overlay state
    private bool overlayActive = false;
    private float overlayTimer = 0f;
    private bool overlayWasGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        score = 0;
        UpdateUI();
        
        Enemy.OnEnemyDied += OnEnemyDied;
        
        ShowOverlay("PRESS SPACE TO START", overlayAutoHideSeconds, false);
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied;
    }

    void Update()
    {
        if (!overlayActive) return;
        
        overlayTimer -= Time.unscaledDeltaTime;

        bool spacePressed = false;
        
        if (spacePressed || overlayTimer <= 0f)
        {
            if (overlayWasGameOver)
            {
                RestartLevel();
            }
            else
            {
                HideOverlayAndStart();
            }
        }
    }
    
    public void ShowOverlay(string message, float autoHideSeconds = 3f, bool isGameOver = false)
    {
        if (overlayPanel != null) overlayPanel.SetActive(true);
        if (overlayMessage != null) overlayMessage.text = message;

        overlayActive = true;
        overlayTimer = autoHideSeconds;
        overlayWasGameOver = isGameOver;
        
        Time.timeScale = 0f;
    }

    private void HideOverlayAndStart()
    {
        if (overlayPanel != null) overlayPanel.SetActive(false);

        overlayActive = false;
        overlayTimer = 0f;
        overlayWasGameOver = false;

        // resume gameplay
        Time.timeScale = 1f;
    }
    
    private void OnEnemyDied(float enemyScore)
    {
        int pts = Mathf.RoundToInt(enemyScore);
        AddScore(pts);
    }

    public void AddScore(int points)
    {
        score += points;
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
        scoreText.text = $"{score:D4}";
        highScoreText.text = $"{highScore:D4}";
    }
    public void GameOver()
    {

        ShowOverlay("GAME OVER\nPRESS SPACE TO RESTART", float.MaxValue, true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}