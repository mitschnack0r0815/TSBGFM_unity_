using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class DatabaseManager : MonoBehaviour
{
    private string apiUrl = "http://localhost:3001/api/";

    // Public property to store the characters array
    public Character[] Characters { get; private set; }

    // Public variable to indicate if data is loaded
    public bool IsDataLoaded { get; private set; } = false;

    void Start()
    {
        StartCoroutine(GetCharacters());
        StartCoroutine(PostInitGame());
    }

    IEnumerator PostInitGame()
    {
        // Create JSON body
        string jsonBody = "{\"Player1\": \"Elon\", \"Player2\": \"Donald\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        // Create request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "initGame", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("DatabaseManager - PostInitGame\nResponse: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    IEnumerator GetCharacters()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "getCharacters"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("DatabaseManager - GetCharacters\nServer Response: " + request.downloadHandler.text);

                // Parse the JSON response
                Characters = HandleJsonResponse(request.downloadHandler.text);
                IsDataLoaded = true;
            }
            else
            {
                Debug.LogError("Request Failed: " + request.error);
            }
        }
    }

    Character[] HandleJsonResponse(string json)
    {
        // Wrap the JSON array in a class to deserialize it
        string wrappedJson = "{\"Characters\":" + json + "}";
        CharacterList characterList = JsonUtility.FromJson<CharacterList>(wrappedJson);

        return characterList.Characters;
    }
}

[Serializable]
public class CharacterList
{
    public Character[] Characters;
}

[Serializable]
public class Character
{
    public string _id;
    public string name;
    public int life;
    public int armor;
    public Weapon weapon;
    public int __v;
}

[Serializable]
public class Weapon
{
    public string _id;
    public string name;
    public string dice;
    public int initiative;
    public string type;
}