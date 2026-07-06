// ============================================================
// MODIFICHE:
// 1. Sottoscrizione a OnDamageClient (rinominato) invece di
//    OnDamage: nessun'altra modifica strutturale necessaria,
//    questa classe era già corretta nell'approccio (SyncVar
//    hook), solo allineata al nuovo naming.
// 2. Aggiunta sottoscrizione a OnDieClient per gestire lo stato
//    "morto" nella barra vita (es. nasconderla o forzarla a 0),
//    così è consistente anche se muori per un motivo diverso da
//    un singolo danno (es. debug/comando GM).
// 3. Rimosso il riferimento diretto a Mirror (using Mirror non
//    serviva più: la classe non tocca la rete, giustamente).
// ============================================================
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorGradient;

    private CharacterEntity characterEntity;

    void Start()
    {
        characterEntity = GetComponentInParent<CharacterEntity>();

        if (characterEntity != null)
        {
            characterEntity.OnDamageClient += UpdateHealthBar;
            characterEntity.OnDieClient += HandleDeath;

            UpdateHealthBar();
        }
    }

    void OnDestroy()
    {
        if (characterEntity != null)
        {
            characterEntity.OnDamageClient -= UpdateHealthBar;
            characterEntity.OnDieClient -= HandleDeath;
        }
    }

    public void UpdateHealthBar()
    {
        float fillValue = (float)characterEntity.CurrentHealth / characterEntity.MaxHealth;
        fillImage.fillAmount = fillValue;
        fillImage.color = colorGradient.Evaluate(fillValue);
    }

    private void HandleDeath()
    {
        fillImage.fillAmount = 0f;
    }
}