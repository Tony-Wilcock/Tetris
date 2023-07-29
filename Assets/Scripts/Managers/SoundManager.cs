using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    // sound effect of a row clearing
    public AudioClip m_clearRowSound;

    // "tick" sound effect when we move the Shape left/right
    public AudioClip m_moveSound;

    // sound effect when a Shape hits the bottom
    public AudioClip m_dropSound;

    // sound effect when the game is over
    public AudioClip m_gameOverSound;

    // vocal effect for game over
    public AudioClip m_gameOverVocal;

    // delay between game over sound effect and game over vocal
    public float m_gameOverDelay;

    // sound when we press the Hold button
    public AudioClip m_holdSound;

    // vocal effects when we clear more than one line
    public AudioClip[] m_vocalClips;
    AudioClip m_randomVocalClip;

    // sound when we level up
    public AudioClip m_levelUpClip;

    // background music clips
    public AudioClip[] m_musicClips;
    AudioClip m_randomMusicClip;

    // audio source for the music object
    public AudioSource m_musicSource;

    // toggle for whether the music is enabled
    public bool m_musicEnabled = true;
    public IconToggle m_musicIconToggle;

    // toggle for whether the sound effect is enabled
    public bool m_fxEnabled = true;

    public IconToggle m_fxIconToggle;

    // music volume
    [Range(0, 1)]
    public float m_musicVolume = 1.0f;


    // multiplier for FX volumne, always one or zero
    float m_fxMultiplier = 1f;




    // sound effect volume
    [Range(0, 1)]
    public float m_fxVolume = 1.0f;

    // good base volume for vocals
    float m_vocalVolume = 1f;



    void Start()
    {
        m_randomMusicClip = GetRandomClip(m_musicClips);
        PlayBackgroundMusic(m_randomMusicClip);
    }

    // only used by the Editor

    void Update()
    {
        SetMusic();
    }

    void SetMusic()
    {
        if (m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                PlayBackgroundMusic(m_randomMusicClip);
            }
            else
            {
                m_musicSource.Stop();
            }

            if (m_musicIconToggle)
            {
                m_musicIconToggle.ToggleIcon(m_musicEnabled);
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        SetMusic();
    }

    public void ToggleMusic(bool musicEnabled)
    {
        m_musicEnabled = musicEnabled;
        SetMusic();

    }

    public void ToggleFX()
    {
        // toggle our Boolean
        m_fxEnabled = !m_fxEnabled;

        // we need to convert our Boolean to a float value so we can use it later; it must always be one (fx on) or zero (fx off)
        // according to DotNetPerls you can do this
        m_fxMultiplier = (m_fxEnabled) ? 1 : 0;



        // or this... if you are using System but I won't because then it interferes with the Random function from Unity
        //m_fxMultiplier = Convert.ToInt32(m_fxEnabled);

        // toggle our icon
        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }

    }





    // replaces AudioSource.PlayClipAtPoint
    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
    {
        //Create an empty game object
        GameObject go = new GameObject("SoundFX" + clip.name);
        go.transform.position = position;

        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * m_fxMultiplier;

        source.Play();
        Destroy(go, clip.length);
        return source;
    }

    // overloaded with pitch option
    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 point, float volume, float pitch)
    {
        //Create an empty game object
        GameObject go = new GameObject("SoundFX" + clip.name);
        go.transform.position = point;

        //Create the source
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * m_fxMultiplier;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }

    // play a random clip if given an array of clips
    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        // return if music is disabled or if musicSource is null or is musicClip is null
        if (!m_musicEnabled || !musicClip || !m_musicSource)
            return;

        m_musicSource.Stop();

        m_musicSource.clip = musicClip;

        // set the music volume
        m_musicSource.volume = m_musicVolume;

        // music repeats forever
        m_musicSource.loop = true;

        // start playing
        m_musicSource.Play();

    }

    // play a random voice praising the player
    public void PlayVocals()
    {
        m_randomVocalClip = GetRandomClip(m_vocalClips);
        PlayClipAtPoint(m_randomVocalClip, Camera.main.transform.position, m_vocalVolume);

    }

    public void PlayGameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    // play game over sound then game over vocals
    IEnumerator GameOverCoroutine()
    {
        if (m_musicSource)
        {
            m_musicSource.Stop();
        }

        m_musicEnabled = false;

        if (m_gameOverSound)
            // play the game over sound at max volume
            PlayClipAtPoint(m_gameOverSound, Camera.main.transform.position, 1f);

        float duration = Mathf.Clamp(m_gameOverSound.length - 4, 0.5f, 2f);

        // wait for the duration of the clip
        yield return new WaitForSeconds(duration);

        // play the game over vocal
        if (m_gameOverVocal)
            PlayClipAtPoint(m_gameOverVocal, Camera.main.transform.position, 1f);

    }

}
