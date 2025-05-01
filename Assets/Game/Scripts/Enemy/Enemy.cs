using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public int health;
    public float despawnTime;
    private bool dead;

    private void Start()
    {
        dead = false;
    }
    private void Update()
    {
        if(health <= 0) // Checks if the enemy should be dead
        {
            Invoke(nameof(Death), despawnTime);
            dead = true;
        }
    }

    public void TakeDamage(int damage) // Applies the calculation for the enemy health
    {
        health -= damage;
        Debug.Log(health);
    }
    private void Death() // Destroys the object after death
    {
        Destroy(gameObject);
    }
}
