using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment_D6 : MonoBehaviour
{
    public List<Sprite> diceFaces;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public int DoRoll()
    {
        int roll = Random.Range(1, 7);
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.Play();
        spriteRenderer.sprite = diceFaces[roll - 1];
        return roll;
    }
}
