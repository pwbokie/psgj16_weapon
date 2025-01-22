using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttachmentParent : MonoBehaviour
{
    public List<AttachmentSlot> slots;
    private PlayerController player;

    public void Start() {
        slots = new List<AttachmentSlot>(GetComponentsInChildren<AttachmentSlot>());
        player = FindObjectOfType<PlayerController>();
    }

    public void AddAttachment(GameObject attachable_go) {
        bool attached = false;

        if (attachable_go.GetComponent<Attachable>() == null) {
            Debug.LogError("The provided GameObject does not have an Attachable component.");
            return;
        }

        Attachable attachable = attachable_go.GetComponent<Attachable>();

        for (int i = 0; i < slots.Count; i++) {
            if (!slots[i].Occupied() && slots[i].type == attachable.type) {
                slots[i].attachment = attachable_go;
                Instantiate(attachable_go, slots[i].transform.position, slots[i].transform.rotation, player.transform);
                attached = true;
                Debug.Log("Attached " + attachable.type + " onto the " + gameObject.name);
                break;
            }
            else if (slots[i].GetComponent<AttachmentParent>() != null) {
                slots[i].GetComponent<AttachmentParent>().AddAttachment(attachable_go);
            }
        }
        
        if (!attached) {
            Debug.LogWarning("No available slot for attachment: " + attachable.type);
        }
        // Additional logic for specific attachment types added here
        else {
            switch(attachable.type) {
                case AttachmentType.MUZZLE:
                    player.muzzleFlashSource.transform.localPosition = player.muzzleFlashSource.transform.localPosition + attachable_go.transform.Find("MuzzleFlashSource").localPosition;
                    if (player.muzzleFlashSource == null) {
                        Debug.LogError("Muzzle flash source not found on the new muzzle. Make sure the muzzle has a child with the exact name MuzzleFlashSource.");
                    }
                    break;
            }
        }
    }
}
