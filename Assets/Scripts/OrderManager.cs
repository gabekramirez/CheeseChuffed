using UnityEngine;
using System;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [Header("Lists")]
    [HideInInspector]
    public static List<CheeseAttempt> previousCheeseAttempts;
    public static List<Cheese> cheeses;
    public List<Cheese> initializedCheeses;
    public static bool shouldAssignNewCheese=true;
    public static EndData endData;


    [HideInInspector]
    public static List<string> usedCheeses;

    [Header("Gameplay Acts")]
    [HideInInspector]
    public static Cheese ongoingCheese;

    [HideInInspector]
    public static CheeseAttempt ongoingCheeseAttempt;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        usedCheeses = new List<string>();

        OrderManager.cheeses = initializedCheeses;
      
    }

    void Update()
    {
        OrderManager.endData.seconds += Time.deltaTime;
        if (OrderManager.endData.seconds >= 60)
        {
            OrderManager.endData.minutes ++;
            OrderManager.endData.seconds = 0;
            if (OrderManager.endData.minutes >= 60)
            {
                OrderManager.endData.hours++;
                OrderManager.endData.minutes = 0;
            }
        }
    }

 
    public static Cheese FetchRandomCheese()
    {
        List<Cheese> remainingCheeses = new List<Cheese>();
        foreach(Cheese cheese in OrderManager.cheeses)
        {
            if (!OrderManager.usedCheeses.Contains(cheese.name))
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

    // Creates and returns a new independent copy of this attempt
    public CheeseAttempt Copy()
    {
        return new CheeseAttempt
        {
            attemptName = this.attemptName,
            amountOfSalt = this.amountOfSalt,
            amountOfCulture = this.amountOfCulture,
            amountOfAge = this.amountOfAge,
            saltFeedback = this.saltFeedback,
            cultureFeedback = this.cultureFeedback,
            ageFeedback = this.ageFeedback
        };
    }
}



[System.Serializable]
public class EndData
{
    [Header("Time")]
    public float hours;
    public float minutes;
    public float seconds;

    [Header("CheeseData")]
    public int cheesesCompleted;
    public float averageAttempts;
}