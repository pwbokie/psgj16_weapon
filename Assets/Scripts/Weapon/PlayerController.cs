using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float maxTorque = 25f;
    public float firepower = 5f;
    public float casingEjectionForce = 5f;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void FixedUpdate()
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
    }

    [Header("FX Prefabs")]
    public GameObject FX_MuzzleFlash;
    public GameObject FX_Casing;

    [Header("FX Spawn Sources")]
    public GameObject muzzleFlashSource;
    public GameObject casingEjectionSource;

    [ContextMenu("Fire")]
    public void Fire()
    {
        GameObject fx_go = Instantiate(FX_MuzzleFlash, muzzleFlashSource.transform.position, Quaternion.identity);
        fx_go.transform.parent = muzzleFlashSource.transform;
        rb2d.AddForce(((transform.up * 0.1f) + -transform.right) * firepower, ForceMode2D.Impulse);

        GameObject casing_go = Instantiate(FX_Casing, casingEjectionSource.transform.position, Quaternion.identity);
        casing_go.GetComponent<Rigidbody2D>().AddForce((casing_go.transform.up + -casing_go.transform.right) * casingEjectionForce, ForceMode2D.Impulse);
        casing_go.GetComponent<Rigidbody2D>().AddTorque(1, ForceMode2D.Impulse);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }
}
