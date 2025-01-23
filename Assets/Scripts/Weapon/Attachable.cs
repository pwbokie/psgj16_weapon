using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public string attachmentName;
    public string description;
    
    public AttachmentType type;
    public List<AttachmentEffect> effects;

    private bool hasGivenBuff = false;
    private PlayerController playerController;

    public void Awake() {
        
        playerController = FindObjectOfType<PlayerController>();
        if (!hasGivenBuff) {
            foreach (AttachmentEffect effect in effects) {
                playerController.AddEffect(effect);
            }
        }
    }
}
