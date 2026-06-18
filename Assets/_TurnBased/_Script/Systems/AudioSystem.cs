using UnityEditor.Timeline;
using UnityEngine;

public class AudioSystem : Singleton<AudioSystem> 
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1)
    {
        _soundSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1)
    {
        _soundSource.PlayOneShot(clip, vol);
    }
}
