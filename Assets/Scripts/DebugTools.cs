using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    public GameObject scope;
    public GameObject silencer;
    public GameObject chicken;

    public GameObject attachmentParent;

    [ContextMenu("Add Scope")]
    public void AddScope() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(scope);
    }

    [ContextMenu("Add Silencer")]
    public void AddSilencer() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(silencer);
    }

    [ContextMenu("Add Chicken")]
    public void AddChicken() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(chicken);
    }
}
