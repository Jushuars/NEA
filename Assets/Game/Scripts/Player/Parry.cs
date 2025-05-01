using UnityEngine;

public class Parry : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public float parryDuration;
    public float parryCd;
    public bool canParry;
    public bool isParrying = false;
    public bool resetParry;
    public float parryTimer;
    private bool parried = false;
    public float resetCd;
    private float resetTimer;

    [Header("Reference")]
    public GameObject sword;
    public KeyCode parryKey;
    public collisionDetection cd;
    public WeaponController wp;
    public GameObject friendlyProjectile;

    private void Update()
    {
        // Counts down parry timer
        if (parryTimer > 0)
            parryTimer -= Time.deltaTime;

        else canParry = true;

        // Sets parry cooldown to 0 if reset parry true
        if (resetParry)
        {
            parryTimer = 0;
            resetParry = false;
        }

        if (Input.GetKeyDown(parryKey) && canParry && !wp.isAttacking && !wp.finisherAttempt)
        {
            Parrying();
        }

        //if (isParrying) cd.hitbox.enabled = true; // Turns hitbox on if parrying
        //if (!isParrying) cd.hitbox.enabled = false; // Turns hitbox off if not parrying

        Animator anim = sword.GetComponent<Animator>();

        if (resetTimer > 0 && isParrying)
            resetTimer -= Time.deltaTime;
        if (resetTimer <= 0 && isParrying)
            ResetParry();
    }
    private void Parrying()
    {
        resetParry = false;
        isParrying = true;
        canParry = false;
        parryTimer = parryCd;
        Animator anim = sword.GetComponent<Animator>();
        anim.ResetTrigger("ResetParry"); // Prevent animation break
        anim.SetTrigger("Parry"); // Parry animation
        resetTimer = resetCd;
    }
    public void ResetParry()
    {
        isParrying = false;
        wp.canAttack = true;
        Animator anim = sword.GetComponent<Animator>();
        anim.SetTrigger("ResetParry"); // Sets animation to idle
    }

    public void DeflectProjectile(Transform projectilePos)
    {
        Rigidbody rb = Instantiate(friendlyProjectile, projectilePos.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 16f, ForceMode.Impulse);
    }
}
