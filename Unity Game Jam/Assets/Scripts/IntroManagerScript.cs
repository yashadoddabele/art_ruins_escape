using UnityEngine;

// Manages the intro screen of the game
public class IntroScreenManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressed space");
            LevelManager.Instance.LoadNextLevel();
            Debug.Log("load next level");
            Destroy(gameObject);
        }
    }
}

