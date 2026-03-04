using UnityEngine;

public class Barricade : MonoBehaviour
{
    [SerializeField] private int health = 5;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet") ||
            collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        health--;
        
        transform.localScale *= 0.8f;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}