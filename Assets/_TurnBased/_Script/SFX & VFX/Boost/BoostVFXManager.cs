using System.Collections.Generic;
using UnityEngine;

public class BoostVFXManager : MonoBehaviour
{
    public static BoostVFXManager Instance { get; private set; }

    [Header("VFX Prefab")]
    [SerializeField] private GameObject boostLightVFXPrefab;

    [Header("UI & System SFX (One-Shot)")]
    [SerializeField] private AudioClip castClip;       
    [SerializeField] private AudioClip countUpClip;    
    [SerializeField] private AudioClip countFullClip;  
    [SerializeField] private AudioClip cancelClip;     

    [Header("Burst SFX (One-Shot)")]
    [SerializeField] private AudioClip[] burstClips;   

    [Header("Aura SFX (Looping)")]
    [SerializeField] private AudioClip[] loopClips;    

    // Audio untuk UI Tick
    private AudioSource sfxSource;   
    private int maxBoostLevel = 3;

    private Dictionary<HeroCharBase, GameObject> activeVFX = new Dictionary<HeroCharBase, GameObject>();
    private Dictionary<HeroCharBase, AudioSource> activeLoops = new Dictionary<HeroCharBase, AudioSource>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    public void PlayBoostEffect(HeroCharBase hero, int currentLevel, int prevLevel)
    {
        if (hero == null) return;

        if (currentLevel == 0)
        {
            sfxSource.PlayOneShot(cancelClip); 
            StopHeroEffect(hero); 
            return;
        }

        if (currentLevel > prevLevel)
        {
            if (currentLevel == 1) sfxSource.PlayOneShot(castClip);
            else if (currentLevel == maxBoostLevel) sfxSource.PlayOneShot(countFullClip);
            else sfxSource.PlayOneShot(countUpClip);
        }

        GameObject lightVFX;
        
        if (!activeVFX.ContainsKey(hero))
        {
            Vector3 spawnPos = hero.transform.position + new Vector3(0f, 0.4f, 0f); 

            lightVFX = Instantiate(boostLightVFXPrefab, spawnPos, Quaternion.identity);
            lightVFX.transform.SetParent(hero.transform);
            
            activeVFX[hero] = lightVFX;

            AudioSource newLoop = hero.gameObject.AddComponent<AudioSource>();
            newLoop.loop = true;
            newLoop.playOnAwake = false;
            activeLoops[hero] = newLoop;
        }
        else
        {
            lightVFX = activeVFX[hero];
        }
        
        Light vfxLight = lightVFX.GetComponent<Light>();
        LightningFade fadeScript = lightVFX.GetComponent<LightningFade>();
        AudioSource loopSource = activeLoops[hero];

        if (vfxLight != null && fadeScript != null)
        {
            float minIntensity = 0f;

            if (currentLevel == 1)
            {
                // LEVEL 1: MERAH (Keseimbangan Normal)
                minIntensity = 50f;
                vfxLight.color = new Color(1f, 0.2f, 0.3f); 
                vfxLight.intensity = 1000f;
            }
            else if (currentLevel == 2)
            {
                // LEVEL 2: KUNING (Tenaga diturunkan agar tidak buta)
                minIntensity = 50f;
                vfxLight.color = new Color(1f, 0.8f, 0.2f); // Kuning keemasan
                vfxLight.intensity = 600f; // Turunkan jauh dari 1000f!
            }
            else if (currentLevel == 3)
            {
                // LEVEL 3: BIRU (Tenaga dinaikkan & warna dicerahkan)
                minIntensity = 100f; // Biarkan minimalnya sedikit lebih tinggi
                vfxLight.color = new Color(0.3f, 0.7f, 1f); // Biru muda / Cyan
                vfxLight.intensity = 3000; // Pompa tenaganya dua kali lipat lebih!
            }

            fadeScript.SetMinimumIntensity(minIntensity);


            if (burstClips.Length >= currentLevel && burstClips[currentLevel - 1] != null && hero.charAudioSource != null)
            {
                hero.charAudioSource.PlayOneShot(burstClips[currentLevel - 1]);
            }

            if (loopClips.Length >= currentLevel && loopClips[currentLevel - 1] != null)
            {
                loopSource.clip = loopClips[currentLevel - 1];
                if (!loopSource.isPlaying) loopSource.Play(); 
            }
        }
    }

    public void StopHeroEffect(HeroCharBase hero)
    {
        if (hero == null) return;

        if (activeLoops.ContainsKey(hero))
        {
            activeLoops[hero].Stop();
            Destroy(activeLoops[hero]); 
            activeLoops.Remove(hero);
        }

        if (activeVFX.ContainsKey(hero))
        {
            Destroy(activeVFX[hero]); 
            activeVFX.Remove(hero);
        }
    }
}
