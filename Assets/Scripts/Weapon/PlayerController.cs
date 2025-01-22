using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    //public float maxTorque = 25f;
    public float firepower = 5f;
    public float casingEjectionForce = 5f;

    public Camera mainCamera;
    public float torqueForce = 1f;
    public float rotationDamping = 2f;
    public float stopThreshold = 1f;

    public int maxAmmo = 6;
    public int currentAmmo = 6;

    public AudioClip gunEmptyClickSound;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private AttachmentParent attachmentParent;

    public List<AttachmentEffect> activeEffects;

    public LayerMask detectionLayer;

    private Vector3 mouseWorldPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = GetComponentInParent<Camera>();

        attachmentParent = GetComponent<AttachmentParent>();
        detectionLayer = LayerMask.GetMask("Shootable");
    }

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            TryFire();
        }
    
        
        /*Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        transform.up = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));*/
    }

    void HandleHitObject(GameObject hitObject)
    {
        if(hitObject.tag == "Enemy")
        {
            DieDieDie(hitObject);
        }

        if(hitObject.tag == "Bubble")
        {
            hitObject.SendMessage("Pop");
        }


    }

    void DieDieDie(GameObject item)
    {
        Destroy(item);
    }

    void FixedUpdate()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        if(rb2d == null) return; 

        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentAngle = rb2d.rotation;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        if (Mathf.Abs(angleDifference) > stopThreshold)
        {
            float torque = angleDifference * torqueForce;

            rb2d.AddTorque(torque * rotationDamping);
        }
        else
        {
            rb2d.angularVelocity = 0f;
        }

        
    }

    [Header("FX Prefabs")]
    public GameObject FX_MuzzleFlash;
    public GameObject FX_Casing;

    [Header("FX Spawn Sources")]
    public GameObject muzzleFlashSource;
    public GameObject casingEjectionSource;

    public void TryFire()
    {
        
        if (currentAmmo > 0)
        {
            Fire();
            currentAmmo--;
        }
        else
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(gunEmptyClickSound);
        }
    }

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

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, detectionLayer);
        if (hit.collider != null)
        {
            HandleHitObject(hit.collider.gameObject);
        }
    }

    // Stuff for effects.
    [Header("Gun Effects Params")]
    public AudioClip silencedGunshotSound;
    public AudioClip rubberChickenSound;

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
                audioSource.clip = silencedGunshotSound;
                break;
            case AttachmentEffect.RUBBER_CHICKEN:
                audioSource.clip = rubberChickenSound;
                break;
            default:
                break;
        }
    }
}
