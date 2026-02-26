using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");
        
        // todo - destroy the bullet
        Destroy(shot);
        // todo - trigger death animation
    }
}
