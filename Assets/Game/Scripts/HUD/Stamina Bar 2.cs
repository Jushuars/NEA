using UnityEngine;
using UnityEngine.UI;

public class StaminaBar2 : MonoBehaviour
{
    // Variables
    public Slider slider;
    public Image fill;
    public Dashing dashing;

    private void Update()
    {
        if (slider.value > 1)
            slider.value = 1;

        slider.value = dashing.dashCharges - 1;
    }
}