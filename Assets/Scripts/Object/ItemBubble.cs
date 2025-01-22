using UnityEngine;

public class ItemBubble : MonoBehaviour
{

    public float contentsRotationSpeed = 1f;
    public GameObject contentsDisplay;

    public GameObject contentsPrefab;

    public void Awake() {
        contentsDisplay.GetComponent<SpriteRenderer>().sprite = contentsPrefab.GetComponent<SpriteRenderer>().sprite;
    }

    public void Update() {
        contentsDisplay.transform.position = transform.position;
        contentsDisplay.transform.Rotate(Vector3.forward * contentsRotationSpeed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(contentsPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
