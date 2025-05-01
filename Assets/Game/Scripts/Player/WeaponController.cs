using NUnit.Framework.Internal.Execution;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

public class WeaponController : MonoBehaviour
{
    // Variables
    [Header("Sword attributes")]
    public bool canAttack;
    public float attackCd;
    public bool isAttacking = false;
    public float attackFinish;
    private float attackCdTimer;

    [Header("Finisher attributes")]
    public bool finisherAttempt = false;

    [Header("Keybinds")]
    public KeyCode attackKey = KeyCode.Mouse1;
    public KeyCode finisherKey = KeyCode.R;

    [Header("Reference")]
    public GameObject sword;
    public AudioClip swordSound;
    public AudioClip hitSound;
    public collisionDetection cd;
    public Parry pr;

    private void Update() // Runs every frame
    {
        if (Input.GetKeyDown(attackKey) && canAttack && !pr.isParrying) // Checks for attack input
        {
            SwordAttack();
        }
        if(Input.GetKeyDown(finisherKey) && !pr.isParrying && !isAttacking && !finisherAttempt)
        {
            PerformFinisher();
        }
        if (attackCdTimer < 0) // Allows player to attack
            canAttack = true;
        if (attackCdTimer > 0) // Countdown for sword attack cooldown
            attackCdTimer -= Time.deltaTime;
        
        if (isAttacking || finisherAttempt || pr.isParrying) cd.hitbox.enabled = true; // Turns hitbox on if attacking
        if (!isAttacking && !finisherAttempt && !pr.isParrying) cd.hitbox.enabled = false; // Turns hitbox off if attacking

        if (cd.playHitSound) // Plays hit sound when hit enemy
        {
            AudioSource ac = GetComponent<AudioSource>();
            cd.playHitSound = false;
            ac.PlayOneShot(hitSound);
        }
    }
    public void SwordAttack() // Subroutine which runs the sword attack
    {
        isAttacking = true;
        attackCdTimer = attackCd;
        canAttack = false;
        Animator anim = sword.GetComponent<Animator>();
        anim.SetTrigger("Attack1"); // Swing animation
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(swordSound); // Swing sound
        Invoke(nameof(ResetAttack),attackFinish); // Starts countdown to reset attack
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }
    private void PerformFinisher()
    {
        finisherAttempt = true;
        Invoke(nameof(ResetFinisher), 1f);
        Animator anim = sword.GetComponent<Animator>();
        anim.SetTrigger("Finisher"); // Finisher Animation
    }
    private void ResetFinisher()
    {
        finisherAttempt = false;
    }
}
