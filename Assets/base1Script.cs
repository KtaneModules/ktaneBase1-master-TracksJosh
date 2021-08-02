using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class base1Script : MonoBehaviour {

    public KMSelectable[] Buttons;
    public TextMesh Screen;
    private int number = 0;
    public KMBombModule Bomb;
    public KMAudio audio;
    int iteration = 0;
    bool solved = false;
    static int moduleIdCounter = 1;
    int moduleId;

    // Use this for initialization
    void Start () {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < Buttons.Length; i++)
        {
            int j = i;
            Buttons[i].OnInteract += delegate { OnPress(j); return false; };
        }
        string oof = "";
        number = Rnd.Range(1,10);
        for(int i = 0; i < number; i++)
        {
            oof += "1";
        }
        Screen.text = oof;
	}

    void OnPress(int help)
    {
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Buttons[help].AddInteractionPunch();
        if (solved == false)
        {
            if (help + 1 == 10)
            {
                bool happy = true;
                StartCoroutine(ByeBye(happy));
                audio.PlaySoundAtTransform("meme", transform);
            }
            else if (help + 1 == number)
            {
                Bomb.HandlePass();
                solved = true;
            }
            else
            {
                Bomb.HandleStrike();
            }
        }
        
    }

	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator ByeBye(bool seeYou)
    {
        
        if(iteration == 150)
        {
            Buttons[9].gameObject.SetActive(false);
        }
        if(seeYou == true)
        {
            iteration++;
            Buttons[9].transform.localPosition += new Vector3(0f,0.01f,0f);
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(ByeBye(seeYou));
        }
    }
}
