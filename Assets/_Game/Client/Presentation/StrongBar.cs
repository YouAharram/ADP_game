using TMPro;
using UnityEngine;
using Mirror;
using System.Collections;

public class StrongBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strongText;

    private CharacterEntity targetEntity;
    private bool subscribed;

    private void Start()
    {
        if (strongText == null)
        {
            Debug.LogError($"[StrongBar] 'strongText' is not assigned on {gameObject.name}.");
            return;
        }

        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        // Aspetta finché Mirror non ha spawnato e assegnato il local player
        while (NetworkClient.localPlayer == null)
        {
            yield return null;
        }

        CharacterEntity entity = NetworkClient.localPlayer.GetComponent<CharacterEntity>();

        if (entity == null)
        {
            Debug.LogError($"[StrongBar] Il local player non ha un componente CharacterEntity.");
            yield break;
        }

        Init(entity);
    }

    public void Init(CharacterEntity entity)
    {
        if (entity == null)
        {
            Debug.LogError($"[StrongBar] Init called with a null CharacterEntity.");
            return;
        }

        Unsubscribe();
        targetEntity = entity;
        Subscribe();
        UpdateStrongBar(targetEntity.Damage);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (targetEntity == null || subscribed)
            return;

        targetEntity.OnDamageChanged += UpdateStrongBar;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (targetEntity == null || !subscribed)
            return;

        targetEntity.OnDamageChanged -= UpdateStrongBar;
        subscribed = false;
    }

    private void UpdateStrongBar(int damage)
    {
        if (strongText == null)
            return;
        strongText.text = damage.ToString();
    }
}