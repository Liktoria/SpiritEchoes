using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSecretArea : MonoBehaviour
{
    [SerializeField]
    private GameObject areaToHide;
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
            areaToHide.SetActive(false);
            if (areaNumber == 1)
            {
                gameManager.secretArea1Active = false;
            }
            else
            {
                gameManager.secretArea2Active = false;
            }
        }
    }
}
