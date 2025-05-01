using UnityEngine;

public class RangedStunned : MonoBehaviour
{
    // Variables
    [Header("Attributes")]
    public bool stunned;
    public Animator anim1;
    public Animator anim2;
    public Animator anim3;

    [Header("Reference")]
    public GameObject self;
    public RangedEnemyAI rAI;

    public void Stunned(float stunDuration)
    {
        stunned = true;

        Debug.Log("Stunned");
        Debug.Log(stunDuration);

        rAI.ResetAttack(); // Stops attack

        Invoke(nameof(ResetStun), stunDuration);

        Animator reset1 = rAI.gun1.GetComponent<Animator>();
        reset1.SetTrigger("ResetAttack"); // Reset Attack Animation Early
        Animator reset2 = rAI.gun2.GetComponent<Animator>();
        reset2.SetTrigger("ResetAttack"); // Reset Attack Animation Early
        anim1.enabled = false;
        anim2.enabled = false;

        anim3.enabled = true;
        Animator anim = self.GetComponent<Animator>();
        anim.SetTrigger("Stun"); // Parried animation
    }
    private void ResetStun()
    {
        stunned = false;

        // Resets enabled animator components
        anim1.enabled = true;
        anim2.enabled = true;

        Animator anim = self.GetComponent<Animator>();
        anim.SetTrigger("ResetStun"); // Sets animation to idle

        Invoke(nameof(ResetAnimation), 1f);
    }
    private void ResetAnimation()
    {
        anim3.enabled = false;
    }
    public void DisabledAnimation1()
    {
        anim1.enabled = false;
        anim2.enabled = false;
        Invoke(nameof(ReenableAnimation1), 7f);
    }
    private void ReenableAnimation1()
    {
        anim1.enabled = true;
        anim2.enabled = true;

    }
}
