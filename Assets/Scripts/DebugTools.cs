using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    public GameObject scope;
    public GameObject silencer;
    public GameObject chicken;

    public GameObject attachmentParent;

    public void AddScope() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(scope);
    }

    public void AddSilencer() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(silencer);
    }

    public void AddChicken() {
        attachmentParent.GetComponent<AttachmentParent>().AddAttachment(chicken);
    }
}
