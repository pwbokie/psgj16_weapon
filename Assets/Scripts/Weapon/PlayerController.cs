using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    //public float maxTorque = 25f;
    public float firepower = 5f;
    public float casingEjectionForce = 5f;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private AttachmentParent attachmentParent;

    public List<AttachmentEffect> activeEffects;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        attachmentParent = GetComponent<AttachmentParent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        transform.up = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
    }

    /*void FixedUpdate()
    {
        // Get cursor position in world space
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction to the cursor
        Vector2 lookDir = cursorPos - (Vector2)transform.position;
        // Desired angle towards the cursor
        float desiredAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        // Get current angle
        float currentAngle = transform.eulerAngles.z;
        // Calculate the shortest angle difference
        float angleDifference = Mathf.DeltaAngle(currentAngle, desiredAngle);

        float torque = angleDifference * 0.1f; // Adjust multiplier for responsiveness
        torque = Mathf.Clamp(torque, -maxTorque, maxTorque);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddTorque(torque);
        }
    }*/

    [Header("FX Prefabs")]
    public GameObject FX_MuzzleFlash;
    public GameObject FX_Casing;

    [Header("FX Spawn Sources")]
    public GameObject muzzleFlashSource;
    public GameObject casingEjectionSource;

    [ContextMenu("Fire")]
    public void Fire()
    {
        GameObject muzzleFlashGO = Instantiate(FX_MuzzleFlash, muzzleFlashSource.transform.position, Quaternion.identity);
        muzzleFlashGO.transform.parent = muzzleFlashSource.transform;

        rb2d.AddForce(((transform.up * 0.1f) + -transform.right) * firepower, ForceMode2D.Impulse);

        GameObject casing_go = Instantiate(FX_Casing, casingEjectionSource.transform.position, Quaternion.identity);
        casing_go.GetComponent<Rigidbody2D>().AddForce((casing_go.transform.up + -casing_go.transform.right) * casingEjectionForce, ForceMode2D.Impulse);
        casing_go.GetComponent<Rigidbody2D>().AddTorque(1, ForceMode2D.Impulse);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }

    // Stuff for effects.
    [Header("Gun Effects Params")]
    public AudioClip silencedGunshot;

    public void AddEffect(AttachmentEffect effect)
    {
        if (activeEffects.Contains(effect))
        {
            return;
        }

        activeEffects.Add(effect);

        switch (effect)
        {
            case AttachmentEffect.NONE:
                Debug.LogWarning("Somehow added NONE to active effects on the gun");
                break;
            case AttachmentEffect.PERFECT_ACCURACY:
                // Implement perfect accuracy logic here
                break;
            case AttachmentEffect.MORE_FIREPOWER:
                firepower += 5f;
                break;
            case AttachmentEffect.SILENCED:
                audioSource.clip = Resources.Load<AudioClip>("Sound/SilencedShot");
                break;
            default:
                break;
        }
    }
}
