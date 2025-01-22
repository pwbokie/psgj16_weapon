using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour
{
    public CrateType crateType;

    private PlayerController playerController;
    private GameObject contents;

    private bool handling = false;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // for some reason I am gaining the same attachment multiple times
        // oncollisionenter probably happening on multiple threads or something
        // anyway here's wonderwall
        if (!handling)
        {
            handling = true;

            if (collision.gameObject.CompareTag("Player"))
            {
                if (contents != null && crateType == CrateType.ATTACHMENT && contents.GetComponent<Attachable>() != null)
                {   
                    if (playerController.GetComponent<AttachmentParent>().AddAttachment(contents))
                    {
                        gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
                    }
                }
                else if (contents == null && crateType == CrateType.AMMO && playerController.currentAmmo < playerController.maxAmmo)
                {
                    playerController.RefillAmmo();
                    gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
                }
                else if (contents == null)
                {
                    Debug.LogWarning("LootCrate has no contents assigned!");
                }
            }

            handling = false;
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