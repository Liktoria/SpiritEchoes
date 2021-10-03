using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is based off a forum entry by https://forum.unity.com/members/losingisfun.1125637/
public class TransitionMusic : MonoBehaviour
{
    //We create an array with 2 audio sources that we will swap between for transitions
    public AudioSource[] aud = new AudioSource[6];
    public AudioSource endSource;
    //We will use this boolean to determine which audio source is the current one
    private int currentMusicSource = 0;
    private int nextSource;
    bool activeMusicSource;
    //We will store the transition as a Coroutine so that we have the ability to stop it halfway if necessary
    IEnumerator musicTransition;

    public void NextSoundtrack()
    {
        nextSource = currentMusicSource < aud.Length - 1 ? currentMusicSource + 1 : 0;

        //If a transition is already happening, stop it here to prevent new Coroutine from competing
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }

        aud[nextSource].Play();

        musicTransition = Transition(20); //20 is the equivalent to 2 seconds (More than 3 seconds begins to overlap for a bit too long)
        StartCoroutine(musicTransition);
    }

    //  'transitionDuration' is how many tenths of a second it will take, eg, 10 would be equal to 1 second
    IEnumerator Transition(int transitionDuration)
    {

        for (int i = 0; i < transitionDuration + 1; i++)
        {
            aud[currentMusicSource].volume = (transitionDuration - i) * (1f / transitionDuration);
            aud[nextSource].volume = (0 + i) * (1f / transitionDuration);

            yield return new WaitForSecondsRealtime(0.1f);
        }

        //finish by stopping the audio clip on the now silent audio source
        aud[currentMusicSource].Stop();

        currentMusicSource = nextSource;
        musicTransition = null;
    }


}
