using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Class that handles scene management
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public Dictionary<int, Vector3[]> levelData;
    public int totalLevels = 3;
    public int currentLevel;
    private int deathCount = 0;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

    }

    // private void Start()
    // {
    //     loadIntro();
    // }

    // Iterate to the next level when the player has reached the exit point
    public void LoadNextLevel()
    {
        currentLevel++;
        Debug.Log(currentLevel);

        if (currentLevel <= totalLevels)
        {
            LoadLevel();
        }
        else
        {
            //End screen
            loadEnd();
        }
    }

    // Loads a scene
    private void LoadLevel()
    {
        string sceneToLoad = "Level" + currentLevel;
        Debug.Log("Loading scene: " + sceneToLoad);
        if (Time.timeScale != 0) {
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }

    public void loadIntro() {
        SceneManager.LoadSceneAsync("Intro");
    }

    public void loadEnd() {
         SceneManager.LoadSceneAsync("End");
    }

    public int getDeathCount() {
        return deathCount;
    }

    public void incrementDeathCount() {
        deathCount++;
    }

    public void restartLoop() {
        currentLevel = 0;
        LoadNextLevel();
    }
}