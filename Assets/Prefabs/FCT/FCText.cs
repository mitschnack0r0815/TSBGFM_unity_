using UnityEngine;
using System.Collections;

public class FCText : MonoBehaviour
{
    public float DestroyTime = 3f; // Time in seconds before the object is destroyed
    public Vector3 OffsetPosition = new Vector3(0, 3f, 0); // Offset position for the text
    public Vector3 MoveDirection = new Vector3(0, 2f, 0); // Direction to move the text
    void Start()
    {
        Destroy(gameObject, DestroyTime); // Schedule the destruction of the object

        transform.position += OffsetPosition; // Apply the offset position
        StartCoroutine(MoveUpOverTime(MoveDirection));
    }

    private IEnumerator MoveUpOverTime(Vector3 moveDirection)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + moveDirection;

        while (elapsedTime < DestroyTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / DestroyTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Ensure the final position is set
    }
}
