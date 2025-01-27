using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour
{
    public CrateType crateType;

    private PlayerController playerController;
    public GameObject contents;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Attachment"))
        {
            if (contents != null && crateType == CrateType.ATTACHMENT && contents.GetComponent<Attachable>() != null)
            {   
                if (playerController.GetComponent<AttachmentParent>().AddAttachment(contents))
                {
                    gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
                }
            }
            else if (crateType == CrateType.AMMO && playerController.magazines < playerController.maxMagazines)
            {
                playerController.SetMagazines(playerController.magazines + 1);
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

public enum CrateType
{
    ATTACHMENT,
    AMMO
}