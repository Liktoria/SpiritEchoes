using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleDetector : MonoBehaviour
{
    [SerializeField]
    private CameraController cameraController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Collectible" && !other.gameObject.GetComponent<Collectible>().hasBeenDetected)
        {
            Debug.Log("Collectible found");
            cameraController.CollectibleFound(other.gameObject.GetComponent<Collectible>());
        }
    }
}
