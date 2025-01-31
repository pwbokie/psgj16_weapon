using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int price;

    public TextMeshProUGUI helpText;
    public bool canActivate = false;

    private PlayerController player;

    public void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        UpdateText();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.tag == "Player")
        {
            helpText.gameObject.SetActive(true);
            canActivate = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            helpText.gameObject.SetActive(false);
            canActivate = false;
        }
    }

    public void Update()
    {
        if (canActivate && Input.GetKeyDown(KeyCode.E))
        {
            if (player.money >= price)
            {
                player.SetMoney(player.money - price);
                player.NextLevel();
            }
            else if (player.money < price)
            {
                StartCoroutine(ShowNotEnoughMoneyMessage());
            }
        }
    }

    private IEnumerator ShowNotEnoughMoneyMessage()
    {
        helpText.text = "the portal rejects you for being too poor!";
        yield return new WaitForSeconds(2);
        UpdateText();
    }

    public void UpdateText()
    {
        helpText.text = "[E] <color=#6AE034>$" + price.ToString() + "</color>";
    }
}