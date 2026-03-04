using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float leftBound = -8f;
    public float rightBound = 8f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootOffsetTransform;

    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed)
                moveInput = 1f;
        }
        
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        float clampedX = Mathf.Clamp(transform.position.x, leftBound, rightBound);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void HandleShooting()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameObject shot = Instantiate(bulletPrefab, shootOffsetTransform.position, Quaternion.identity);
            Destroy(shot, 3f);

            if (animator != null)
                animator.SetTrigger("Shot Trigger");

            Debug.Log("Bang!");
        }
    }
}