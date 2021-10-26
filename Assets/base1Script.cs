using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class base1Script : MonoBehaviour {

    public KMSelectable[] Buttons;
    public TextMesh Screen;
    private int number = 0;
    private string oof = "";
    public KMBombModule Bomb;
    public KMAudio audio;
    int iteration = 0;
    bool funny = false;
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
        number = Rnd.Range(1,10);
        for(int i = 0; i < number; i++)
        {
            oof += "1";
        }
        Screen.text = "";
        Debug.LogFormat("[Base-1 #{0}] The displayed number is {1} which in Base-1 is {2}", moduleId, number, oof);
        Bomb.OnActivate += ShowDisplay;
	}

    void ShowDisplay()
    {
        Screen.text = oof;
    }

    void OnPress(int help)
    {
        if (help + 1 == 10 && funny)
            return;
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[help].transform);
        Buttons[help].AddInteractionPunch();
        if (solved == false)
        {
            if (help + 1 == 10)
            {
                funny = true;
                StartCoroutine(ByeBye(funny));
                audio.PlaySoundAtTransform("meme", transform);
                Debug.LogFormat("[Base-1 #{0}] Pressed button ?????, woooooooooowooooooooo", moduleId);
            }
            else if (help + 1 == number)
            {
                Bomb.HandlePass();
                solved = true;
                Debug.LogFormat("[Base-1 #{0}] Pressed button {1}, module solved", moduleId, help + 1);
            }
            else
            {
                Bomb.HandleStrike();
                Debug.LogFormat("[Base-1 #{0}] Pressed button {1}, strike", moduleId, help + 1);
            }
        }
    }

    IEnumerator ByeBye(bool seeYou)
    { 
        if (iteration == 150)
        {
            Buttons[9].gameObject.SetActive(false);
        }
        if (seeYou == true)
        {
            iteration++;
            Buttons[9].transform.localPosition += new Vector3(0f,0.01f,0f);
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(ByeBye(seeYou));
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <label> [Presses the button with the specified label]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                string[] labels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "?????" };
                if (!labels.Contains(parameters[1]) || (parameters[1].Equals("?????") && funny))
                {
                    yield return "sendtochaterror!f The specified label '" + parameters[1] + "' is not on any buttons!";
                    yield break;
                }
                yield return null;
                Buttons[Array.IndexOf(labels, parameters[1])].OnInteract();
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the label of a button to press!";
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        Buttons[number - 1].OnInteract();
        yield return new WaitForSeconds(.1f);
    }

}