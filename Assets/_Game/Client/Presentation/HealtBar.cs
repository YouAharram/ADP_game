using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorGradient;
    
    private Entity targetEntity;

    private bool subscribed;

    void Start()
    {
        targetEntity = GetComponentInParent<Entity>();
        

        if (fillImage == null)
        {
            Debug.LogError($"[HealthBar] ERRORE: 'fillImage' NON è collegata nell'Inspector di {gameObject.name}!");
        }

        Subscribe();
        UpdateHealthBar(targetEntity.CurrentHealth, targetEntity.MaxHealth);
    }

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
        UpdateHealthBar(targetEntity.CurrentHealth, targetEntity.MaxHealth);
    }
    
    void OnDestroy()
    {
        Unsubscribe();
    }
    private void Subscribe()
    {
        if (targetEntity == null || subscribed) return;
        targetEntity.OnHealthChangedClient += UpdateHealthBar;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (targetEntity == null || !subscribed) return;
        targetEntity.OnHealthChangedClient -= UpdateHealthBar;
        subscribed = false;
    }

    public void UpdateHealthBar(int current, int max)
    {
        if (max <= 0)
        {
            if (fillImage != null) fillImage.fillAmount = 0;
            return;
        }

        float fillValue = (float)current / max;

        if (fillImage != null)
        {
            fillImage.fillAmount = fillValue;
            if (colorGradient != null) fillImage.color = colorGradient.Evaluate(fillValue);
        }
    }
}