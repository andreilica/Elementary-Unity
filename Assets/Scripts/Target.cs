using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;
    protected float initialHealth;

    private void Start()
    {
        initialHealth = health;
    }
    public virtual void TakeDamage (float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
