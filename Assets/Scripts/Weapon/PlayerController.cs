using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    //public float maxTorque = 25f;
    public float firepower = 5f;
    public float casingEjectionForce = 5f;

    public Camera mainCamera;
    public GameObject casingParent;
    public float torqueForce = 1f;
    public float rotationDamping = 2f;
    public float stopThreshold = 0.2f;

    public int money;
    public TextMeshProUGUI moneyText;

    public int maxAmmo = 6;
    public int currentAmmo = 6;

    public AudioClip gunEmptyClickSound;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private AttachmentParent attachmentParent;

    public List<AttachmentEffect> activeEffects;

    public LayerMask detectionLayer;
    public LayerMask attachmentLayer;

    private Vector3 mouseWorldPosition;

    private AchievementManager achievementManager;
    private ModModeManager modModeManager;

    public List<GameObject> allAttachments;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = GetComponentInParent<Camera>();

        attachmentParent = GetComponent<AttachmentParent>();
        detectionLayer = LayerMask.GetMask("Shootable");

        achievementManager = FindObjectOfType<AchievementManager>();
        modModeManager = FindObjectOfType<ModModeManager>();

        UpdateAmmoCount();

        allAttachments = new List<GameObject>();

        UpdateMoneyDisplay();
    }

    private GameObject hoveredAttachment;

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // play mode logic
        if (!modModeManager.isModModeActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryFire();
            }
        }
        // mod mode logic
        else
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, attachmentLayer);
            if (hit != null && hit.gameObject != hoveredAttachment && hit.gameObject.tag == "Attachment")
            {  
                if (hoveredAttachment != null && hoveredAttachment != modModeManager.selectedAttachment)
                {
                    hoveredAttachment.GetComponent<SpriteRenderer>().color = Color.white;
                }

                hoveredAttachment = hit.gameObject;
                
                if (modModeManager.selectedAttachment == null)
                {
                    hoveredAttachment.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }
            else if (hit == null && hoveredAttachment != null && hoveredAttachment != modModeManager.selectedAttachment)
            {
                hoveredAttachment.GetComponent<SpriteRenderer>().color = Color.white;
                hoveredAttachment = null;
            }

            if (Input.GetMouseButtonDown(0) && hoveredAttachment != null)
            {
                modModeManager.SelectAttachment(hoveredAttachment);
            }
        }
        Vector3 right = new Vector3(1, 20, 0);
        Debug.DrawRay(rb2d.position, right, Color.cyan, 0.1f);
        
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
        item.GetComponent<Enemy>().TakeDamage();

    }

    public float maxRotationalVelocity = 10f;
    public float submarineModeMaxMovement = 5f;
    
    void FixedUpdate()
    {
        if (!submarineModeActive)
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

            // clamp angular velocity so we aren't clipping through objects and stuff
            rb2d.angularVelocity = Mathf.Clamp(rb2d.angularVelocity, -maxRotationalVelocity, maxRotationalVelocity);
        }
        else {
            if (Input.GetKey(KeyCode.W))
            {
                rb2d.AddForce(transform.up, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb2d.AddForce(-transform.up, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.AddForce(-transform.right, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb2d.AddForce(transform.right, ForceMode2D.Impulse);
            }

            rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, submarineModeMaxMovement);
        }
    }

    [Header("FX Prefabs")]
    public GameObject FX_MuzzleFlash;
    public GameObject FX_Casing;

    [Header("FX Spawn Sources")]
    public GameObject muzzleFlashSource;
    public GameObject casingEjectionSource;

    // for funny achievement
    private int dryFireConsecutiveCount = 0;

    public void TryFire()
    {
        if (currentAmmo > 0)
        {
            Fire();
            dryFireConsecutiveCount = 0;
        }
        else
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(gunEmptyClickSound);
            dryFireConsecutiveCount++;

            if (dryFireConsecutiveCount == 50)
            {
                achievementManager.UnlockAchievement("I think it's empty");
            }
        }
    }

    private float tempPowerMod = 1f;

    public void Fire()
    {
        // additional multiplier which is affected by attachments and stuff
        tempPowerMod = 1f;
        
        // d6 logic
        foreach (GameObject attachment in allAttachments)
        {
            if (attachment.GetComponent<Attachment_D6>() != null)
            {
                tempPowerMod += 0.3f * attachment.GetComponent<Attachment_D6>().DoRoll();
            }
        }

        currentAmmo--;
        UpdateAmmoCount();

        GameObject muzzleFlashGO = Instantiate(FX_MuzzleFlash, muzzleFlashSource.transform.position, Quaternion.identity);
        muzzleFlashGO.transform.parent = muzzleFlashSource.transform;

        rb2d.AddForce(((transform.up * 0.1f) + -transform.right) * firepower * tempPowerMod, ForceMode2D.Impulse);

        GameObject casing_go = Instantiate(FX_Casing, casingEjectionSource.transform.position, Quaternion.identity, casingParent.transform);
        casing_go.GetComponent<Rigidbody2D>().AddForce((casing_go.transform.up + -casing_go.transform.right) * casingEjectionForce, ForceMode2D.Impulse);
        casing_go.GetComponent<Rigidbody2D>().AddTorque(1, ForceMode2D.Impulse);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();

        RaycastHit2D hit = Physics2D.Raycast(rb2d.position, rb2d.transform.right, Mathf.Infinity, detectionLayer);
        if (hit.collider != null)
        {
            HandleHitObject(hit.collider.gameObject);
        }
    }

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoCount();
    }

    public TextMeshProUGUI ammoText;

    public void UpdateAmmoCount()
    {
        if (currentAmmo < 0) currentAmmo = 0;
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void SetMoney(int amount)
    {
        money = amount;
        UpdateMoneyDisplay();
    }

    public void UpdateMoneyDisplay()
    {
        moneyText.text = "$" + money.ToString();
    }

    // Stuff for attachment effects.
    [Header("Gun Effects Params")]
    public AudioClip silencedGunshotSound;
    public AudioClip rubberChickenSound;

    private bool perfectAccuracyActive = false;
    private bool submarineModeActive = false;

    public void AddEffect(AttachmentEffect effect, float ammo = 0f)
    {
        activeEffects.Add(effect);

        switch (effect)
        {
            case AttachmentEffect.NONE:
                Debug.LogWarning("Somehow added NONE to active effects on the gun");
                break;
            case AttachmentEffect.PERFECT_ACCURACY:
                perfectAccuracyActive = true;
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
            case AttachmentEffect.SUBMARINE:
                submarineModeActive = true;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                rb2d.angularVelocity = 0;
                rb2d.velocity = Vector2.zero;
                rb2d.freezeRotation = true;
                rb2d.gravityScale = 0;
                break;
            case AttachmentEffect.MORE_AMMO:
                maxAmmo += (int)ammo;
                currentAmmo += (int)ammo;
                UpdateAmmoCount();
                break;
            default:
                break;
        }
    }

    public void RemoveEffect(AttachmentEffect effect, float ammo = 0f)
    {
        if (!activeEffects.Contains(effect))
        {
            Debug.LogWarning("Tried to remove an effect that wasn't active on the gun");
            return;
        }

        activeEffects.Remove(effect);

        // These effects are ones that stack, so we need to remove them multiple times.
        switch (effect)
        {
            case AttachmentEffect.NONE:
                Debug.LogWarning("Somehow removed NONE from active effects on the gun");
                break;
            case AttachmentEffect.MORE_FIREPOWER:
                firepower -= 5f;
                break;
            default:
                break;
        }

        // Some effects only go away once every instance of the effect is removed.
        if (!activeEffects.Contains(effect) && !activeEffects.Any(e => e == effect)) {
            switch (effect)
            {
                case AttachmentEffect.NONE:
                    Debug.LogWarning("Somehow removed NONE from active effects on the gun");
                    break;
                case AttachmentEffect.PERFECT_ACCURACY:
                    perfectAccuracyActive = false;
                    break;
                case AttachmentEffect.SILENCED:
                    audioSource.clip = null;
                    break;
                case AttachmentEffect.RUBBER_CHICKEN:
                    audioSource.clip = null;
                    break;
                case AttachmentEffect.SUBMARINE:
                    submarineModeActive = false;
                    rb2d.freezeRotation = false;
                    rb2d.gravityScale = 2.4f;
                    break;
                case AttachmentEffect.MORE_AMMO:
                    maxAmmo -= (int)ammo;
                    currentAmmo -= (int)ammo;
                    UpdateAmmoCount();
                    break;
                default:
                    break;
            }
        }
    }

    [Header("HealthBar")]
    public GameObject HealthBar;
    
}
