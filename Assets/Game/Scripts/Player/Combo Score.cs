using UnityEngine;

public class ComboScore : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public Player pl;

    [Header("Stats")]
    public float score;
    public float multiplier;
    public string rank;
    public bool inCombat;
    public float combatCD;
    private float combatTimer;
    private bool resetCombatTimer;

    private void Update()
    {
        if (combatTimer > 0) // Counts down combat cooldown
            combatTimer -= Time.deltaTime;

        if (combatTimer <= 0) // Sets player in combat
            inCombat = false;

        if (!inCombat) // Decreases score when out of combat
        {
            score -= Time.deltaTime * 5;
            if (score <= 200)
                score = 200;
            return;
        }

        if (score < 0) // Prevents score from going below 0
            score = 0;

        if(score >= 501)
        {
            score -= Time.deltaTime * (1 + (score - 500) / 100);
        }

        // Decides the rank the player should get and their reward
        if (score <= 0)
        {
            pl.health -= Time.deltaTime * 10 * multiplier;
            rank = "No Lemons";
        }
        else if (score <= 50)
        {
            pl.health -= Time.deltaTime * 5 * multiplier;
            rank = "F";
        }
        else if (score <= 150)
        {
            pl.health -= Time.deltaTime * 2 * multiplier;
            rank = "E";
        }
        else if (score <= 250)
        {
            rank = "D";
        }
        else if (score <= 350)
        {
            pl.health += Time.deltaTime * multiplier;
            rank = "C";
        }
        else if (score <= 400)
        {
            pl.health += Time.deltaTime * 2 * multiplier;
            rank = "B";
        }
        else if (score <= 500)
        {
            pl.health += Time.deltaTime * 3 * multiplier;
            rank = "A";
        }
        else if (score >= 501)
        {
            pl.health += Time.deltaTime * 5 * multiplier;
            rank = "Making Lemonade";
        }
    }
    public void CombatUpdate()
    {
        resetCombatTimer = false;
        combatTimer = combatCD;
    }
}
