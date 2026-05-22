using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Mixer")]
    [Tooltip("Audio Mixer para controle de volume")]
    public AudioMixer audioMixer;
    
    [Header("Music")]
    [Tooltip("AudioSource para música de fundo")]
    public AudioSource musicSource;
    
    [Tooltip("Música do menu principal")]
    public AudioClip menuMusic;
    
    [Tooltip("Música da gameplay")]
    public AudioClip gameplayMusic;
    
    [Tooltip("Música do fim de jogo")]
    public AudioClip gameEndMusic;
    
    [Header("SFX")]
    [Tooltip("AudioSource para efeitos sonoros")]
    public AudioSource sfxSource;
    
    [Tooltip("Som de clique em botão")]
    public AudioClip buttonClickSound;
    
    [Tooltip("Som de arrastar ficha")]
    public AudioClip cardDragSound;
    
    [Tooltip("Som de soltar ficha")]
    public AudioClip cardDropSound;
    
    [Tooltip("Som de escolha correta")]
    public AudioClip correctChoiceSound;
    
    [Tooltip("Som de escolha incorreta")]
    public AudioClip incorrectChoiceSound;
    
    [Tooltip("Som de popup abrindo")]
    public AudioClip popupOpenSound;
    
    [Tooltip("Som de popup fechando")]
    public AudioClip popupCloseSound;
    
    [Tooltip("Som de transição de dia")]
    public AudioClip dayTransitionSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    [Header("Fade Settings")]
    [Tooltip("Duração do fade de música em segundos")]
    public float musicFadeDuration = 1f;
    
    private Coroutine musicFadeCoroutine;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        // Criar AudioSources se não existirem
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        // Aplicar volumes iniciais
        ApplyVolumes();        
    }
    
    void ApplyVolumes()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        else
        {
            // Fallback se não houver mixer
            if (musicSource != null)
                musicSource.volume = masterVolume * musicVolume;
            
            if (sfxSource != null)
                sfxSource.volume = masterVolume * sfxVolume;
        }
    }
    
    #region Music Control
    
    /// <summary>
    /// Toca música com fade in
    /// </summary>
    public void PlayMusic(AudioClip clip, bool fadeIn = true)
    {
        if (clip == null || musicSource == null)
            return;
        
        // Se já está tocando a mesma música, não fazer nada
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;
        
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);
        
        if (fadeIn)
        {
            musicFadeCoroutine = StartCoroutine(FadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }        
    }
    
    /// <summary>
    /// Para música com fade out
    /// </summary>
    public void StopMusic(bool fadeOut = true)
    {
        if (musicSource == null)
            return;
        
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);
        
        if (fadeOut)
        {
            musicFadeCoroutine = StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }
    }
    
    /// <summary>
    /// Fade entre músicas
    /// </summary>
    IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out da música atual
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            
            while (elapsed < musicFadeDuration / 2)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (musicFadeDuration / 2));
                yield return null;
            }
        }
        
        // Trocar música
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in da nova música
        float targetVolume = masterVolume * musicVolume;
        float elapsed2 = 0f;
        
        while (elapsed2 < musicFadeDuration / 2)
        {
            elapsed2 += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed2 / (musicFadeDuration / 2));
            yield return null;
        }
        
        musicSource.volume = targetVolume;
    }
    
    /// <summary>
    /// Fade out da música
    /// </summary>
    IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        
        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
            yield return null;
        }
        
        musicSource.Stop();
        musicSource.volume = startVolume;
    }
    
    #endregion
    
    #region SFX Control
    
    /// <summary>
    /// Toca efeito sonoro
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null)
            return;
        
        sfxSource.PlayOneShot(clip, volumeScale);
    }
    
    /// <summary>
    /// Toca som de clique em botão
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    
    /// <summary>
    /// Toca som de arrastar ficha
    /// </summary>
    public void PlayCardDrag()
    {
        PlaySFX(cardDragSound, 0.5f);
    }
    
    /// <summary>
    /// Toca som de soltar ficha
    /// </summary>
    public void PlayCardDrop()
    {
        PlaySFX(cardDropSound);
    }
    
    /// <summary>
    /// Toca som de escolha correta
    /// </summary>
    public void PlayCorrectChoice()
    {
        PlaySFX(correctChoiceSound);
    }
    
    /// <summary>
    /// Toca som de escolha incorreta
    /// </summary>
    public void PlayIncorrectChoice()
    {
        PlaySFX(incorrectChoiceSound);
    }
    
    /// <summary>
    /// Toca som de popup abrindo
    /// </summary>
    public void PlayPopupOpen()
    {
        PlaySFX(popupOpenSound);
    }
    
    /// <summary>
    /// Toca som de popup fechando
    /// </summary>
    public void PlayPopupClose()
    {
        PlaySFX(popupCloseSound);
    }
    
    /// <summary>
    /// Toca som de transição de dia
    /// </summary>
    public void PlayDayTransition()
    {
        PlaySFX(dayTransitionSound);
    }
    
    #endregion
    
    #region Volume Control
    
    /// <summary>
    /// Define volume master
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }
    
    /// <summary>
    /// Define volume da música
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }
    
    /// <summary>
    /// Define volume dos efeitos sonoros
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }
    
    #endregion
    
    #region Scene Music
    
    /// <summary>
    /// Toca música do menu
    /// </summary>
    public void PlayMenuMusic()
    {
        if (menuMusic != null)
            PlayMusic(menuMusic);
    }
    
    /// <summary>
    /// Toca música da gameplay
    /// </summary>
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null)
            PlayMusic(gameplayMusic);
    }
    
    /// <summary>
    /// Toca música do fim de jogo
    /// </summary>
    public void PlayGameEndMusic()
    {
        if (gameEndMusic != null)
            PlayMusic(gameEndMusic);
    }
    
    #endregion
}
