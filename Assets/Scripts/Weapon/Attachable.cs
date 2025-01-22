using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
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

public enum AttachmentEffect
{
    NONE,
    PERFECT_ACCURACY,
    MORE_FIREPOWER,
    SILENCED,
    RUBBER_CHICKEN
}