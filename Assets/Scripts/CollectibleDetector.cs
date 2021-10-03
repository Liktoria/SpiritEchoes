using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleDetector : MonoBehaviour
{
    [SerializeField]
    private CameraController cameraController;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Collectible" && !other.gameObject.GetComponent<Collectible>().hasBeenDetected)
        {
            Debug.Log("Collectible found");
            cameraController.collectibleFound(other.gameObject.GetComponent<Collectible>());
        }
    }
}
