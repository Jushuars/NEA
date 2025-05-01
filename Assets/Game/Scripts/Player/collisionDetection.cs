using UnityEngine;

public class collisionDetection : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public WeaponController wp;
    public Parry pr;
    public GameObject hitParticle;
    public LayerMask hitLayer;
    public BoxCollider hitbox;
    public bool playHitSound;
    public int damage;
    public Enemy enemy;
    public MeleeStunned MS;
    public RangedStunned RS;
    public RangedEnemyAI rAI;
    public MeleeEnemyAI mAI;
    public Player player;
    public float parryStunDuration;
    public ProjectileDamage pd;
    public MeleeDamage MD;
    public ComboScore cs;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Ranged_Enemy" ||other.tag == "Melee_Enemy") && wp.isAttacking) // Runs if hitbox hit a tag called "Enemy"
        {
            Debug.Log(other.name);
            other.GetComponent<Animator>().SetTrigger("Hit");

            // Makes sure particles location is on the hit object
            Instantiate (hitParticle, new Vector3(other.transform.position.x, 
                transform.position.y, other.transform.position.z), other.transform.rotation);

            enemy = other.GetComponent<Enemy>();

            enemy.TakeDamage(damage);

            playHitSound = true;
        }
        else if(other.tag == "Enemy_Projectile" && pr.isParrying) // Runs if player is parrying and hits a projectile
        {
            pd = other.GetComponent<ProjectileDamage>();
            pr.resetParry = true;
            Animator anim = pr.sword.GetComponent<Animator>();
            anim.SetTrigger("ResetParry"); // Sets animation to idle
            pr.DeflectProjectile(other.transform);
            pd.DestroyObject();
            cs.score += 25;
            cs.CombatUpdate();
        }

        else if(other.tag == "Weapon_Hitbox_Enemy" && pr.isParrying) // Runs if player parries and contacts the enemy sword
        {
            MD = other.GetComponent<MeleeDamage>();
            MD.CallMSParry(parryStunDuration);
            pr.resetParry = true;
            Animator anim = pr.sword.GetComponent<Animator>();
            anim.SetTrigger("ResetParry"); // Sets animation to idle
            cs.score += 25;
            cs.CombatUpdate();
        }

        if(other.tag == "Melee_Enemy" && wp.finisherAttempt) // Runs if player trys to perfrom a finisher on a melee enemy
        {
            Debug.Log("Finisher detected Melee");
            mAI = other.GetComponent<MeleeEnemyAI>();
            mAI.FinisherKill();
            cs.score += 50;
            cs.CombatUpdate();
        }
        if (other.tag == "Ranged_Enemy" && wp.finisherAttempt) // Runs if player trys to perfrom a finisher on a ranged enemy
        {
            Debug.Log("Finisher detected Ranged");
            rAI = other.GetComponent<RangedEnemyAI>();
            rAI.FinisherKill();
            cs.score += 50;
            cs.CombatUpdate();
        }
    }
}
