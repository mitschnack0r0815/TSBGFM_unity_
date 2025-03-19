using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using UnityEngine.UIElements;

public class DatabaseManager : MonoBehaviour
{
    private string apiUrl = "http://localhost:3001/api/";

    // Public property to store the characters array
    public Character[] Characters { get; private set; }

    public GameStatus GameStatus { get; private set; }

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
                GameStatus = HandleJsonResponseGame(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    // Example JSON response for GameStatus
    /*
    {
        "gameNumber": 15,
        "board": {
            "x": 10,
            "y": 10,
            "_id": "67dafe07e8ba2eb65d873d8b"
        },
        "chars": [
            {
                "position": {
                    "x": 0,
                    "y": 0
                },
                "_id": "67d7287d15edcc39a0ba4fd9",
                "name": "Elon",
                "life": 90,
                "armor": 2,
                "weapon": {
                    "name": "Axe",
                    "dice": "1W12",
                    "initiative": 1,
                    "type": "Axe",
                    "_id": "67d7287d15edcc39a0ba4fda"
                },
                "__v": 0
            },
            {
                "position": {
                    "x": 0,
                    "y": 1
                },
                "_id": "67d7287d15edcc39a0ba4fd6",
                "name": "Donald",
                "life": 120,
                "armor": 1,
                "weapon": {
                    "name": "Sword",
                    "dice": "2W6",
                    "initiative": 5,
                    "type": "Sword",
                    "_id": "67d7287d15edcc39a0ba4fd7"
                },
                "__v": 0
            }
        ],
        "_id": "67dafe07e8ba2eb65d873d8a",
        "createdAt": "2025-03-19T17:25:27.406Z",
        "__v": 0
    }
    */

    GameStatus HandleJsonResponseGame(string json)
    {
        GameStatus gameStatus = JsonUtility.FromJson<GameStatus>(json);
        return gameStatus;
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
                Characters = HandleJsonResponseCharacters(request.downloadHandler.text);
                IsDataLoaded = true;
            }
            else
            {
                Debug.LogError("Request Failed: " + request.error);
            }
        }
    }

    Character[] HandleJsonResponseCharacters(string json)
    {
        // Wrap the JSON array in a class to deserialize it
        string wrappedJson = "{\"Characters\":" + json + "}";
        CharacterList characterList = JsonUtility.FromJson<CharacterList>(wrappedJson);

        return characterList.Characters;
    }
}

[Serializable]
public class GameStatus
{
    public string _id;
    public int gameNumber;
    public Board board;
    public Character[] chars;
    public DateTime createdAt;
    public int __v;
}

[Serializable]
public class Board
{
    public int x;
    public int y;
    public string _id;
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
    public BoardPos position;
    public int __v;
}

[Serializable]
public class BoardPos
{
    public int x;
    public int y;
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