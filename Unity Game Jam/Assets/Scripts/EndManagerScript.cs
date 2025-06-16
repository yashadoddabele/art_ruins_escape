using UnityEngine;

// Manages the end screen of the game
public class EndScreenManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.restartLoop();
            Destroy(gameObject);
        }
    }
}
