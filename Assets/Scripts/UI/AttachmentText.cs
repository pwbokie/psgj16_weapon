using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttachmentText : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private Animator animator;

    public void Start()
    {    
        animator = GetComponent<Animator>();
    }

    public void ShowDetails(Attachable attachable)
    {
        SetText(attachable);
        Show();
    }

    public void SetText(Attachable attachable)
    {
        // attachable type with first letter capitalized and the rest lowercase, plus the name
        nameText.text = attachable.type.ToString().ToLower().Replace(attachable.type.ToString()[0], char.ToUpper(attachable.type.ToString()[0])) + ": " + attachable.attachmentName;
        descriptionText.text = attachable.description;
    }

    public void Show()
    {
        animator.SetBool("textShown", true);
        StartCoroutine("HideText", 7f);
    }

    IEnumerator HideText(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("textShown", false);
    }
}
