using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class CreditsController : MonoBehaviour
{
    void Start()
    {
        // Make sure time is running
        Time.timeScale = 1f;
    }

    void Update()
    {
        bool spacePressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
            spacePressed = Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        spacePressed = Input.GetKeyDown(KeyCode.Space);
#endif

        if (spacePressed)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}