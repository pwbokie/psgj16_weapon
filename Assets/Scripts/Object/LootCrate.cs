using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour
{
    public CrateType crateType;

    private PlayerController playerController;
    public GameObject contents;
    public bool legendaryGuaranteed = false;

    private String commonAttachmentsDirectory = "Prefabs/Attachments/Common/";
    private String legendaryAttachmentsDirectory = "Prefabs/Attachments/Legendary/";

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (contents == null)
        {
            if (crateType == CrateType.ATTACHMENT)
            {
                contents = GetRandomAttachment();
            }
        }
    }

    private GameObject GetRandomAttachment()
    {
        int random = UnityEngine.Random.Range(0, 100);

        GameObject[] commonAttachments = Resources.LoadAll<GameObject>(commonAttachmentsDirectory);
        GameObject[] legendaryAttachments = Resources.LoadAll<GameObject>(legendaryAttachmentsDirectory);

        GameObject attachment;
        if (legendaryGuaranteed || random < 5)
        {
            attachment = legendaryAttachments[UnityEngine.Random.Range(0, legendaryAttachments.Length)];
        }
        else
        {
            attachment = commonAttachments[UnityEngine.Random.Range(0, commonAttachments.Length)];
        }

        return attachment;
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