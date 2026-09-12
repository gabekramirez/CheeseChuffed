using UnityEngine;
using System;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [Header("Lists")]
    [HideInInspector]
    public List<CheeseAttempt> previousCheeseAttempts;
    public List<Cheese> cheeses;
    [HideInInspector]
    public List<string> usedCheeses;

    [Header("Gameplay Acts")]
    [HideInInspector]
    public static Cheese ongoingCheese;

    [HideInInspector]
    public static CheeseAttempt ongoingCheeseAttempt;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        usedCheeses = new List<string>();
    }


    public Cheese FetchRandomCheese()
    {
        List<Cheese> remainingCheeses = new List<Cheese>();
        foreach(Cheese cheese in cheeses)
        {
            if (!usedCheeses.Contains(cheese.name))
            {
                remainingCheeses.Add(cheese);   
            }
        }

        if(remainingCheeses.Count == 0)
            return null;

        return remainingCheeses[UnityEngine.Random.Range(0, remainingCheeses.Count)];
    }


}

[System.Serializable]
public class Cheese
{
    public string name;
    public string description;

    public int saltiness;
    public int stinkiness;
    public int dryness;

}


[System.Serializable]
public class CheeseAttempt
{
    public string attemptName;

    [Header("Numbers")]
    public int amountOfSalt;
    public int amountOfCulture;
    public int amountOfAge;

    [Header("Recieved Feedback")]
    public string saltFeedback;
    public string cultureFeedback;
    public string ageFeedback;
}