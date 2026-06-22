using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStats : MonoBehaviour
{  

    [SerializeField] private int maxHealth;
    [SerializeField] private int avarageAttack;
    private int currentHealth;
    private bool isCollidingWithEnemy = false;
    private CharacterStats enemyStats;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        CheckAttacking();
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log(gameObject.name + " current health: " + currentHealth);

    }

    public void Attack()
    {
        if (enemyStats != null)
        {
            int damage = CalculateAttack();
            enemyStats.Damage(damage);

            Debug.Log(gameObject.name + " attacks with damage " + damage);
        }
        
    }

    private void CheckAttacking()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isCollidingWithEnemy && enemyStats != null)
            {
                Attack();
            }
            else
            {
                isCollidingWithEnemy = false;
                enemyStats = null;
            }
           
        }
    }

    private int CalculateAttack()
    {
        int offset = avarageAttack*30/100;
        return UnityEngine.Random.Range(avarageAttack - offset, avarageAttack + offset) + 1;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
           enemyStats = other.gameObject.GetComponent<CharacterStats>();
           isCollidingWithEnemy = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
           enemyStats = null;
           isCollidingWithEnemy = false;
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

    }
}
