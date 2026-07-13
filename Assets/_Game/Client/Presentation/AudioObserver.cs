using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObserver : MonoBehaviour
{
    private AudioSource audioSource;
    
    [Header("Clip Audio")]
    public AudioClip attackClip;
    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip groanAudio;
    private CharacterEntity characterEntity;

    [Header("Impostazioni")]
    [Range(0.2f, 1f)] public float pitchVariation = 0.1f; 

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    private int soundInterval; 
    private float nextCheckTime = 0f;
    
    private void Start()
    {
        characterEntity = GetComponent<CharacterEntity>();
        if (characterEntity != null)
        {
            characterEntity.OnAttackingClient += PlayAttack;
            characterEntity.OnDamageClient += PlayHit;
            characterEntity.OnDieClient  += PlayDeath;
        }
        
        soundInterval = Random.Range(0, 10);
    }
    
    private void Update()
    {
        // ogni tanto fa un verso
        if (Time.time < nextCheckTime) return;

        nextCheckTime = Time.time + soundInterval;
        soundInterval = Random.Range(0, 10);
        Play(groanAudio);
    }
    
    private void PlayAttack() => Play(attackClip);
    private void PlayHit() => Play(hitClip);
    private void PlayDeath() => Play(deathClip);

    private void Play(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip);
    }

    private void OnDestroy()
    {
        characterEntity.OnAttackingClient -= PlayAttack;
        characterEntity.OnDamageClient -= PlayHit;
        characterEntity.OnDieClient -= PlayDeath;
    }
}