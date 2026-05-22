using UnityEngine;
using UnityEngine.UI;

public class SceneAudioController : MonoBehaviour
{
    [Header("Música de Fundo")]
    [Tooltip("Música que toca nesta cena (deixe vazio para não tocar música)")]
    public AudioClip backgroundMusic;
    
    [Range(0f, 1f)]
    [Tooltip("Volume da música (0 = mudo, 1 = máximo)")]
    public float musicVolume = 0.7f;
    
    [Tooltip("Música em loop?")]
    public bool loopMusic = true;
    
    [Header("Sons de Botão")]
    [Tooltip("Som que toca quando clica em qualquer botão")]
    public AudioClip buttonClickSound;
    
    [Range(0f, 1f)]
    [Tooltip("Volume dos botões")]
    public float buttonVolume = 1f;
    
    [Header("Efeitos Sonoros da Cena")]
    [Tooltip("Som quando arrasta ficha")]
    public AudioClip cardDragSound;
    
    [Tooltip("Som quando solta ficha")]
    public AudioClip cardDropSound;
    
    [Tooltip("Som de escolha correta")]
    public AudioClip correctSound;
    
    [Tooltip("Som de escolha incorreta")]
    public AudioClip incorrectSound;
    
    [Tooltip("Som de popup abrindo")]
    public AudioClip popupOpenSound;
    
    [Tooltip("Som de popup fechando")]
    public AudioClip popupCloseSound;
    
    [Tooltip("Som de transição de dia (toca ao carregar DaySummary)")]
    public AudioClip dayTransitionSound;
    
    [Range(0f, 1f)]
    [Tooltip("Volume dos efeitos sonoros")]
    public float sfxVolume = 1f;
    
    [Header("Configuração Automática")]
    [Tooltip("Adicionar som automaticamente em todos os botões da cena?")]
    public bool autoAddButtonSounds = true;
    
    // AudioSources
    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    void Awake()
    {
        SetupAudioSources();
    }
    
    void Start()
    {
        PlayBackgroundMusic();
        
        if (autoAddButtonSounds)
        {
            AddSoundToAllButtons();
        }
    }
    
    void SetupAudioSources()
    {
        // Criar AudioSource para música
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = loopMusic;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        
        // Criar AudioSource para SFX
        GameObject sfxObj = new GameObject("SFXSource");
        sfxObj.transform.SetParent(transform);
        sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }
    
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    void AddSoundToAllButtons()
    {
        // Encontrar todos os botões na cena
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            // Adicionar listener para tocar som
            button.onClick.AddListener(() => PlayButtonSound());
        }
    }
    
    // Métodos públicos para tocar sons
    
    public void PlayButtonSound()
    {
        PlaySound(buttonClickSound, buttonVolume);
    }
    
    public void PlayCardDrag()
    {
        PlaySound(cardDragSound, sfxVolume * 0.5f);
    }
    
    public void PlayCardDrop()
    {
        PlaySound(cardDropSound, sfxVolume);
    }
    
    public void PlayCorrect()
    {
        PlaySound(correctSound, sfxVolume);
    }
    
    public void PlayIncorrect()
    {
        PlaySound(incorrectSound, sfxVolume);
    }
    
    public void PlayPopupOpen()
    {
        PlaySound(popupOpenSound, sfxVolume);
    }
    
    public void PlayPopupClose()
    {
        PlaySound(popupCloseSound, sfxVolume);
    }
    
    public void PlayDayTransition()
    {
        PlaySound(dayTransitionSound, sfxVolume);
    }
    
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    // Controle de volume em tempo real
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
