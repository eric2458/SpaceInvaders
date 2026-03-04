using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip ticClip;
    public AudioClip tacClip;

    [SerializeField] private float points = 10f;

    public delegate void EnemyDiedFunc(float points);
    public static event EnemyDiedFunc OnEnemyDied;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);

            OnEnemyDied?.Invoke(points);

            Destroy(gameObject);
        }
    }

    public void PlayTicSound()
    {
        GetComponent<AudioSource>().PlayOneShot(ticClip);
    }

    public void PlayTacSound()
    {
        GetComponent<AudioSource>().PlayOneShot(tacClip);
    }
}