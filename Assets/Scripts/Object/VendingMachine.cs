using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public int basePrice;
    private int totalPrice;
    public int stock = 3;

    public TextMeshProUGUI helpText;

    public GameObject vendedItem;

    public GameObject vendPreview;

    private bool canVend = true;

    private PlayerController player;

    public void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        UpdatePrice();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.tag == "Player")
        {
            helpText.gameObject.SetActive(true);
            canVend = true;

            UpdatePrice();
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
            if (stock > 0 && player.money >= totalPrice)
            {
                stock--;
                player.SetMoney(player.money - totalPrice);
                Instantiate(vendedItem, transform.position + new Vector3(0, -1f, 0), Quaternion.identity, transform.Find("/PlayMode"));
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                GetComponent<AudioSource>().Play();
                if (stock <= 0)
                {
                    vendPreview.SetActive(false);
                    helpText.text = "out of stock!";
                }
            }
            else if (player.money < totalPrice)
            {
                StopAllCoroutines();
                StartCoroutine(ShowNotEnoughMoneyMessage());
            }
        }
    }

    private IEnumerator ShowNotEnoughMoneyMessage()
    {
        helpText.text = "not enough money!";
        yield return new WaitForSeconds(2);
        UpdatePrice();
    }

    public void UpdatePrice()
    {
        totalPrice = basePrice;
        if (player.hasCounterfeitCoin)
        {
            totalPrice = basePrice - 1;
            if (totalPrice < 0)
            {
                totalPrice = 0;
            }
        }
        helpText.text = "[E] <color=#6AE034>$" + totalPrice.ToString() + "</color>";
    }
}

public enum VendorType
{
    Attachment,
    Ammo
}