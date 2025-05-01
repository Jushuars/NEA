using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    // Variables
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(float health) // Applies the max health the player has
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue); // Changes the color of the health bar based on health
    }

    public void SetHealth(float health) // Updates the health bar to the player's current health
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue); // Set's the player's health bar colour based on their current health
    }
}
