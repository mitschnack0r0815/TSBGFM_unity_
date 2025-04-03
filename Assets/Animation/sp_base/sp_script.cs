using UnityEngine;

public class Sp_script : MonoBehaviour
{
    void Start()
    {
        
    }

    // Called in the Editor when the script is recompiled or values are changed
    void OnValidate()
    {
        GetSpecificSprites("front");
    }

    private void GetSpecificSprites(string childFolder)
    {
        // ListChildNames();

        foreach (Transform child in transform)
        {
            var spriteRenderers = child.GetComponentsInChildren<SpriteRenderer>();

            // Check if the child's name is not "side" (case-insensitive and trimmed)
            if (!string.Equals(child.name.Trim(), childFolder, System.StringComparison.OrdinalIgnoreCase))
            {
                // Get all SpriteRenderer components in the child and its nested children
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.enabled = false; // Make all sprites invisible
                }
            } else {
                // If the child is named "side", we want to keep it visible
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.enabled = true; // Make all sprites visible
                }
            }
        }
    }

    private void ListChildNames()
    {
        Debug.Log($"Listing all children of {transform.name}:");

        foreach (Transform child in transform)
        {
            Debug.Log($"Child Name: {child.name}");
        }
    }
}
