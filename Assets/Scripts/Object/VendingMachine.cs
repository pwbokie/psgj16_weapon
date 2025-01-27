using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public int price;
    public int stock = 3;

    public TextMeshProUGUI helpText;

    public GameObject vendedItem;

    public GameObject vendPreview;

    private bool canVend = true;

    private PlayerController player;

    public void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        helpText.text = "[E] <color=#6AE034>$" + price.ToString() + "</color>";
    }

    public void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.tag == "Player")
        {
            helpText.gameObject.SetActive(true);
            canVend = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            helpText.gameObject.SetActive(false);
            canVend = false;
        }
    }

    public void Update()
    {
        if (canVend && Input.GetKeyDown(KeyCode.E))
        {
            if (stock > 0 && player.money >= price)
            {
                stock--;
                player.SetMoney(player.money - price);
                Instantiate(vendedItem, transform.position + new Vector3(0, -1f, 0), Quaternion.identity, transform.Find("/PlayMode"));
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                GetComponent<AudioSource>().Play();
                if (stock <= 0)
                {
                    vendPreview.SetActive(false);
                    helpText.text = "out of stock!";
                }
            }
            else if (player.money < price)
            {
                StopAllCoroutines();
                StartCoroutine(ShowNotEnoughMoneyMessage());
            }
        }
    }

    private IEnumerator ShowNotEnoughMoneyMessage()
    {
        string originalText = "[E] <color=#6AE034>$" + price.ToString() + "</color>";
        helpText.text = "not enough money!";
        yield return new WaitForSeconds(2);
        helpText.text = originalText;
    }
}

public enum VendorType
{
    Attachment,
    Ammo
}