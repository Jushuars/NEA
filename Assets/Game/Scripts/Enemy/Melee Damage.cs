using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public int damage;

    [Header("Reference")]
    private Player player;
    public Parry pr;
    public MeleeStunned MS;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player_Sword")
            pr = other.GetComponent<Parry>();

        if(other.tag == "Player" && !pr.isParrying)
        { 
            player = other.GetComponent<Player>();
            player.TakeDamage(damage);
        }
    }
    public void CallMSParry(float stunDuration) // Calls the stun function for the enemy
    {
        MS.Parried(stunDuration);
    }
}
