using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class Attachable : MonoBehaviour
{
    public string attachmentName;
    public string description;
    public int value;
    // for magazines
    public int ammo;

    public AttachmentType type;
    public List<AttachmentEffect> effects;

    private bool hasGivenBuff = false;
    private PlayerController playerController;

    public AttachmentSlot slot;

    public void Awake() {
        
        playerController = FindObjectOfType<PlayerController>();
        if (!hasGivenBuff) {
            foreach (AttachmentEffect effect in effects) {
                playerController.AddEffect(effect, ammo);
            }
        }
    }

    public int GetSellValue() {
        if (value == 0) {
            Debug.LogWarning("Value is 0 for " + attachmentName + ", did you forget to set it?");
            return 0;
        }

        return Mathf.RoundToInt(value * 0.5f);
    }

    // this still isn't exactly right but I'm dying and I need to move on
    public void RemoveAttachment()
    {
        AttachmentParent attachmentController = GetComponent<AttachmentParent>();

        if (attachmentController != null && attachmentController.slots.Count > 0)
        {
            foreach (AttachmentSlot slot in attachmentController.slots)
            {
                if (slot.attachment != null)
                {
                    if (slot.transform.parent.GetComponent<Attachable>().slot == null)
                    {
                        // base case, but we can't access the gun normally because of the dumb stuff we did to get this mode to work, so we'll cheat
                        slot.attachment.GetComponent<Attachable>().AdjustPositionRecursive(slot, playerController.GetComponent<Attachable>().slot);
                    }
                    else
                    {
                        slot.attachment.GetComponent<Attachable>().AdjustPositionRecursive(slot, slot.transform.parent.GetComponent<Attachable>().slot);
                    }
                }
            }
        }

        // my childrens' parents should be my parent
        if (attachmentController != null && attachmentController.slots.Count > 0)
        {
            foreach (AttachmentSlot slot in attachmentController.slots)
            {
                if (slot.attachment != null)
                {
                    slot.attachment.GetComponent<Attachable>().slot = GetComponent<Attachable>().slot;
                }
            }
            attachmentController.UpdateSlots();
        }

        // Destroy the current object and its shadow
        GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
    }

    public void AdjustPositionRecursive(AttachmentSlot slot, AttachmentSlot parentSlot)
    {
        slot.attachment.transform.position = parentSlot.transform.position;

        // do I own slots?
        AttachmentParent attachmentController = slot.attachment.GetComponent<AttachmentParent>();
        if (attachmentController != null && attachmentController.slots.Count > 0)
        {
            foreach (AttachmentSlot childSlot in attachmentController.slots)
            {
                if (childSlot.attachment != null)
                {
                    childSlot.attachment.GetComponent<Attachable>().AdjustPositionRecursive(childSlot, slot);
                }
            }
        }
    }
}
