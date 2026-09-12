using UnityEngine;

public class GameplayLoop : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] private UIController uIController;
    [SerializeField] private OrderManager orderManager;

    void Start()
    {
        //First part of game loop. Initialize the cheese we're using
        orderManager.ongoingCheese = orderManager.FetchRandomCheese();
        orderManager.ongoingCheeseAttempt = new CheeseAttempt();

        uIController.SetCurrentOrderUI();
        uIController.RefreshCheeseAttemptsUI();
    }
}
