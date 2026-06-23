using UnityEngine;

public class BoostVFXController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject lightVFXPrefab;
    
    [Header("Boost Audio")]
    public AudioClip boostLv1Clip;
    public AudioClip boostLv2Clip;
    public AudioClip boostLv3Clip;
    
    private AudioSource audioSource;
    private GameObject currentVFX;

    private void Awake()
    {
        // Menambahkan AudioSource secara otomatis agar tidak perlu setting manual di inspector
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBoostEffect(int level, Vector3 position)
    {
        StopEffect(); // Bersihkan efek sebelumnya

        if (lightVFXPrefab == null) return;

        currentVFX = Instantiate(lightVFXPrefab, position, Quaternion.identity);
        var light = currentVFX.GetComponent<Light>();
        var fade = currentVFX.GetComponent<LightningFade>();

        if (light != null && fade != null)
        {
            fade.flag = true; // Mengaktifkan logika fade
            ConfigureLevel(level, light, fade);
        }

        PlayBoostAudio(level);
    }

    private void ConfigureLevel(int level, Light light, LightningFade fade)
    {
        switch (level)
        {
            case 1:
                fade.minIntensity = 50f;
                light.color = new Color(209f/255f, 39f/255f, 81f/255f); // Merah
                light.intensity = 1000f;
                break;
            case 2:
                currentVFX.transform.position += new Vector3(0f, 2.5f, 0f);
                fade.minIntensity = 150f;
                light.color = new Color(33f/255f, 95f/255f, 255f/255f); // Biru
                light.intensity = 1300f;
                break;
            case 3:
                currentVFX.transform.position += new Vector3(0f, 3.8f, 0f);
                fade.minIntensity = 300f;
                light.color = new Color(211f/255f, 236f/255f, 68f/255f); // Kuning
                light.intensity = 1600f;
                break;
        }
    }

    private void PlayBoostAudio(int level)
    {
        AudioClip clip = null;
        if (level == 1) clip = boostLv1Clip;
        else if (level == 2) clip = boostLv2Clip;
        else if (level == 3) clip = boostLv3Clip;

        if (clip != null) audioSource.PlayOneShot(clip);
    }

    public void StopEffect()
    {
        if (currentVFX != null) Destroy(currentVFX);
    }
}