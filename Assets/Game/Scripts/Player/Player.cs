using UnityEngine;

public class Player : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public int baseHealth = 100;
    public float health;
    public bool isDead = false;
    public bool disableMovement = false;
    public bool disableWeapon = false;
    public float iFramesDuration;
    public bool iFramesActive;
    public bool revive;
    private int maxHealth;
    private float iFramesTimeLeft;
    

    [Header("Reference")]
    // Movement
    public Movement pm;
    public Grappling gm;
    public Sliding sm;
    public Dashing dm;
    public WallRunning wm;
    // Weapon
    public Parry pr;
    public WeaponController wp;
    // UI
    public Health_Bar HB;
    public ComboScore cs;

    private void Start() // Runs on start
    {
        isDead = false;
        health = baseHealth;
        maxHealth = baseHealth;
        iFramesActive = false;
        iFramesTimeLeft = iFramesDuration;

        HB.SetMaxHealth(maxHealth);
    }
    private void Update()
    {
        // Will revive the player back to full health
        if (revive)
        {
            health = maxHealth;
            revive = false;
        }
        // Disables all player movement if dead
        if (isDead)
        {
            pm.enabled = false;
            gm.enabled = false;
            sm.enabled = false;
            dm.enabled = false;
            wm.enabled = false;
            wp.enabled = false;
            pr.enabled = false;
            pm.rb.linearVelocity = new Vector3(0, 0, 0);
        }
        if (!isDead)
        {
            if (!disableMovement)
            {
                pm.enabled = true;
                gm.enabled = true;
                sm.enabled = true;
                dm.enabled = true;
                wm.enabled = true;
            }
            if(!disableWeapon)
            {
                wp.enabled = true;
                pr.enabled = true;
            }
        }
        // Disables player movement if true
        if (!disableMovement)
        {
            pm.enabled = true;
            gm.enabled = true;
            sm.enabled = true;
            dm.enabled = true;
            wm.enabled = true;
        }
        if (disableMovement)
        {
            pm.enabled = false;
            gm.enabled = false;
            sm.enabled = false;
            dm.enabled = false;
            wm.enabled = false;
            pm.rb.linearVelocity = new Vector3(0,0,0);
        }
        // Disables player use of weapons if true
        if (disableWeapon)
        {
            pr.enabled = false;
            wp.enabled= false;
        }
        if (!disableWeapon)
        {
            pr.enabled = true;
            wp.enabled = true;
        }
        // Makes sure health doesnt go above max health
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        // Checks to make sure if the player should be dead or not
        if (health <= 0)
            isDead = true;
        else isDead = false;
        // Enables & counts down iFrames
        if (iFramesActive)
            iFramesTimeLeft -= Time.deltaTime;
        if (iFramesTimeLeft <= 0)
        {
            iFramesActive = false;
            iFramesTimeLeft = iFramesDuration;
        }
    }
    public void TakeDamage(float damage)
    {
        if (!iFramesActive)
        {
            health -= damage;
            HB.SetHealth(health);
            Debug.Log("Player health:" + health);
            iFramesActive = true;
            cs.score -= 50;
            cs.CombatUpdate();
        }
    }
}
