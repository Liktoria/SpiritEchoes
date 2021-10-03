using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    //use this method to start a new soundtrack, with a reference to the AudioClip that you want to use
    //    such as:        newSoundtrack((AudioClip)Resources.Load("Audio/soundtracks/track01"));
    public void NextSoundtrack()
    {
        //This ?: operator is short hand for an if/else statement, eg.
        //
        //      if (activeMusicSource) {
        //          nextSource = 1;
        //      } else {
        //           nextSource = 0;
        //      }

        nextSource = currentMusicSource < aud.Length - 1 ? currentMusicSource + 1 : 0;

        //If a transition is already happening, we stop it here to prevent our new Coroutine from competing
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

            //  Here I have a global variable to control maximum volume.
            //  options.musicVolume is a float that ranges from 0f - 1.0f
            //------------------------------------------------------------//
            //aud[0].volume *= options.musicVolume;
            //aud[1].volume *= options.musicVolume;
            //------------------------------------------------------------//

            yield return new WaitForSecondsRealtime(0.1f);
            //use realtime otherwise if you pause the game you could pause the transition half way
        }

        //finish by stopping the audio clip on the now silent audio source
        aud[currentMusicSource].Stop();

        currentMusicSource = nextSource;
        musicTransition = null;
    }


}
