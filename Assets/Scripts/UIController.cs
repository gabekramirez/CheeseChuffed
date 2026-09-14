using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] private OrderManager orderManager;

    [Header("Current Order UI")]
    [SerializeField] private TMP_Text cheeseOrderedTXT;
    [SerializeField] private TMP_Text descriptionOrderedTXT;

    public void SetCurrentOrderUI()
    {
        cheeseOrderedTXT.text = OrderManager.ongoingCheese.name;
        descriptionOrderedTXT.text = $"\"{OrderManager.ongoingCheese.description}\"";
    }


    private string GetAttemptName(int index, int totalAttempts)
    {
        if (index == totalAttempts - 1)
            return "Latest";

        int attemptNumber = totalAttempts - index;

        string suffix = "th";

        if (attemptNumber % 100 < 11 || attemptNumber % 100 > 13)
        {
            switch (attemptNumber % 10)
            {
                case 1:
                    suffix = "st";
                    break;
                case 2:
                    suffix = "nd";
                    break;
                case 3:
                    suffix = "rd";
                    break;
            }
        }

        return $"{attemptNumber}{suffix} Attempt";
    }

    string GamePlayPhaseToString(GameplayPhase phase)
    {
        switch (phase)
        {
            case GameplayPhase.AddMilk:
                return "Add Milk";

            case GameplayPhase.TurnOnPot:
                return "Turn On Pot";

            case GameplayPhase.AddSalt:
                return "Add Salt";

            case GameplayPhase.AddCulture:
                return "Add Culture";

            case GameplayPhase.TurnOffPot:
                return "Turn Off Pot";

            case GameplayPhase.Age:
                return "Age the Cheese";

            case GameplayPhase.Present:
                return "Present the Cheese";

            default:
                return "";
        }
    }

    // Helper method to recursively delete all the children of a transform
    public void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DestroyAllChildren(child);
            Destroy(child.gameObject);
        }
    }
}
