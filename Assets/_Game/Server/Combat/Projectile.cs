using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private enum ImpactType
    {
        Ground,
        Obstacle,
        Enemy
    }

    [Header("Movimento")]
    [SerializeField] private float speed;

    [Header("Collisioni")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Aspetto")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite buriedSprite;     // usata su ostacolo e su nemico
    [SerializeField] private Sprite buriedDownSprite; // usata quando arriva a terra senza colpire nulla

    [Header("Comportamento")]
    [Tooltip("Se true, il proiettile sparisce subito dopo l'impatto invece di restare visibile per qualche secondo")]
    [SerializeField] private bool instantDespawn = false;

    private const float LifetimeAfterImpact = 3f;
    private const float ArrivalThreshold = 0.1f;
    private const float KnockbackForce = 3f;
    private const float KnockbackTime = 0.3f;
    private const float StunTime = 0.2f;

    [SyncVar] private bool isFlipped;

    private Vector2 puntoArrivo;
    private bool inVolo;
    private Rigidbody2D rb;
    private bool despawned;

    public int Damage { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ApplyFlip();
    }

    public void SetFlip(bool flipped)
    {
        isFlipped = flipped;
    }

    private void ApplyFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = isFlipped ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    [Server]
    public void Shoot(Vector2 arrivo)
    {
        puntoArrivo = arrivo;
        inVolo = true;
    }

    private void FixedUpdate()
    {
        if (!inVolo) return;

        Vector2 direction = (puntoArrivo - rb.position).normalized;
        Vector2 nextPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        if (!isServer) return;

        bool arrivato = Vector2.Distance(rb.position, puntoArrivo) < ArrivalThreshold;
        if (arrivato)
        {
            ResolveImpact(ImpactType.Ground, null, 0f, Vector2.zero);
        }
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!inVolo) return;

        int otherLayerBit = 1 << other.gameObject.layer;

        if ((otherLayerBit & targetLayer) != 0)
        {
            HandleEnemyHit(other);
        }
        else if ((otherLayerBit & obstacleLayer) != 0)
        {
            ResolveImpact(ImpactType.Obstacle, null, 0f, Vector2.zero);
        }
    }

    [Server]
    private void HandleEnemyHit(Collider2D other)
    {
        CharacterEntity target = other.GetComponentInParent<CharacterEntity>();
        if (target == null) return;

        target.TakeDamage(Damage);

        Vector2 direzioneVolo = (puntoArrivo - rb.position).normalized;
        target.Knockback(direzioneVolo, KnockbackForce, KnockbackTime, StunTime);

        NetworkIdentity targetIdentity = target.GetComponent<NetworkIdentity>();

        // Posizione esatta d'impatto rilevata dal server, e offset rispetto al
        // target cosi' la freccia lo segue se si muove.
        Vector2 hitPosition = rb.position;
        Vector2 localOffset = hitPosition - (Vector2)target.transform.position;

        ResolveImpact(ImpactType.Enemy, targetIdentity, target.CurrentHealth, localOffset);
    }

   
    [Server]
    private void ResolveImpact(ImpactType type, NetworkIdentity targetIdentity, float targetHealth, Vector2 localOffset)
    {
        if (!inVolo) return; // gia' gestito da un altro trigger nello stesso frame
        inVolo = false;

        RpcOnImpact(type, targetIdentity, targetHealth, localOffset);

        if (ShouldDespawnInstantly(type))
        {
            DespawnProjectile();
        }
        else
        {
            Invoke(nameof(DespawnProjectile), LifetimeAfterImpact);
        }
    }

    // L'acqua fa sempre despawnare subito, indipendentemente dal flag
    // instantDespawn: non esiste uno sprite "conficcato in acqua".
    private bool ShouldDespawnInstantly(ImpactType type)
    {
        return instantDespawn;
    }

    [ClientRpc]
    private void RpcOnImpact(ImpactType type, NetworkIdentity targetIdentity, float targetHealth, Vector2 localOffset)
    {
        inVolo = false;

        // Quando la freccia non è più in volo, viene renderizzata sopra.
        SetBuriedSortingOrder();

        if (ShouldDespawnInstantly(type))
        {
            HideAndFreeze();
            return;
        }

        switch (type)
        {
            case ImpactType.Enemy:
                AttachToEnemy(targetIdentity, targetHealth, localOffset);
                break;

            case ImpactType.Obstacle:
                BuryInPlace(buriedSprite);
                break;

            case ImpactType.Ground:
                BuryInPlace(buriedDownSprite);
                break;
        }
    }
    private void AttachToEnemy(NetworkIdentity targetIdentity, float targetHealth, Vector2 localOffset)
    {
        // Il target puo' essere gia' stato distrutto/non piu' osservato da questo
        // client quando la RPC arriva (es. e' morto per un altro colpo nello stesso
        // istante): in quel caso Mirror lo deserializza a null, quindi va sempre
        // controllato prima di usarlo.
        if (targetIdentity == null || targetHealth <= 0f)
        {
            HideAndFreeze();
            return;
        }

        DisableNetworkTransform();
        DisablePhysics();

        transform.SetParent(targetIdentity.transform);
        // Forziamo la posizione ESATTA rilevata dal server, ignorando dove si
        // trovava la freccia sul client a causa della latenza.
        transform.localPosition = localOffset;

        if (sr != null && buriedSprite != null)
        {
            sr.sprite = buriedSprite;
        }

        SetColliderEnabled(false);
    }

    private void BuryInPlace(Sprite sprite)
    {
        if (sr != null && sprite != null)
        {
            sr.sprite = sprite;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        SetColliderEnabled(false);
    }

    private void HideAndFreeze()
    {
        if (sr != null) sr.enabled = false;
        DisablePhysics();
        SetColliderEnabled(false);
    }

    private void DisablePhysics()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    private void SetColliderEnabled(bool value)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = value;
    }

    private void DisableNetworkTransform()
    {
        // troiaio
        foreach (var behaviour in GetComponents<Behaviour>())
        {
            if (behaviour.GetType().Name.Contains("NetworkTransform"))
            {
                behaviour.enabled = false;
            }
        }
    }

    [Server]
    private void DespawnProjectile()
    {
        if (despawned) return;
        despawned = true;
        NetworkServer.Destroy(gameObject);
    }

    [SerializeField] private int buriedSortingOrder = 10;

    private void SetBuriedSortingOrder()
    {
        if (sr != null)
        {
            sr.sortingOrder = buriedSortingOrder;
        }
    }
}