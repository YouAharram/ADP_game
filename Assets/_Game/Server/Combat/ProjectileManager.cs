using Mirror;
using UnityEngine;

public class ProjectileManager : NetworkBehaviour
{
    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab;

    [Server]
public void ShootProjectile(Vector2 spawnPoint, Vector2 bersaglioPosizione, int damage)
{
    Vector2 direzione = (bersaglioPosizione - spawnPoint).normalized;

    bool flip = direzione.x < 0f;
    float angle = Mathf.Atan2(direzione.y, direzione.x) * Mathf.Rad2Deg;
    if (flip) angle -= 180f;

    GameObject projectileObject = Instantiate(projectilePrefab, spawnPoint, Quaternion.Euler(0, 0, angle));

    Projectile projectile = projectileObject.GetComponent<Projectile>();
    if (projectile != null)
    {
        projectile.Damage = damage;
        projectile.SetFlip(flip);
    }

    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.position = spawnPoint;
    }

    projectile.Shoot(bersaglioPosizione);

    NetworkServer.Spawn(projectileObject);
}
}
