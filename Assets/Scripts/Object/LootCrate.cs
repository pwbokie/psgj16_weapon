using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour
{
    private PlayerController playerController;
    private GameObject contents;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (contents != null && contents.GetComponent<Attachable>() != null)
            {
                playerController.GetComponent<AttachmentParent>().AddAttachment(contents);
                gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
            }
            else if (contents == null)
            {
                Debug.LogWarning("LootCrate has no contents assigned!");
            }
        }
    }

    public void SetContents(GameObject newContents)
    {
        contents = newContents;
    }
}
