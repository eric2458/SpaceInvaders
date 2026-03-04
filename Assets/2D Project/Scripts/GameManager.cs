using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI (assign value text objects)")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Overlay (assign a full-screen panel and a child TMP text)")]
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private TextMeshProUGUI overlayMessage;
    [SerializeField] private float overlayAutoHideSeconds = 3f;

    [Header("Barricade Spawning")]
    [SerializeField] private GameObject barricadePrefab;
    [SerializeField] private Transform[] barricadeSpawnPoints;
    [SerializeField] private int killsToSpawn = 3;
    [SerializeField] private float spawnCheckRadius = 0.4f;

    private int score;
    private int highScore;
    private const string HighScoreKey = "HighScore";
    
    private bool overlayActive = false;
    private float overlayTimer = 0f;
    private bool overlayWasGameOver = false;
    
    private int killCounter = 0;

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
        
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
            spacePressed = Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.spaceKey.isPressed;
        #else
        spacePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space);
        #endif
        
        if (spacePressed)
            Debug.Log($"[GameManager] Space detected. overlayWasGameOver={overlayWasGameOver}, overlayTimer={overlayTimer}");

        if (spacePressed || overlayTimer <= 0f)
        {
            if (overlayWasGameOver)
            {
                Debug.Log("[GameManager] Restarting level...");
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
        
        Time.timeScale = 1f;
    }
    
    private void OnEnemyDied(float enemyScore)
    {
        AddScore(Mathf.RoundToInt(enemyScore));
        
        killCounter++;
        if (killCounter >= killsToSpawn)
        {
            TrySpawnBarricade();
            killCounter = 0;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
            Debug.Log("[GameManager] New high score saved: " + highScore);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{score:D4}";
        if (highScoreText != null)
            highScoreText.text = $"{highScore:D4}";
    }
    
    private void TrySpawnBarricade()
    {
        if (barricadePrefab == null || barricadeSpawnPoints == null || barricadeSpawnPoints.Length == 0)
            return;
        
        System.Collections.Generic.List<Transform> free = new System.Collections.Generic.List<Transform>();

        foreach (Transform sp in barricadeSpawnPoints)
        {
            if (sp == null) continue;
            
            Collider2D found = Physics2D.OverlapCircle(sp.position, spawnCheckRadius);

            if (found == null || !found.CompareTag("Barricade"))
            {
                free.Add(sp);
            }
        }
        
        if (free.Count == 0)
        {
            Debug.Log("[GameManager] No available spawn point for barricade (all occupied).");
            return;
        }
        
        Transform chosen = free[Random.Range(0, free.Count)];
        Instantiate(barricadePrefab, chosen.position, chosen.rotation);
        Debug.Log("[GameManager] Spawned barricade at " + chosen.name);
    }
    
    void OnDrawGizmosSelected()
    {
        if (barricadeSpawnPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (var sp in barricadeSpawnPoints)
            if (sp != null) Gizmos.DrawWireSphere(sp.position, spawnCheckRadius);
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