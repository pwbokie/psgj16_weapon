using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class AttachmentParent : MonoBehaviour
{
    public List<AttachmentSlot> slots;
    private PlayerController player;

    public void Awake() {
        UpdateSlots();
        player = FindObjectOfType<PlayerController>();
    }

    public void UpdateSlots()
    {
        slots = new List<AttachmentSlot>(GetComponentsInChildren<AttachmentSlot>());
    }

    public bool AddAttachment(GameObject attachable_go) {
        if (attachable_go.GetComponent<Attachable>() == null) {
            Debug.LogError("The provided GameObject does not have an Attachable component.");
            return false;
        }

        Attachable attachable = attachable_go.GetComponent<Attachable>();

        for (int i = 0; i < slots.Count; i++) {
            if (!slots[i].Occupied() && slots[i].type == attachable.type) {
                slots[i].attachment = Instantiate(attachable_go, slots[i].transform.position, slots[i].transform.rotation, player.transform);
                slots[i].attachment.GetComponent<Attachable>().slot = slots[i];

                AttachmentText attachmentText = FindObjectOfType<AttachmentText>();
                attachmentText.ShowDetails(attachable);

                player.allAttachments.Add(slots[i].attachment);

                Debug.Log("Attached " + attachable.type + " onto the " + gameObject.name);
                
                // non-player-centric logic, just logistical stuff
                switch(attachable.type) {
                    case AttachmentType.MUZZLE:
                        player.muzzleFlashSource.transform.localPosition = player.muzzleFlashSource.transform.localPosition + attachable_go.transform.Find("MuzzleFlashSource").localPosition;
                        if (player.muzzleFlashSource == null) {
                            Debug.LogError("Muzzle flash source not found on the new muzzle. Make sure the muzzle has a child with the exact name MuzzleFlashSource.");
                        }
                        break;
                }

                return true;
            }
            // if the matching slot is taken, can we attach to an attachment instead?
            else if (slots[i].type == attachable.type && slots[i].Occupied() == true && slots[i].attachment.GetComponent<AttachmentParent>() != null) {
                return slots[i].attachment.GetComponent<AttachmentParent>().AddAttachment(attachable_go);
            }
        }
        return false;
    }
}
