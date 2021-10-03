using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSecretArea : MonoBehaviour
{
    [SerializeField]
    private GameObject areaToShow;
    [SerializeField]
    private int areaNumber;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.GetInstance();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name != "CollectibleDetector")
        {
            areaToShow.SetActive(true);
            if (areaNumber == 1)
            {
                gameManager.secretArea1Active = true;
            }
            else
            {
                gameManager.secretArea2Active = true;
            }
        }
    }
}
