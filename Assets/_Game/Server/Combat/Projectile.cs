using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Vector2 puntoArrivo;
    private bool inVolo = false;
    private Rigidbody2D rb;
    private bool despawned = false; // guard anti doppia Destroy

    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float stunTime;


    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float speed;

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

        // Colpisce un bersaglio
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            CharacterEntity target = other.GetComponentInParent<CharacterEntity>();

            if (target != null)
            {
                target.TakeDamage(damage);
                
                // KNOCKBACK
                Vector2 direzioneVolo = (puntoArrivo - rb.position).normalized;
                target.Knockback(direzioneVolo, knockbackForce, knockbackTime, stunTime);

                inVolo = false;
                DespawnProjectile();
            }

            return;
        }
    }

    [Server]
    private void DespawnProjectile()
    {
        if (despawned) return;

        despawned = true;
        NetworkServer.Destroy(gameObject);
    }
}