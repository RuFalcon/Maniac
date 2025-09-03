using GamePush;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    // --- Синглтон (Singleton) ---
    public static SoundManager instance;

    // --- Источники звука ---
    [SerializeField] private AudioSource _musicSource; // Для фоновой музыки и эмбиенса
    [SerializeField] private AudioSource _sfxSource;   // Для звуковых эффектов

    // --- Клипы для SFX ---
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _chimeSound; // Правильный ответ
    [SerializeField] private AudioClip _errorSound; // Неправильный ответ
    [SerializeField] private AudioClip _sonarSound; // Сообщение маньяка
    [SerializeField] private AudioClip _swooshSound; // Сообщение игрока
    [SerializeField] private AudioClip _typingSound; // Цикличный звук печатания
    [SerializeField] private AudioClip _gameoverSound; // Звук при проигрыше
    [SerializeField] private AudioClip _stampSound; // Звук печати
    [SerializeField] private AudioClip _puzzleClickSound; // Звук печати
    [SerializeField] private AudioClip _thumbtackSound; // Звук пришпиливания карточки
    [SerializeField] private AudioClip _backgroundMusic;


    void Awake()
    {
        // Классическая реализация синглтона
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await GP_Init.Ready;
        GameManager.soundOff = GP_Player.GetBool("maniac_soundoff");
        if (!GameManager.soundOff && !GP_Ads.IsPreloaderPlaying()) PlayMusic();
    }

    // --- Публичные методы для SFX ---
    public void PlayButtonClickSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_buttonClick);
    }

    public void PlayChimeSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_chimeSound);
    }

    public void PlayErrorSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_errorSound);
    }

    public void PlaySonarSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_sonarSound);
    }

    public void PlaySwooshSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_swooshSound);
    }

    public void PlayGameOverSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_gameoverSound);
    }

    public void PlayStampSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_stampSound);
    }

    public void PlayPuzzleClickSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_puzzleClickSound);
    }

    public void PlayThumbtackSound()
    {
        if (!GameManager.soundOff) _sfxSource.PlayOneShot(_thumbtackSound);
    }

    // --- Методы для цикличного звука печатания ---
    public void StartTypingSound()
    {
        if (_typingSound == null) return;
        _sfxSource.clip = _typingSound;
        _sfxSource.loop = true;
        if (!GameManager.soundOff) _sfxSource.Play();
    }

    public void StopTypingSound()
    {
        if (_sfxSource.clip == _typingSound)
        {
            _sfxSource.Stop();
            _sfxSource.loop = false;
        }
    }

    // --- Методы для фоновой музыки/эмбиенса ---
    public void PlayMusic()
    {
        _musicSource.clip = _backgroundMusic;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Pause();
    }
}