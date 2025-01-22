using UnityEngine;

public class ItemBubble : MonoBehaviour
{
    public float contentsRotationSpeed = 1f;
    public GameObject contents;
    private GameObject crate;
    private Rigidbody2D contentsRb2d;

    public void Start()
    {    
        crate = transform.GetChild(0).gameObject;
        crate.GetComponent<LootCrate>().SetContents(contents);
        contentsRb2d = crate.GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        crate.transform.Rotate(0, 0, contentsRotationSpeed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Pop();
        }
    }

    public void Pop() {
        crate.transform.parent = null;
        contentsRb2d.bodyType = RigidbodyType2D.Dynamic;

        gameObject.GetComponent<ShadowedObject>().DestroyThisAndItsShadow();
    }
}