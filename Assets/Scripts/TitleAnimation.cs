using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationAmount = 8f;
    public float rotationSpeed = 2f;

    [Header("Scale")]
    public float scaleAmount = 0.08f;
    public float scaleSpeed = 2f;

    [Header("Bobbing")]
    public float bobAmount = 0.03f;
    public float bobSpeed = 2f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        float time = Time.time;

        // Rotate back and forth
        float rotation = Mathf.Sin(time * rotationSpeed) * rotationAmount;
        transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

        // Squash and stretch
        float scale = 1f + Mathf.Sin(time * scaleSpeed) * scaleAmount;
        transform.localScale = new Vector3(
            originalScale.x * scale,
            originalScale.y / scale,
            originalScale.z
        );

        // Slightly bob up and down
        float bob = Mathf.Sin(time * bobSpeed) * bobAmount;
        transform.localPosition = originalPosition +
            Vector3.up * bob;
    }
}