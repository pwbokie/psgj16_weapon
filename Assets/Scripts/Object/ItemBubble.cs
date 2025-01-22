using UnityEngine;

public class ItemBubble : MonoBehaviour
{
    public GameObject contents;
    public ItemBubbleMovementType movementType = ItemBubbleMovementType.STATIC;
    
    private float floatyPushFrequency = 5f;

    private Rigidbody2D rb2d;

    private GameObject crate;
    private Rigidbody2D contentsRb2d;

    public void Start()
    {    
        rb2d = GetComponent<Rigidbody2D>();

        crate = transform.GetChild(0).gameObject;
        crate.GetComponent<LootCrate>().SetContents(contents);
        contentsRb2d = crate.GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        // If the movement type is FLOATY, apply a gentle force to the bubble every floatyPushPushFrequency seconds
        if (movementType == ItemBubbleMovementType.FLOATY)
        {
            if (Time.time % floatyPushFrequency < 0.01f)
            {
                rb2d.AddForce(new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f)));
            }

            // dampen movement
            rb2d.velocity *= 0.999f;
        }
    }

    public void Pop() {
        crate.transform.parent = null;
        contentsRb2d.simulated = true;

        gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
    }
}

public enum ItemBubbleMovementType
{
    STATIC,
    FLOATY,
    GRAVITY
}