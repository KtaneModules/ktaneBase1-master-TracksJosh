using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class base1Script : MonoBehaviour
{

    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMSelectable[] NumberButtons;
    public KMSelectable WhatTheFuck;
    public TextMesh ScreenText;
    public GameObject TheButtonThatDoesTheThing;
    public GameObject TheParentOfTheButtonThatDoesTheThing;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private int _answer;
    private string _text;
    private bool _canDoTheFunnyThing = true;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        _answer = Rnd.Range(1, 10);
        for (int i = 0; i < _answer; i++)
            _text += "1";
        ScreenText.text = _text;
        Debug.LogFormat("[Base-1 #{0}] Screen is {1}, which in Base-1 is {2}.", _moduleId, _text, _answer);

        for (int i = 0; i < NumberButtons.Length; i++)
            NumberButtons[i].OnInteract += NumberButtonPress(i);
        WhatTheFuck.OnInteract += WhatTheFuckPress;
    }

    private KMSelectable.OnInteractHandler NumberButtonPress(int button)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return false;
            if (button + 1 == _answer)
            {
                Module.HandlePass();
                _moduleSolved = true;
                Debug.LogFormat("[Base-1 #{0}] Pressed {1}. Cool.", _moduleId, button + 1);
            }
            else
            {
                Module.HandleStrike();
                Debug.LogFormat("[Base-1 #{0}] Pressed {1} instead of {2}. Literally how did you screw this up.", _moduleId, button + 1, _answer);
            }
            Debug.LogFormat("press");
            return false;
        };
    }

    private bool WhatTheFuckPress()
    {
        if (_canDoTheFunnyThing)
            StartCoroutine(WOOOOOOWOOOOOOOOOOOOOOOO());
        return false;
    }

    private IEnumerator WOOOOOOWOOOOOOOOOOOOOOOO()
    {
        Audio.PlaySoundAtTransform("WOOOOOWOOOOOOOOOOOOOO", transform);
        _canDoTheFunnyThing = false;
        var duration = 2f;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            TheButtonThatDoesTheThing.transform.localPosition = new Vector3(Easing.InOutQuad(elapsed, 0.06f, 0f, duration), Easing.InOutQuad(elapsed, 0.015f, 0.5f, duration), Easing.InOutQuad(elapsed, -0.04f, 0f, duration));
            TheButtonThatDoesTheThing.transform.localEulerAngles = new Vector3(0f, Easing.InQuad(elapsed, 0f, -1440f, duration), 0f);
            TheButtonThatDoesTheThing.transform.localScale = new Vector3(Easing.InOutQuad(elapsed, 0.125f, 0f, duration), Easing.InOutQuad(elapsed, 0.05f, 0f, duration), Easing.InOutQuad(elapsed, 0.125f, 0f, duration));
            TheParentOfTheButtonThatDoesTheThing.transform.localEulerAngles = new Vector3(0f, Easing.InQuad(elapsed, 0f, 720f, duration), 0f);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <label> [Presses the button with the specified label]";
#pragma warning restore 414
    private IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
                yield break;
            }
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the label of a button to press!";
                yield break;
            }
            if (parameters.Length == 2)
            {
                string[] labels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "???" };
                if (!labels.Contains(parameters[1]))
                {
                    yield return "sendtochaterror!f The specified label '" + parameters[1] + "' is not on any buttons!";
                    yield break;
                }
                yield return null;
                if (parameters[1] == "???")
                {
                    if (_canDoTheFunnyThing)
                    {
                        WhatTheFuck.OnInteract();
                        yield break;
                    }
                    else
                    {
                        yield return "sendtochaterror You already did the funny haha";
                        yield break;
                    }
                }
                else
                {
                    NumberButtons[int.Parse(parameters[1]) - 1].OnInteract();
                    yield break;
                }
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        NumberButtons[_answer - 1].OnInteract();
        yield break;
    }
}