using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class BulletCasing : MonoBehaviour
{
    private AudioSource audioSource;
    private int playCount = 0; // Tracks the number of times the sound has played
    private const int maxPlays = 4; // Maximum number of times the sound can play
    private bool isOnCooldown = false; // Tracks if the sound is on cooldown
    private const float cooldownTime = 0.1f; // Cooldown duration in seconds

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
        }

        GetComponent<ShadowedObject>().DestroyThisAndItsShadow(10f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the sound has already played the maximum number of times and is not on cooldown
        if (playCount < maxPlays && !isOnCooldown && audioSource != null)
        {
            UnityEngine.Debug.Log("Playing casing sound");
            audioSource.pitch = Random.Range(0.7f, 1.3f);
            audioSource.Play();
            playCount++; // Increment the play count
            StartCoroutine(Cooldown()); // Start the cooldown
        }
    }

    private System.Collections.IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}