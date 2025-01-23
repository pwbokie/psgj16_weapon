using System.Collections;
using Cinemachine;
using UnityEngine;

public class ModModeManager : MonoBehaviour
{
    public GameObject playMode;
    public GameObject modMode;

    public bool isModModeActive = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleModMode();
        }
    }
    
    public void ToggleModMode()
    {
        if (isModModeActive)
        {
            EnableModMode();
        }
        else
        {
            DisableModMode();
        }
    }

    [ContextMenu("Mod Mode")]
    public void EnableModMode()
    {
        playMode.SetActive(false);
        modMode.SetActive(true);
    }

    [ContextMenu("Exit Mod Mode")]
    public void DisableModMode()
    {
        playMode.SetActive(true);
        modMode.SetActive(false);
    }
}