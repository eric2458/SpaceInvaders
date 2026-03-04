using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Prefabs (4 types)")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Grid")]
    [SerializeField] private int columns = 8;
    [SerializeField] private int rows = 4;
    [SerializeField] private float spacingX = 1.2f;
    [SerializeField] private float spacingY = 1.0f;
    [SerializeField] private Vector2 startOffset = new Vector2(-4f, 3.5f);

    [Header("Movement (Discrete Steps)")]
    [SerializeField] private float stepX = 0.4f;
    [SerializeField] private float stepDown = 0.35f;
    [SerializeField] private float startStepInterval = 0.9f;

    [Header("Speed Up")]
    [SerializeField] private float minStepInterval = 0.15f;

    [Header("Bounds")]
    [SerializeField] private float leftEdge = -8.5f;
    [SerializeField] private float rightEdge = 8.5f;

    private float stepInterval;
    private float timer;
    private int direction = 1;
    private int initialEnemyCount;

    void Start()
    {
        stepInterval = startStepInterval;

        SpawnGrid();
        initialEnemyCount = transform.childCount;

        Enemy.OnEnemyDied += OnEnemyDied;
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= stepInterval)
        {
            timer = 0f;
            StepMove();
        }
    }

    private void SpawnGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int typeIndex = Mathf.Clamp(r, 0, enemyPrefabs.Length - 1);
                GameObject prefab = enemyPrefabs[typeIndex];

                Vector3 pos = new Vector3(
                    startOffset.x + c * spacingX,
                    startOffset.y - r * spacingY,
                    0f
                );

                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }
    }

    private void StepMove()
    {
        if (transform.childCount == 0) return;

        float leftMost = GetLeftMostX();
        float rightMost = GetRightMostX();

        bool wouldHitRight = (rightMost + direction * stepX) > rightEdge;
        bool wouldHitLeft = (leftMost + direction * stepX) < leftEdge;

        if (wouldHitLeft || wouldHitRight)
        {
            transform.position += Vector3.down * stepDown;
            direction *= -1;
        }
        else
        {
            transform.position += Vector3.right * direction * stepX;
        }
    }

    private float GetLeftMostX()
    {
        float leftMost = float.PositiveInfinity;
        foreach (Transform child in transform)
            leftMost = Mathf.Min(leftMost, child.position.x);
        return leftMost;
    }

    private float GetRightMostX()
    {
        float rightMost = float.NegativeInfinity;
        foreach (Transform child in transform)
            rightMost = Mathf.Max(rightMost, child.position.x);
        return rightMost;
    }

    private void OnEnemyDied(float points)
    {
        int alive = transform.childCount;
        if (initialEnemyCount <= 0) return;

        float t = 1f - (alive / (float)initialEnemyCount);
        stepInterval = Mathf.Lerp(startStepInterval, minStepInterval, t);
    }
}