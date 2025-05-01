using UnityEngine;
using UnityEngine.UI;

public class ParryBar : MonoBehaviour
{
    // Variables
    public Slider slider;
    public Image fill;
    public Parry pr;

    private void Update()
    {
        if (pr.canParry)
            slider.value = 1;

        else slider.value = 0;
    }
}
