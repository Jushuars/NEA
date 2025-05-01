using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public int damage;

    [Header("Reference")]
    private Player player;
    private void Start()
    {
        Invoke(nameof(DestroyObject), 20);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<Player>();
            player.TakeDamage(damage);
            Invoke(nameof(DestroyObject), 0.2f);
        }

        else if (other.tag == "Map")
            DestroyObject();
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
