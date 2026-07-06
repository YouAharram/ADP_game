using UnityEngine;
using System.Collections;

public class AnimationObserver : MonoBehaviour
{
    [SerializeField] private GameObject deathVFXPrefab;

    private CharacterEntity characterEntity;
    private Animator animator;
    private SpriteRenderer sr;

    private Vector2 lastPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        characterEntity = GetComponent<CharacterEntity>();

        if (characterEntity != null)
        {
            lastPosition = characterEntity.GetPosition();

            characterEntity.OnAttackingClient += AnimationAttack;
            characterEntity.OnDamageClient += AnimationDamage;
            characterEntity.OnDieClient += AnimationDeath;
            characterEntity.OnFlipDirectionClient += FlipDirection;
        }
    }

    private float checkInterval = 0.1f; // Controlla 10 volte al secondo
    private float nextCheckTime = 0f;

    private void Update()
    {
        // Se non è ancora passato abbastanza tempo, salta l'Update
        if (Time.time < nextCheckTime) return;

        // Aggiorna il timer per il prossimo controllo
        nextCheckTime = Time.time + checkInterval;
        // Lettura locale di uno stato già sincronizzato da NetworkTransform
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null || sr == null || characterEntity == null) return;

        Vector2 currentPosition = characterEntity.GetPosition();
        Vector2 delta = currentPosition - lastPosition;

        if (delta.sqrMagnitude < 0.0001f)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        animator.SetFloat("Speed", 1f);
        lastPosition = currentPosition;
    }

    private void FlipDirection()
    {
        Debug.Log("FlipDirection chiamato in AnimationObserver");
        sr.flipX = !sr.flipX;
    }

    private void AnimationAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void AnimationDamage()
    {
        if (sr != null)
        {
            StartCoroutine(FlashRedEffect());
        }
    }

    private void AnimationDeath()
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("deathVFXPrefab è null porca pera!");
        }
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    private IEnumerator FlashRedEffect()
    {
        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (sr != null)
        {
            sr.color = originalColor;
        }
    }

    void OnDestroy()
    {
        if (characterEntity != null)
        {
            characterEntity.OnAttackingClient -= AnimationAttack;
            characterEntity.OnDamageClient -= AnimationDamage;
            characterEntity.OnDieClient -= AnimationDeath;
            characterEntity.OnFlipDirectionClient -= FlipDirection;
        }
    }
}