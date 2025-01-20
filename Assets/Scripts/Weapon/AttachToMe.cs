using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToMe : MonoBehaviour
{
    public List<AttachmentSlot> slots;

    public void Start() {
        slots = new List<AttachmentSlot>(GetComponentsInChildren<AttachmentSlot>());
    }
}
