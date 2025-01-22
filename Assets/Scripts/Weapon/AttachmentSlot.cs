using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    public AttachmentType type;
    public GameObject attachment;
    public AttachmentParent parent;

    private bool renamed = false;
    public void Awake()
    {
        if (type == AttachmentType.NONE)
        {
            Debug.LogError("AttachmentSlot has no type assigned!");
        }
        if (!renamed) {
            gameObject.name = gameObject.name + "_"  + type.ToString();
            renamed = true;
        }
    }

    public bool Occupied()
    {
        if (attachment == null)
        {
            return false;
        }
        return true;
    }
}