using UnityEngine;
using UnityEngine.UI;

public class GrappleBar : MonoBehaviour
{
    // Variables
    public Slider slider;
    public Image fill;
    public Grappling gp;

    private void Update()
    {
        if (gp.grapplingCdTimer <= 0)
            slider.value = 1;

        else slider.value = 0;
    }
}
