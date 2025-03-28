using System.Collections;
using UnityEngine;

public class MusicManager : Singlton<MusicManager>
{
    [Header("References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _gameplayMusic;

    [Header("Fade settings")]
    [SerializeField] private float _fadeTime = 0.5f;

    public void PlayGameplayMusic() => _playMusic(_gameplayMusic);

    private void _playMusic(AudioClip clip)
    {
        if (_audioSource.clip == null)
        {
            _playClip(clip);

            return;
        }

        if (_audioSource.clip != clip)
        {
            StartCoroutine(_fadeMusic(clip));
        }
    }

    private IEnumerator _fadeMusic(AudioClip newClip)
    {
        for (float t = 0; t < _fadeTime; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(1, 0, t / _fadeTime);
            yield return null;
        }
        
        _audioSource.clip = newClip;
        _audioSource.Play();

        for (float t = 0; t < _fadeTime; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(0, 1, t / _fadeTime);
            yield return null;
        }

        _audioSource.volume = 1f;
    }

    private void _playClip(AudioClip newClip)
    {
        _audioSource.volume = 1f;
        _audioSource.clip = newClip;
        _audioSource.Play();
    }
}