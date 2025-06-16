using UnityEngine;

// Singleton class to handle audio management
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource loopingSfxSource;  // For looping sounds (running, etc.)
    public AudioSource oneShotSfxSource;  // For one-shot effects (jump, pickup, etc.)
    public AudioSource musicSource;       // Already set for background music

    [Header("Audio Clips")]
    public AudioClip jumpClip;
    public AudioClip runningClip;
    public AudioClip pickupClip;
    public AudioClip backgroundMusicClip;
    public AudioClip dyingClip;
    public AudioClip waterClip;
    public AudioClip wallClip;
    public AudioClip leverClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AudioSource[] sources = GetComponents<AudioSource>();
            // Assign each audio source from inspector to a designated purpose
            if (sources.Length >= 2)
            {
                musicSource = sources[0];
                loopingSfxSource = sources[1];
                oneShotSfxSource = sources[2];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Play background music on its dedicated audiosource
        if (backgroundMusicClip != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Bg music clip not assigned");
        }
    }

    // One-shot sound effects
    public void JumpSound()
    {
        oneShotSfxSource.PlayOneShot(jumpClip);
    }

    public void PickupSound()
    {
        oneShotSfxSource.PlayOneShot(pickupClip);
    }

    public void DyingSound() {
        oneShotSfxSource.PlayOneShot(dyingClip);
    }

    public void WaterSound() {
        oneShotSfxSource.PlayOneShot(waterClip);
    }
    public void LeverSound() {
        oneShotSfxSource.PlayOneShot(leverClip);
    }
    public void WallSound() {
        oneShotSfxSource.PlayOneShot(wallClip);
    }

    // Looping sound effect for running
    public void RunSound() 
    {
        if (loopingSfxSource.clip != runningClip)
        {
            loopingSfxSource.clip = runningClip;
            loopingSfxSource.loop = true;
            loopingSfxSource.Play();
        }
        else if (!loopingSfxSource.isPlaying)
        {
            loopingSfxSource.Play();
        }
    }

    // Halts loop when player stops running
    public void StopRunSound()
    {
        if (loopingSfxSource.clip == runningClip && loopingSfxSource.isPlaying)
        {
            loopingSfxSource.Stop();
        }
    }
}