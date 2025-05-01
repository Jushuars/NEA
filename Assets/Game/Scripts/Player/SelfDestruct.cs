using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // Variables
    [Header("Timer")]
    public float time = 20;

    // Update is called once per frame
    void Update()
    {
        // Counts down timer
        time -= Time.deltaTime;
        // Destroys game object when timer is 0
        if (time <= 0)
            Destroy(gameObject);
    }
}
