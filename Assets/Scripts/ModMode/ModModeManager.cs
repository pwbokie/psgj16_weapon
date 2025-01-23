using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ModModeManager : MonoBehaviour
{
    public GameObject playMode;
    public GameObject modMode;

    public GameObject playModeBackground;

    public Camera mainCamera;
    public CinemachineVirtualCamera cinemachineCamera;

    public Color playModeColor;
    public Color modModeColor;

    public Transform gunTransform; // Reference to the gun's Transform
    public float targetCameraZoom = 2f;
    public float cameraLerpSpeed = 3f; // Speed for zooming and rotation lerp

    public bool isModModeActive = false;

    private float originalCameraZoom;
    private Quaternion originalCameraRotation;

    private List<Rigidbody2D> pausedRigidbodies = new List<Rigidbody2D>();

    private void Start()
    {
        originalCameraZoom = cinemachineCamera.m_Lens.OrthographicSize;
        originalCameraRotation = cinemachineCamera.transform.rotation;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleModMode();
        }
    }

    public void ToggleModMode()
    {
        if (!isModModeActive)
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
        modMode.SetActive(true);
        mainCamera.backgroundColor = modModeColor;
        playModeBackground.SetActive(false);
        isModModeActive = true;

        PausePhysics();

        // Start lerping the camera to the target zoom and rotation
        StartCoroutine(LerpCamera(targetCameraZoom, Quaternion.LookRotation(Vector3.forward, gunTransform.up)));
    }

    [ContextMenu("Exit Mod Mode")]
    public void DisableModMode()
    {
        modMode.SetActive(false);
        mainCamera.backgroundColor = playModeColor;
        playModeBackground.SetActive(true);
        isModModeActive = false;

        ResumePhysics();

        // Start lerping the camera back to its original zoom and rotation
        StartCoroutine(LerpCamera(originalCameraZoom, originalCameraRotation));
    }

    private void PausePhysics()
    {
        Rigidbody2D[] rigidbodies = playMode.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in rigidbodies)
        {
            if (rb.simulated)
            {
                pausedRigidbodies.Add(rb);
                rb.simulated = false;
            }
        }
    }

    private void ResumePhysics()
    {
        foreach (var rb in pausedRigidbodies)
        {
            // make sure it wasn't destroyed because that breaks everything
            if (rb != null)
            {
                rb.simulated = true;
            }
        }
        pausedRigidbodies.Clear();
    }

    private IEnumerator LerpCamera(float targetZoom, Quaternion targetRotation)
    {
        float zoomStart = cinemachineCamera.m_Lens.OrthographicSize;
        Quaternion rotationStart = cinemachineCamera.transform.rotation;

        float lerpProgress = 0f;

        while (lerpProgress < 1f)
        {
            lerpProgress += Time.unscaledDeltaTime * cameraLerpSpeed;

            // Lerp camera size
            cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(zoomStart, targetZoom, lerpProgress);

            // Lerp camera rotation
            cinemachineCamera.transform.rotation = Quaternion.Slerp(rotationStart, targetRotation, lerpProgress);

            yield return null;
        }

        cinemachineCamera.m_Lens.OrthographicSize = targetZoom;
        cinemachineCamera.transform.rotation = targetRotation;
    }
}