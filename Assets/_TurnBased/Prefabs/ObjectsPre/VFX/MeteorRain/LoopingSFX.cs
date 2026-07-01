using System.Collections;
using UnityEngine;

// Tempel di prefab VFX (misal meteor/fire) yang butuh suara loop selama VFX-nya hidup.
// Otomatis play di Awake, otomatis berhenti pas GameObject di-Destroy().
[RequireComponent(typeof(AudioSource))]
public class LoopingSFX : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip loopClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Header("Mode")]
    [Tooltip("OFF = AudioSource.loop biasa (paling smooth, tapi polanya kedengeran persis berulang).\n" +
             "ON  = di-retrigger manual tiap sekian detik dengan pitch random, biar gak kedengeran robotic.")]
    [SerializeField] private bool usePitchVariation = false;

    [Header("Pitch Variation Settings (kalau usePitchVariation ON)")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [Tooltip("Overlap dikit biar gak ada jeda hening pas retrigger. 0.9 = mulai ulang di 90% durasi clip.")]
    [Range(0.5f, 1f)]
    [SerializeField] private float retriggerAtFraction = 0.9f;

    private AudioSource _source;
    private Coroutine _loopRoutine;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.volume = volume;

        if (loopClip == null) return;

        if (usePitchVariation)
        {
            _loopRoutine = StartCoroutine(LoopWithVariation());
        }
        else
        {
            _source.clip = loopClip;
            _source.loop = true;
            _source.Play();
        }
    }

    private IEnumerator LoopWithVariation()
    {
        while (true)
        {
            _source.pitch = Random.Range(pitchRange.x, pitchRange.y);
            _source.PlayOneShot(loopClip);
            yield return new WaitForSeconds(loopClip.length * retriggerAtFraction);
        }
    }

    private void OnDestroy()
    {
        if (_loopRoutine != null) StopCoroutine(_loopRoutine);
        if (_source != null) _source.Stop();
    }
}