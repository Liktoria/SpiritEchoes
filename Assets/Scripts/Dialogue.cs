using System;

[Serializable]
public class DialogueLine
{
    public string name;
    public string text;
    public int side;
}

[Serializable]
public class Dialogue
{
    public DialogueLine[] lines;
    public string name;
}

[Serializable]
public class Dialogues
{
    public Dialogue[] dialogues;
}