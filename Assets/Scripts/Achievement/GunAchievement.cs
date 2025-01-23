using UnityEngine;

[CreateAssetMenu(fileName = "GunAchievement", menuName = "Achievements/GunAchievement")]
public class GunAchievement : ScriptableObject
{
    public string achievementName;
    public string description;
    public Sprite incompleteIcon;
    public Sprite completeIcon;
}
