using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private List<GunAchievement> achievements = new List<GunAchievement>();
    private List<GunAchievement> unlockedAchievements = new List<GunAchievement>();

    private AchievementPanel achievementPanel;

    private void Awake()
    {
        achievements.AddRange(Resources.LoadAll<GunAchievement>("Achievements"));
        achievementPanel = FindObjectOfType<AchievementPanel>();
    }

    public void UnlockAchievement(String achievementName)
    {
        foreach (GunAchievement achievement in achievements)
        {
            if (achievement.achievementName == achievementName)
            {
                if (unlockedAchievements.Contains(achievement))
                {
                    Debug.LogWarning("Achievement already unlocked: " + achievementName);
                    return;
                }
                unlockedAchievements.Add(achievement);
                achievementPanel.ShowDetails(achievement);
                return;
            }
        }

        Debug.LogWarning("Achievement not found: " + achievementName);
    }
}
