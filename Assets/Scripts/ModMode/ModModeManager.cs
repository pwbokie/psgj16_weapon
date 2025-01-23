using System.Collections;
using Cinemachine;
using UnityEngine;

public class ModModeManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private float normalTimeScale = 1f;
    private float slowdownTimeScale = 0.01f;
    private float freezeTimeScale = 0f; // Completely frozen
    private float transitionSpeed = 5f; // Speed of the slowdown
    private float cameraZoomSpeed = 4f; // Speed of the camera zoom
    private bool isModModeActive = false;

    public void ToggleModMode()
    {
        if (isModModeActive)
        {
            DisableModMenu();
        }
        else
        {
            EnableModMenu();
        }
    }

    [ContextMenu("Mod Mode")]
    public void EnableModMenu()
    {
        if (isModModeActive) return;

        isModModeActive = true;
        StartCoroutine(SlowdownAndFreeze());
        StartCoroutine(SmoothCameraZoom(2f)); // Gradually zoom in
    }

    [ContextMenu("Exit Mod Mode")]
    public void DisableModMenu()
    {
        if (!isModModeActive) return;

        isModModeActive = false;
        StartCoroutine(SpeedUpAndResume());
        StartCoroutine(SmoothCameraZoom(6f)); // Gradually zoom out
    }

    private IEnumerator SlowdownAndFreeze()
    {
        float currentScale = Time.timeScale;

        // Slow down to the minimum scale
        while (Time.timeScale > slowdownTimeScale + 0.0001f) // Add a small epsilon to avoid infinite loop
        {
            currentScale = Mathf.Lerp(currentScale, slowdownTimeScale, transitionSpeed * Time.unscaledDeltaTime);
            Time.timeScale = Mathf.Max(currentScale, slowdownTimeScale); // Clamp to avoid going below the target
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Update physics time step
            yield return null;
        }

        // Final freeze
        Time.timeScale = freezeTimeScale; // Completely freeze
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private IEnumerator SpeedUpAndResume()
    {
        float currentScale = Time.timeScale;

        // Gradually speed up back to normal time scale
        while (Time.timeScale < normalTimeScale - 0.01f) // Use an epsilon to avoid infinite looping
        {
            currentScale = Mathf.Lerp(currentScale, normalTimeScale, transitionSpeed * Time.unscaledDeltaTime);
            Time.timeScale = Mathf.Clamp(currentScale, slowdownTimeScale, normalTimeScale); // Clamp to stay within range
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Update physics time step
            yield return null;
        }

        // Ensure final values are exact
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private IEnumerator SmoothCameraZoom(float targetZoom)
    {
        float currentZoom = virtualCamera.m_Lens.OrthographicSize;

        while (Mathf.Abs(currentZoom - targetZoom) > 0.01f) // Check for a small difference to avoid infinite looping
        {
            currentZoom = Mathf.MoveTowards(currentZoom, targetZoom, cameraZoomSpeed * Time.unscaledDeltaTime);
            virtualCamera.m_Lens.OrthographicSize = currentZoom;
            yield return null; // Wait for the next frame
        }

        virtualCamera.m_Lens.OrthographicSize = targetZoom; // Ensure exact final value
    }
}