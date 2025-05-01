using UnityEngine;

public class PlayerProjectileStun : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public float stunDuration;

    [Header("Reference")]
    public MeleeStunned MS;
    public RangedStunned RS;
    private void Start()
    {
        Invoke(nameof(DestroyObject), 20);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee_Enemy")
        {
            Debug.Log("Projectile Hit");
            MS = other.GetComponent<MeleeStunned>();
            MS.Parried(stunDuration);
            Invoke(nameof(DestroyObject), 0.2f);
            transform.position = transform.position;
        }
        if (other.tag == "Ranged_Enemy")
        {
            Debug.Log("Projectile Hit");
            RS = other.GetComponent<RangedStunned>();
            RS.Stunned(stunDuration);
            Invoke(nameof(DestroyObject), 0.2f);
            transform.position = transform.position;
        }

        else if (other.tag == "Map")
            DestroyObject();
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
