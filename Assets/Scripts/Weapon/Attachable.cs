using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public string attachmentName;
    public string description;
    public int value;

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

    public int GetSellValue() {
        if (value == 0) {
            Debug.LogWarning("Value is 0 for " + attachmentName + ", did you forget to set it?");
            return 0;
        }

        return Mathf.RoundToInt(value * 0.5f);
    }
}
