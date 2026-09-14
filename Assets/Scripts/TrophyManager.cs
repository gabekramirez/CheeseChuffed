using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TrophyManager : MonoBehaviour
{
    [Header("Statistics")]
    [SerializeField] private TMP_Text cheesesMadeTXT;
    [SerializeField] private TMP_Text averageAttemptsTXT;
    [SerializeField] private TMP_Text timeTakenTXT;

    [Header("Trophy")]
    [SerializeField] private TMP_Text trophyTXT;
    [SerializeField] private GameObject trophyPage;
    public List<string> trophies;

    void Start()
    {
        trophyTXT.text = GetTrophyName();
        ExecuteAfterTime(8f, () => OpenStatisticsPage());
    }

    string GetTrophyName()
    {
        int index = Mathf.RoundToInt(OrderManager.endData.averageAttempts);
        if(OrderManager.endData.averageAttempts>12)
            index = 12;
        
        return trophies[index];
    }
    
    void OpenStatisticsPage()
    {
        cheesesMadeTXT.text = OrderManager.endData.cheesesCompleted.ToString("N0");
        averageAttemptsTXT.text = OrderManager.endData.averageAttempts.ToString("N0");
        timeTakenTXT.text = string.Format("{0:00}:{1:00}:{2:00}",
            (int)OrderManager.endData.hours,
            (int)OrderManager.endData.minutes,
            (int)OrderManager.endData.seconds);
    }

    public void ExecuteAfterTime(float time, Action action)
    {
        StartCoroutine(ExecuteAfterTimeCoroutine(time, action));
    }

    private IEnumerator ExecuteAfterTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

}
