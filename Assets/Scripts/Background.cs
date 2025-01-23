using UnityEngine;

public class Background : MonoBehaviour
{
    public float driftSpeedX = 0.1f;
    public float driftSpeedY = 0.2f;
    private Vector2 drift;

    public int tilesWidth = 6;
    public int tilesHeight = 4;

    public GameObject cam;

    public GameObject tilePrefab;

    public Sprite sprite;
    public Color foregroundColor;

    public bool deactivateAfterInstantiating = false;
    private bool instantiated = false;

    public void Awake() {
        if (!instantiated) {
            instantiated = true;
            drift = new Vector2(driftSpeedX, driftSpeedY);
            // Spawn tiles to fill the visible background
            float spriteWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            float spriteHeight = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.y;

            for (int i=-(tilesWidth/2); i < tilesWidth/2; i++) {
                for (int j=-(tilesHeight/2); j < tilesHeight/2; j++) {
                    GameObject tile = Instantiate(tilePrefab, new Vector2(spriteWidth * i, spriteHeight * j), Quaternion.identity);
                    tile.transform.parent = transform;
                    tile.GetComponent<SpriteRenderer>().sprite = sprite;
                    tile.GetComponent<SpriteRenderer>().color = foregroundColor;
                    tile.GetComponent<SpriteRenderer>().sortingOrder = -10;
                }
            }
        }

        if (deactivateAfterInstantiating) {
            gameObject.SetActive(false);
        }
    }

    public void Update() {
        Vector2 newPosition = (Vector2)transform.position + drift * Time.deltaTime;

        // Define bounds relative to the camera
        float xMin = cam.transform.position.x - 10;
        float xMax = cam.transform.position.x + 10;
        float yMin = cam.transform.position.y - 5;
        float yMax = cam.transform.position.y + 5;

        // Wrap the X-axis position seamlessly
        if (newPosition.x > xMax) {
            newPosition.x = xMin + (newPosition.x - xMax);
        } else if (newPosition.x < xMin) {
            newPosition.x = xMax - (xMin - newPosition.x);
        }

        // Wrap the Y-axis position seamlessly
        if (newPosition.y > yMax) {
            newPosition.y = yMin + (newPosition.y - yMax);
        } else if (newPosition.y < yMin) {
            newPosition.y = yMax - (yMin - newPosition.y);
        }

        // Apply the new position
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}