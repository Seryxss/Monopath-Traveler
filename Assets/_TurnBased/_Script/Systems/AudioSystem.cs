using UnityEditor.Timeline;
using UnityEngine;

public class AudioSystem : Singleton<AudioSystem> 
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;

    // 1. Fungsi untuk BGM
    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        _soundSource.PlayOneShot(clip, 1f);
    }
    
    public void PlaySound(AudioClip clip, float vol = 1f)
    {
        if (clip == null) return;
        
        _soundSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        
        _soundSource.PlayOneShot(clip, vol);
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos, float vol = 1f)
    {
        if (clip == null) return;
        _soundSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }
}
