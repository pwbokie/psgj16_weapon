using System.Collections;
using UnityEngine;

public class FX : MonoBehaviour
{
    private Animator animator;
    private float animationDuration;

    void Awake()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        // Ensure the Animator component exists
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
            return;
        }

        animator.Play(0);

        // Schedule the destruction of the GameObject after the animation finishes
        Destroy(gameObject, 1f);
    }
}