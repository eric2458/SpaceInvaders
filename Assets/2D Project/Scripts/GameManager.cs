using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Enemy.OnEnemyDied += OnEnemyDied;
        // todo - sign up for notification about enemy death 
    }

    void OnEnemyDied(float score)
    {
        Debug.Log($"Killed enemy worth {score}");
    }
}
