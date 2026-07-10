using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorGradient;
    
    [Tooltip("Trascina qui l'Entity se vuoi forzare il collegamento, altrimenti lo cerca da solo")]
    [SerializeField] private Entity targetEntity;

    private bool subscribed;

    void Start()
    {
        // Ricerca automatica solo se non è già stato assegnato via Init() o Inspector
        if (targetEntity == null)
        {
            targetEntity = GetComponentInParent<Entity>();
        }

        if (fillImage == null)
        {
            Debug.LogError($"[HealthBar] ERRORE: 'fillImage' NON è collegata nell'Inspector di {gameObject.name}!");
        }

        if (targetEntity == null)
        {
            Debug.LogError($"[HealthBar] ERRORE: targetEntity NULL su '{gameObject.name}'. " +
                            $"Parent: {(transform.parent != null ? transform.parent.name : "nessuno")}. " +
                            $"Se questa HealthBar viene istanziata dinamicamente (es. nameplate), " +
                            $"chiama HealthBar.Init(entity) subito dopo Instantiate invece di affidarti a Start().");
            return;
        }

        Subscribe();
        UpdateHealthBar();
    }

    /// <summary>
    /// Da chiamare esplicitamente quando la HealthBar viene istanziata dinamicamente
    /// (es. spawnata come nameplate DOPO che l'Entity esiste già), invece di
    /// affidarsi a GetComponentInParent in Start().
    /// </summary>
    public void Init(Entity entity)
    {
        if (entity == null)
        {
            Debug.LogError($"[HealthBar] Init chiamato con entity NULL su '{gameObject.name}'!");
            return;
        }

        Unsubscribe();
        targetEntity = entity;
        Subscribe();
        UpdateHealthBar();
    }

    private void Subscribe()
    {
        if (targetEntity == null || subscribed) return;
        targetEntity.OnDamageClient += UpdateHealthBar;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (targetEntity == null || !subscribed) return;
        targetEntity.OnDamageClient -= UpdateHealthBar;
        subscribed = false;
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    public void UpdateHealthBar()
    {
        if (targetEntity == null)
        {
            Debug.LogWarning($"[HealthBar] UpdateHealthBar chiamato ma targetEntity è NULL su '{gameObject.name}'.");
            return;
        }

        // Se MaxHealth è 0, siamo in una fase di transizione di rete. Non calcoliamo nulla.
        if (targetEntity.MaxHealth <= 0)
        {
            if (fillImage != null) fillImage.fillAmount = 0;
            return;
        }

        float fillValue = (float)targetEntity.CurrentHealth / targetEntity.MaxHealth;

        Debug.Log($"[HealthBar] '{targetEntity.name}': hp={targetEntity.CurrentHealth}/{targetEntity.MaxHealth} -> fill={fillValue}");

        if (fillImage != null)
        {
            fillImage.fillAmount = fillValue;
            if (colorGradient != null) fillImage.color = colorGradient.Evaluate(fillValue);
        }
    }
}