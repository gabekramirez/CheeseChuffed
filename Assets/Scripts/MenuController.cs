using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        ExecuteAfterTime(1f, () =>
        {
            SceneManager.LoadScene("CheeseMaker");
        });
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