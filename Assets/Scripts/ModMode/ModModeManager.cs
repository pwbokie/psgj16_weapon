using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

public class ModModeManager : MonoBehaviour
{
    private PlayerController player;
    public GameObject attachmentsHolder;

    public GameObject playMode;
    public GameObject modMode;

    public AudioSource music;

    public GameObject playModeBackground;

    public Camera mainCamera;
    public CinemachineVirtualCamera cinemachineCamera;

    public Color playModeColor;
    public Color modModeColor;

    public Transform gunTransform; // Reference to the gun's Transform
    public float targetCameraZoom = 2f;
    public float cameraLerpSpeed = 3f; // Speed for zooming and rotation lerp

    public float modModeMusicPitch = 0.8f;
    public float modModePitchChangeSpeed = 0.1f;

    public bool isModModeActive = false;

    private float originalCameraZoom;
    private Quaternion originalCameraRotation;

    private List<Rigidbody2D> pausedRigidbodies = new List<Rigidbody2D>();

    public GameObject sellButton;
    public TextMeshProUGUI sellButtonText;

    public AudioClip sellSound;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        originalCameraZoom = cinemachineCamera.m_Lens.OrthographicSize;
        originalCameraRotation = cinemachineCamera.transform.rotation;
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
        StopAllCoroutines();

        TakeAllAttachments();
        modMode.SetActive(true);
        PausePhysics();

        mainCamera.backgroundColor = modModeColor;
        playModeBackground.SetActive(false);
        isModModeActive = true;

        StartCoroutine(ChangeMusicPitch(modModeMusicPitch));

        // Start lerping the camera to the target zoom and rotation
        StartCoroutine(LerpCamera(targetCameraZoom, Quaternion.LookRotation(Vector3.forward, gunTransform.up)));
    }

    [ContextMenu("Exit Mod Mode")]
    public void DisableModMode()
    {
        StopAllCoroutines();

        ResumePhysics();
        modMode.SetActive(false);
        sellButton.SetActive(false);
        ReturnAllAttachments();

        mainCamera.backgroundColor = playModeColor;
        playModeBackground.SetActive(true);
        isModModeActive = false;

        StartCoroutine(ChangeMusicPitch(1f));

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

    private IEnumerator ChangeMusicPitch(float targetPitch)
    {
        while (Mathf.Abs(music.pitch - targetPitch) > 0.01f)
        {
            music.pitch = Mathf.Lerp(music.pitch, targetPitch, modModePitchChangeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        // Ensure the pitch is set exactly to the target
        music.pitch = targetPitch;
    }

    public GameObject selectedAttachment;

    public void TrySelectAttachment(GameObject attachment)
    {
        if (attachment != null)
        {
            selectedAttachment = attachment;
        }
    }

    private List<GameObject> borrowedAttachments = new List<GameObject>();

    public void TakeAllAttachments()
    {
        foreach (GameObject attachment in player.allAttachments)
        {
            borrowedAttachments.Add(attachment);
            attachment.transform.SetParent(attachmentsHolder.transform);
        }

        Debug.Log("Found " + borrowedAttachments.Count + " attachments");
    }

    public void ReturnAllAttachments()
    {
        foreach (GameObject attachment in borrowedAttachments)
        {
            attachment.GetComponent<SpriteRenderer>().color = Color.white;
            attachment.transform.SetParent(player.transform);
        }

        Debug.Log("Returned " + borrowedAttachments.Count + " attachments");

        borrowedAttachments.Clear();
    }

    // this code's getting messy
    // ... but was I ever the master of my own destiny?
    public void SelectAttachment(GameObject attachment)
    {
        if (attachment != null)
        {
            if (selectedAttachment != null)
            {
                selectedAttachment.GetComponent<SpriteRenderer>().color = Color.white;
            }

            selectedAttachment = attachment;
            selectedAttachment.GetComponent<SpriteRenderer>().color = Color.yellow;

            sellButton.SetActive(true);
            sellButtonText.text = "Sell " + selectedAttachment.GetComponent<Attachable>().attachmentName + ": $" + selectedAttachment.GetComponent<Attachable>().GetSellValue();
        }
    }

    public void SellSelectedAttachment()
    {
        if (selectedAttachment != null)
        {
            player.allAttachments.Remove(selectedAttachment);
            player.SetMoney(player.money + selectedAttachment.GetComponent<Attachable>().GetSellValue());
            
            borrowedAttachments.Remove(selectedAttachment);

            foreach (AttachmentEffect effect in selectedAttachment.GetComponent<Attachable>().effects){
                player.RemoveEffect(effect, selectedAttachment.GetComponent<Attachable>().ammo);
            }

            selectedAttachment.GetComponent<Attachable>().RemoveAttachment();
            
            player.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            player.GetComponent<AudioSource>().PlayOneShot(sellSound);

            sellButton.SetActive(false);
        }
    }
}