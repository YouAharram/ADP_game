using Mirror;
using UnityEngine;
using System.Linq;

public class Projectile : NetworkBehaviour
{
    private Vector2 puntoArrivo;
    private bool inVolo = false;
    private Rigidbody2D rb;
    private bool despawned = false; 

    private float knockbackForce = 3f;
    private float knockbackTime = 0.3f;
    private float stunTime = 0.2f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float speed;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite buriedSprite;
    
    private int damage;
    public int Damage { get => damage; set => damage = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        bool arrivato = Vector2.Distance(rb.position, puntoArrivo) < 0.1f;

        if (arrivato)
        {
            inVolo = false;
            DespawnProjectile();
        }
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!inVolo) return;

        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            CharacterEntity target = other.GetComponentInParent<CharacterEntity>();

            if (target != null)
            {
                target.TakeDamage(damage);

                Vector2 direzioneVolo = (puntoArrivo - rb.position).normalized;
                target.Knockback(direzioneVolo, knockbackForce, knockbackTime, stunTime);

                inVolo = false;

                NetworkIdentity targetIdentity = target.GetComponent<NetworkIdentity>();
            
                // Posizione esatta d'impatto rilevata dal server
                Vector2 hitPosition = rb.position;
            
                // Offset rispetto al target, cosi la freccia segue il nemico se si muove
                Vector2 localOffset = hitPosition - (Vector2)target.transform.position;

                RpcAttachToEnemy(targetIdentity, localOffset);

                Invoke(nameof(DespawnProjectile), 3f);
            }

            return;
        }

        // Colpisce un ostacolo
        else if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            inVolo = false;
            RpcAttachToObstacle();
            Invoke(nameof(DespawnProjectile), 3f);
        }
    }
    
    [ClientRpc]
    private void RpcAttachToEnemy(NetworkIdentity targetIdentity, Vector2 localOffset)
    {
        inVolo = false;

        DisableNetworkTransform();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (targetIdentity != null)
        {
            transform.SetParent(targetIdentity.transform);
            // Forziamo la posizione ESATTA rilevata dal server, ignorando dove si trovava
            // la freccia sul client a causa della latenza
            transform.localPosition = localOffset;
        }

        if (sr != null && buriedSprite != null)
        {
            sr.sprite = buriedSprite;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    private void DisableNetworkTransform()
    {
        // troiaio per far funzionare
        var behaviours = GetComponents<Behaviour>();
        foreach (var b in behaviours)
        {
            if (b.GetType().Name.Contains("NetworkTransform"))
            {
                b.enabled = false;
            }
        }
    }

    [ClientRpc]
    private void RpcAttachToObstacle()
    {
        inVolo = false;
        
        if (sr != null && buriedSprite != null)
        {
            sr.sprite = buriedSprite;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    [Server]
    private void DespawnProjectile()
    {
        if (despawned) return;

        despawned = true;
        NetworkServer.Destroy(gameObject);
    }
}