using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip ticClip;
    public AudioClip tacClip;
    public delegate void EnemyDiedFunc(float points);
    public static event EnemyDiedFunc OnEnemyDied;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");

        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);

            OnEnemyDied?.Invoke(10);
        }
        // todo - trigger death animation
    }

    public void PlayTicSound()
    {
        //Debug.Log("tic");
        GetComponent<AudioSource>().PlayOneShot(ticClip);
    }

    public void PlayTacSound()
    {
        //Debug.Log("tac");
        GetComponent<AudioSource>().PlayOneShot(tacClip);
    }
}