using UnityEngine;

public class Bob : MonoBehaviour
{
    [Header("Bob Settings")]
    public float height = 0.15f;
    public float speed = 2f;

    private float baseY;

    private void Start()
    {
        baseY = transform.localPosition.y;
    }

    private void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y = baseY + Mathf.Sin(Time.time * speed) * height;
        transform.localPosition = pos;
    }
}