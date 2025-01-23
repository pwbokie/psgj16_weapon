using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementPanel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public SpriteRenderer icon;

    public AudioClip achievementSound;

    private AudioSource audioSource;
    private Animator animator;

    public void Start()
    {    
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = achievementSound;
        animator = GetComponent<Animator>();
    }

    public void ShowDetails(GunAchievement achievement)
    {
        SetDetails(achievement);
        Show();
    }

    [ContextMenu("Test Achievement")]
    public void TestAchievement()
    {
        GunAchievement testAchievement = new GunAchievement();
        testAchievement.achievementName = "Test Achievement";
        testAchievement.description = "This is a test achievement.";
        ShowDetails(testAchievement);
    }

    public void SetDetails(GunAchievement achievement)
    {
        // attachable type with first letter capitalized and the rest lowercase, plus the name
        nameText.text = achievement.achievementName;
        descriptionText.text = achievement.description;
        icon.sprite = achievement.completeIcon;
    }

    public void Show()
    {
        animator.SetBool("achievementShown", true);
        audioSource.Play();
        StartCoroutine("HideAchievement", 8f);
    }

    IEnumerator HideAchievement(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("achievementShown", false);
    }
}
