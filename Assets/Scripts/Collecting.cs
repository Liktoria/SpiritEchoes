using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Collecting : MonoBehaviour
{
    SpriteRenderer playerSprite;
    [SerializeField]
    private Image fadeImage;
    private AudioSource audioSource;
    private Color imageColor;
    [SerializeField]
    private TransitionMusic musicManager;
    private bool isFading = false;
    [SerializeField]
    GameObject collectiblePurple1;
    [SerializeField]
    GameObject collectiblePurple2;
    [SerializeField]
    GameObject secretArea1;
    [SerializeField]
    GameObject secretArea2;
    [SerializeField]
    GameObject secretWall1;
    [SerializeField]
    GameObject secretWall2;
    [SerializeField]
    GameObject secretWall3;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        imageColor = fadeImage.color;
        playerSprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Collectible")
        {
            //reaction to collect
            audioSource.PlayOneShot(audioSource.clip, 0.5f);

            musicManager.NextSoundtrack();
            
            if(collider.gameObject.name == "CollectibleYellow")
            {
                playerSprite.color = Color.yellow;
                //all the purple stuff and so on
                //remove ground at tree trunk
                Destroy(GameObject.Find("TreeTrunkGround"));
                collectiblePurple1.SetActive(true);
                collectiblePurple2.SetActive(true);

                secretArea1.SetActive(true);
                secretArea2.SetActive(true);
                Destroy(secretWall1);
                Destroy(secretWall2);
                Destroy(secretWall3);
            }
            else if(collider.gameObject.name == "CollectibleRed")
            {
                playerSprite.color = Color.red;
            }
            else if (collider.gameObject.name == "CollectibleBlue")
            {
                playerSprite.color = Color.blue;
            }
            else if (collider.gameObject.name == "CollectibleGreen")
            {
                playerSprite.color = Color.green;
            }
            else if (collider.gameObject.name == "CollectiblePurple2")
            {
                playerSprite.color = Color.magenta;
                Application.Quit();
                //end menu
            }

            if (!isFading)
            {
                Destroy(collider.gameObject);
                isFading = true;
                StartCoroutine(fadeOverlay(imageColor.a, imageColor.a - 20.0f, 1.0f, collider));
            }
            //overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, overlay.color.a - 20.0f);
        }
    }

    IEnumerator fadeOverlay(float startValue, float endValue, float duration, Collider2D collider)
    {
        float time = 0;
        imageColor.a = startValue;

        while (time < duration)
        {
            imageColor.a = Mathf.Lerp(startValue, endValue, time / duration);
            fadeImage.color = imageColor;

            time += Time.deltaTime;
            yield return null;
        }
        imageColor.a = endValue;
        fadeImage.color = imageColor;
        isFading = false;
    }
}
