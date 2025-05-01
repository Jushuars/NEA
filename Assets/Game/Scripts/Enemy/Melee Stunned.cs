using UnityEngine;

public class MeleeStunned : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public MeleeEnemyAI mAI;
    public GameObject self;

    [Header("Attributes")]
    public bool parried;
    public Animator anim1;
    public Animator anim2;

    public void Parried(float stunDuration) // Publically called when parried
    {
        parried = true;

        Debug.Log("Parry hit");
        Debug.Log(stunDuration);

        mAI.ResetAttack(); // Stops attack

        Invoke(nameof(ResetParry), stunDuration);

        Animator reset = mAI.sword.GetComponent<Animator>();
        reset.SetTrigger("ResetAttack"); // Reset Attack Animation Early
        anim1.enabled = false;

        anim2.enabled = true;
        Animator anim = self.GetComponent<Animator>();
        anim.SetTrigger("Parried"); // Parried animation
    }
    private void ResetParry()
    {
        parried = false;

        Animator anim = self.GetComponent<Animator>();
        anim.SetTrigger("ResetParried"); // Sets animation to idle

        // Resets enabled animator components
        anim1.enabled = true;
        Invoke(nameof(DisableAnimation2), 0.5f);
    }
    public void DisableAnimation2()
    {
        anim2.enabled = false;
        Invoke(nameof(RenableAnimation2), 7f);
    }
    public void DisabledAnimation1()
    {
        anim1.enabled = false;
        Invoke(nameof(RenableAnimation1), 7f);
    }
    private void RenableAnimation2()
    {
        anim2.enabled = true;
    }
    private void RenableAnimation1()
    {
        anim1.enabled = true;
    }
}

