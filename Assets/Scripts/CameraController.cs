using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform followTransform;
    [SerializeField]
    private float smoothSpeed = 0.5f;
    private Transform targetPosition;
    private Vector3 originalPosition;
    [System.NonSerialized]
    public bool collectibleDetected = false;
    private Collectible detectedCollectible;
    private bool dialogueDone = false;
    private bool coroutineStarted = false;
    private float duration = 1.5f;

    [SerializeField] 
    CameraBounds2D bounds;
    Vector2 maxXPositions, maxYPositions;

    void Awake()
    {
        bounds.Initialize(GetComponent<Camera>());
        maxXPositions = bounds.maxXlimit;
        maxYPositions = bounds.maxYlimit;
    }

    void LateUpdate()
    {        
        if (collectibleDetected)
        {
            if (!coroutineStarted)
            {
                StartCoroutine(MoveToCollectible());
                coroutineStarted = true;
            }
        }
        else
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(Mathf.Clamp(followTransform.position.x, maxXPositions.x, maxXPositions.y), Mathf.Clamp(followTransform.position.y, maxYPositions.x, maxYPositions.y), currentPosition.z);
            this.transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * smoothSpeed);
        }        
    }
    
    public void DialogueDone()
    {
        dialogueDone = true;
        detectedCollectible.EndDialogueSounds();
    }

    public void CollectibleFound(Collectible collectible)
    {
        originalPosition = this.transform.position;        
        detectedCollectible = collectible;
        detectedCollectible.hasBeenDetected = true;
        targetPosition = detectedCollectible.cameraPosition;
        detectedCollectible.SubscribeToDialogue(this);
        collectibleDetected = true;
    }

    //This is started from the Update() function when a blob is found to move the camera to the desired position
    IEnumerator MoveToCollectible()
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition.position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition.position;

        //Start the dialogue and wait until it is completed
        detectedCollectible.StartOwnDialogue();
        yield return new WaitUntil(() => dialogueDone);

        if(detectedCollectible.gameObject.name == "CollectiblePurple1")
        {
            float velocityX = detectedCollectible.gameObject.GetComponent<Rigidbody2D>().velocity.x - 3.0f;
            float velocityY = detectedCollectible.gameObject.GetComponent<Rigidbody2D>().velocity.y;
            detectedCollectible.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2 (velocityX, velocityY);
            yield return new WaitForSeconds(1.0f);
            detectedCollectible.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, - 10.0f);
            StartCoroutine(MoveToPlayer());
        }
        else
        {
            //Start the movement back 
            StartCoroutine(MoveToPlayer());
        }
        
    }

    IEnumerator MoveToPlayer()
    {
        Debug.Log("Move back");
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, originalPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        //resetting everything for the next time I want the camera to move somewhere
        collectibleDetected = false;
        dialogueDone = false;
        coroutineStarted = false;
        detectedCollectible.UnsubscribeFromDialogue(this);
        if(detectedCollectible.gameObject.name == "CollectiblePurple1")
        {
            Destroy(detectedCollectible.gameObject);
        }
        detectedCollectible = null;
    }
}
