using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    public int magazines = 0;
    public int maxMagazines = 5;

    public AudioClip gunEmptyClickSound;
    public AudioClip magReloadSound;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;

    public List<AttachmentEffect> activeEffects;

    public LayerMask detectionLayer;
    public LayerMask attachmentLayer;

    private Vector3 mouseWorldPosition;

    private AchievementManager achievementManager;
    private ModModeManager modModeManager;

    public List<GameObject> allAttachments;

    public bool canControl = true;

    public CameraMode currentCameraMode;
    public CinemachineVirtualCamera closeCamera;
    public CinemachineVirtualCamera farCamera;
    private GameObject healthBarParent;
    private GameObject healthBar;
    private float parentHeight;
    public GameObject healthBarPrefab;
    public int currentHealth;
    public int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = GetComponentInParent<Camera>();

        detectionLayer = LayerMask.GetMask("Shootable");

        achievementManager = FindObjectOfType<AchievementManager>();
        modModeManager = FindObjectOfType<ModModeManager>();

        healthBarParent = GameObject.Find("HealthBars");
        parentHeight = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y/2f * gameObject.transform.localScale.y;
        HealthBar = Instantiate<GameObject>(healthBarPrefab, new Vector3(transform.position.x, parentHeight + transform.position.y + 0.01f, 0f), quaternion.identity);
        HealthBar.transform.localScale = gameObject.transform.localScale;

        UpdateAmmoCount();

        allAttachments = new List<GameObject>();

        UpdateMoneyDisplay();

        SetMagazines(0);
    }

    private GameObject hoveredAttachment;

    void Update()
    {
        if (canControl)
        {
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                ForceCloseCamera();
                modModeManager.ToggleModMode();
            }

            // play mode logic
            if (!modModeManager.isModModeActive)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TryFire();
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ReloadFromMag();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    ToggleCameraMode();
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
            //Debug.DrawRay(rb2d.position, right, Color.cyan, 0.1f);
            
            /*Vector2 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
            transform.up = direction;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));*/    
                    
        }
        UpdateHealthBar();
    }

    public void IncreasePlayerMoney(int moneyAmount)
    {
        money += moneyAmount;
        UpdateMoneyDisplay();
    }

    public void UpdateHealthBar()
    {
        HealthBar.transform.position = new Vector3(transform.position.x, parentHeight + transform.position.y + .3f, 0f);
        HealthBar.transform.rotation = quaternion.identity;

        HealthBar.transform.GetChild(0).GetChild(0).transform.localScale = new Vector3((float)currentHealth/(float)maxHealth * 2f, 1f, 1f);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {

    }

    public float maxRotationalVelocity = 10f;
    public float submarineModeMaxMovement = 5f;
    
    void FixedUpdate()
    {
        if (canControl)
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
    }

    [Header("FX Prefabs")]
    public GameObject FX_MuzzleFlash;
    public GameObject FX_Casing;
    public GameObject FX_Pea;

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

        GameObject casing_go;

        if (peashooterActive)
        {
            casing_go = Instantiate(FX_Pea, casingEjectionSource.transform.position, Quaternion.identity, casingParent.transform);
        }
        else
        {
            casing_go = Instantiate(FX_Casing, casingEjectionSource.transform.position, Quaternion.identity, casingParent.transform);
        }
        casing_go.GetComponent<Rigidbody2D>().AddForce((transform.up + -transform.right) * casingEjectionForce, ForceMode2D.Impulse);
        casing_go.GetComponent<Rigidbody2D>().AddForce(rb2d.velocity * 0.3f, ForceMode2D.Impulse);
        casing_go.GetComponent<Rigidbody2D>().AddTorque(1, ForceMode2D.Impulse);
        if (submarineModeActive)
        {
            casing_go.GetComponent<Rigidbody2D>().gravityScale = -casing_go.GetComponent<Rigidbody2D>().gravityScale * 0.1f;
        }

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();

        RaycastHit2D hit = Physics2D.Raycast(rb2d.position, rb2d.transform.right, Mathf.Infinity, detectionLayer);
        if (hit.collider != null)
        {
            HandleHitObject(hit.collider.gameObject);
        }
    }

    public void ReloadFromMag()
    {
        if (magazines > 0 && currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
            UpdateAmmoCount();
            magazines--;
            SetMagazines(magazines);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(magReloadSound);
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
        if (currentAmmo > maxAmmo) currentAmmo = maxAmmo;

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
    public bool hasCounterfeitCoin = false;
    private bool peashooterActive = false;

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
                rb2d.freezeRotation = true;
                rb2d.gravityScale = 0;
                break;
            case AttachmentEffect.MORE_AMMO:
                maxAmmo += (int)ammo;
                currentAmmo += (int)ammo;
                UpdateAmmoCount();
                break;
            case AttachmentEffect.COUNTERFEIT_COIN:
                hasCounterfeitCoin = true;
                break;
            case AttachmentEffect.PEASHOOTER:
                peashooterActive = true;
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
                    UpdateAmmoCount();
                    break;
                case AttachmentEffect.COUNTERFEIT_COIN:
                    hasCounterfeitCoin = false;
                    break;
                case AttachmentEffect.PEASHOOTER:
                    peashooterActive = false;
                    break;
                default:
                    break;
            }
        }
    }

    public void KilledSomething()
    {
        foreach (GameObject attachment in allAttachments)
        {
            attachment.GetComponent<Attachable>().kills++;
        }
    }

    public GameObject magazineDisplayPrefab;
    public GameObject magazineDisplayParent;

    public void SetMagazines(int amount)
    {
        magazines = amount;

        if (magazines > maxMagazines)
        {
            magazines = maxMagazines;
        }
        if (magazines < 0)
        {
            magazines = 0;
        }

        foreach (Transform child in magazineDisplayParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < magazines; i++)
        {
            Instantiate(magazineDisplayPrefab, magazineDisplayParent.transform);
        }
    }

    [Header("HealthBar")]
    public GameObject HealthBar;
    public Animator goodNightAnimator;

    public void OutOfBounds()
    {
        canControl = false;
        rb2d.simulated = false;
        StartCoroutine(SlowMusicToStop());
        goodNightAnimator.SetBool("shown", true);
    }

    public AudioSource musicAudioSource;
    
    public IEnumerator SlowMusicToStop()
    {
        while (musicAudioSource.pitch > 0)
        {
            musicAudioSource.pitch -=  Time.deltaTime / 6;
            yield return null;
        }

        musicAudioSource.Stop();
    }

    public void ToggleCameraMode()
    {
        if (currentCameraMode == CameraMode.CLOSE)
        {
            currentCameraMode = CameraMode.FAR;
            closeCamera.Priority = 0;
        }
        else if (currentCameraMode == CameraMode.FAR)
        {
            currentCameraMode = CameraMode.CLOSE;
            closeCamera.Priority = 10;
        }
    }

    public void ForceCloseCamera()
    {
        currentCameraMode = CameraMode.CLOSE;
        closeCamera.Priority = 10;
    }
}

public enum CameraMode {
    CLOSE,
    FAR
}