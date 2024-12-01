using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintedTouchGameManager : MonoBehaviour
{
    [SerializeField] Switch[] switchesObj;

    [SerializeField] Animator[] platformsToMove;

    readonly int[] permutationKey = { 1, 0, 2 };

    [SerializeField] List<int> currSelectedSwitches = new List<int>();

    bool allSwitchOn = false;
    bool sfxPlayed = false;

    private void Update()
    {
        CheckSwitch();
    }

    void CheckSwitch()
    {
        allSwitchOn = true;
        for (int i = 0; i < switchesObj.Length; i++)
        {
            if (!switchesObj[i].GetComponent<Switch>().isOn)
            {
                allSwitchOn = false;
                RemoveSelectedSwitchFromList(i);
                RaisePlatform(i, "SwitchIsOn", switchesObj[i].GetComponent<Switch>().isOn);
                continue;
            }

            AddToSelectedSwitchList(i);
            RaisePlatform(i, "SwitchIsOn", switchesObj[i].GetComponent<Switch>().isOn);
        }

        if (allSwitchOn && !sfxPlayed)
        {
            RaisePlatform(3, "SwitchIsOn", allSwitchOn);
            AudioManager.instance.PlaySFX("CorrectPuzzle");
            sfxPlayed = true;
        }
    }

    void RemoveSelectedSwitchFromList(int index)
    {
        currSelectedSwitches.Remove(index);
    }

    void AddToSelectedSwitchList(int index)
    {
        if (currSelectedSwitches.Contains(index))
        {
            return;
        }

        if (currSelectedSwitches.Count < 3)
        {
            if (!CheckIfCorrectPattern())
            {
                ResetSwitches();
                currSelectedSwitches.Clear();
                return;
            }
            currSelectedSwitches.Add(index);
            return;
        }

        currSelectedSwitches.Clear();
        currSelectedSwitches.Add(index);
    }

    bool CheckIfCorrectPattern()
    {
        for (int i = 0; i < currSelectedSwitches.Count; i++)
        {
            if (currSelectedSwitches[i] != permutationKey[i])
            {
                return false;
            }
        }
        return true;
    }



    void ResetSwitches()
    {
        for (int i = 0; i < switchesObj.Length; i++)
        {
            switchesObj[i].Reset();
        }
    }

    void RaisePlatform(int platformIndexToRaise, string animatorParameterName, bool isOn)
    {
        platformsToMove[platformIndexToRaise].SetBool(animatorParameterName, isOn);
    }
}
