using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private string dialogue;
    public AudioSource audioSource;
    private DialogueManager dialogueManager;
    public Transform cameraPosition;
    [System.NonSerialized]
    public bool hasBeenDetected = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueManager = GetComponent<DialogueManager>();
    }

    public void StartOwnDialogue()
    {
        audioSource.Play();
        dialogueManager.StartDialogue(dialogue);
    }

    public void SubscribeToDialogue(CameraController camera)
    {
        dialogueManager.Subscribe(camera);
    }

    public void UnsubscribeFromDialogue(CameraController camera)
    {
        dialogueManager.Unsubscribe(camera);
    }

    public void EndDialogueSounds()
    {
        audioSource.Stop();
    }
}
