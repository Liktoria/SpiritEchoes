using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private List<CameraController> subscribers = new List<CameraController>();

    [SerializeField]
    private float timeBetweenCharacters = 0.1f;
    private float timer = 0.0f;
    private int characterIndex = 0;
    private bool isDoneShowingLine = true;
    private Collectible attachedCollectible;

    private bool IsDoneShowingLine
    {
        get { return isDoneShowingLine; }
        set
        {
            isDoneShowingLine = value;
            continueArrow.SetActive(value);
        }
    }

    [SerializeField]
    private Sprite[] dialogueSprites;
    // All fields for controlling the UI
    [SerializeField]
    private GameObject dialogueBox;
    private TMP_Text speaker;
    private TMP_Text content;
    private GameObject background;
    private GameObject continueArrow;

    // File with the Dialogues
    [SerializeField]
    private TextAsset jsonDialogues;

    private Dialogue[] dialogues;
    public Dialogue currentDialogue;
    private int currentLine;
    private int currentSide;
    private int currentBackgroundImage;


    void Start()
    {
        dialogueBox.SetActive(false);
        dialogues = JsonUtility.FromJson<Dialogues>(jsonDialogues.text).dialogues;

        speaker = dialogueBox.transform.Find("Name").GetComponent<TMP_Text>();
        content = dialogueBox.transform.Find("Content").GetComponent<TMP_Text>();
        background = dialogueBox.transform.Find("Background").gameObject;
        continueArrow = dialogueBox.transform.Find("ContinueArrow").gameObject;
        attachedCollectible = GetComponent<Collectible>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueBox.activeSelf && !IsDoneShowingLine)
        {            
            timer += Time.deltaTime;
            if (timer > timeBetweenCharacters)
            {
                string textToDisplay = currentDialogue.lines[currentLine].text;
                characterIndex++;
                if (characterIndex < textToDisplay.Length + 1)
                {
                    content.text = textToDisplay.Substring(0, characterIndex);
                    if (!attachedCollectible.audioSource.isPlaying)
                    {
                        attachedCollectible.audioSource.Play();
                    }
                }
                else
                {
                    characterIndex = 0;
                    IsDoneShowingLine = true;
                    attachedCollectible.audioSource.Pause();
                }
                timer = 0.0f;
            }
        }
    }

    public void StartDialogue(string dialogueName)
    {
        Dialogue[] filteredDialogues = dialogues.Where(d => d.name == dialogueName).ToArray();

        if (filteredDialogues.Length > 0)
        {
            dialogueBox.SetActive(true);
            currentDialogue = filteredDialogues[0];
            currentLine = 0;
            ChangeLine(currentDialogue.lines[0]);
        }
    }

    public void NextLine()
    {
        if (!IsDoneShowingLine)
        {
            IsDoneShowingLine = true;
            characterIndex = 0;
            content.text = currentDialogue.lines[currentLine].text;
            attachedCollectible.audioSource.Pause();
        }
        else if (dialogueBox.activeSelf)
        {
            currentLine += 1;
            if (currentDialogue.lines.Length > currentLine)
            {
                ChangeLine(currentDialogue.lines[currentLine]);
            }
            else
            {
                currentDialogue = null;
                currentLine = 0;
                dialogueBox.SetActive(false);
                characterIndex = 0;
                IsDoneShowingLine = true;
                /*
                if (!interactionManager.getInspectingSomething())
                {
                    interactionManager.setInteractableCollisions(true);
                } */
                NotifyAll();
            }
        }
    }

    private void ChangeLine(DialogueLine line)
    {
        if (line.side == 0)
        {
            this.currentSide = this.currentSide == 1 ? 2 : 1;
        }
        else
        {
            this.currentSide = line.side;
        }
        background.SetActive(true);
        speaker.text = line.name;
        content.text = "";

        IsDoneShowingLine = false;
    }

    
    public void Subscribe(CameraController subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(CameraController subscriber)
    {
        subscribers.Remove(subscriber);
    }

    private void NotifyAll()
    {
        foreach (CameraController subscriber in subscribers)
        {
            subscriber.DialogueDone();
        }
    }
}
