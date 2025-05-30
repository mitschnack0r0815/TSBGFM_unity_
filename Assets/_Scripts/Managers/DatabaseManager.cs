using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using UnityEngine.UIElements;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;

public class DatabaseManager : StaticInstance<DatabaseManager>
{
    [SerializeField]
    public bool OfflineMode = false;
    private string apiUrl = "http://localhost:3001/api/";

    // Public property to store the characters array
    // public Character[] Characters { get; private set; }

    public GameStatus GameStatus { get; private set; }

    // Public variable to indicate if data is loaded
    public bool IsDataLoaded { get; private set; } = false;

    void Start()
    {
        if (OfflineMode)
        {
            // If in offline mode, load the characters from the resource system
            // Characters = HandleJsonResponseCharacters(TestData.chars);
            GameStatus = HandleJsonResponseGame(TestData.gameStatus);
            IsDataLoaded = true;
        }
        else
        {
            // If not in offline mode, start the initialization coroutine
            StartCoroutine(InitializeGame());
        }


    }

    IEnumerator InitializeGame()
    {
        // Wait for GetCharacters coroutine to complete
        // yield return StartCoroutine(GetCharacters());

        // Wait for PostInitGame coroutine to complete
        yield return StartCoroutine(PostInitGame());

        // Optionally, set IsDataLoaded to true if both coroutines are successful
        IsDataLoaded = true;
    }

    IEnumerator PostInitGame()
    {
        // Create JSON body
        string jsonBody = "{\"Player1\": \"Donald\", \"Player2\": \"Elon\"}";
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
                Debug.Log("GameStatus: " + GameStatus.gameNumber);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    private string ExtractMapJson(string json)
    {
        // Extract the map JSON object from the provided JSON string
        int startIndex = json.IndexOf("\"map\":") + 6;
        int endIndex = json.IndexOf("]]", startIndex) + 2;
        string mapJson = json.Substring(startIndex, endIndex - startIndex);
        return  mapJson;
    }

    // Method to handle JSON response for GameStatus
    GameStatus HandleJsonResponseGame(string json)
    {      
        GameStatus gameStatus = JsonUtility.FromJson<GameStatus>(json);

        // HAndle the map object separately
        string mapJson = ExtractMapJson(json);
        int[][] map = new int[gameStatus.board.x][];
        for (int i = 0; i < gameStatus.board.x; i++)
        {
            map[i] = new int[gameStatus.board.y];
            for (int j = 0; j < gameStatus.board.y; j++)
            {
                map[i][j] = 0;
            }
        }
        
        int x = 0;
        int y = 0;
        foreach (char c in mapJson)
        {   
            if (char.IsDigit(c))
            {
                map[x][y] = (int)char.GetNumericValue(c);
                x++;
            }
            if (c == ']')
            {
                x = 0;
                y++;
            }
        }

        gameStatus.board.map = map;
        return gameStatus;
    }

    // IEnumerator GetCharacters()
    // {
    //     using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "getCharacters"))
    //     {
    //         yield return request.SendWebRequest();

    //         if (request.result == UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log("DatabaseManager - GetCharacters\nServer Response: " + request.downloadHandler.text);

    //             // Parse the JSON response
    //             Characters = HandleJsonResponseCharacters(request.downloadHandler.text);
    //         }
    //         else
    //         {
    //             Debug.LogError("Request Failed: " + request.error);
    //         }
    //     }
    // }

    // Character[] HandleJsonResponseCharacters(string json)
    // {
    //     // Wrap the JSON array in a class to deserialize it
    //     string wrappedJson = "{\"Characters\":" + json + "}";
    //     CharacterList characterList = JsonUtility.FromJson<CharacterList>(wrappedJson);

    //     return characterList.Characters;
    // }
}

[Serializable]
public class GameStatus
{
    public string _id;
    public int gameNumber;
    public Board board;
    public Player[] players;
    public int currentTurn;
    public string[] turnList;
    public DateTime createdAt;
    public int __v;
}

[Serializable]
public class Board
{
    public string _id;
    public int x;
    public int y;
    public int[][] map;
    public int __v;
}

[Serializable]
public class Player
/// <summary>
/// Represents a database entry for a character group controlled by the player.
/// </summary>
/// <remarks>
/// This class contains information about the group, including its ID, name, faction, 
/// list of characters, initiative value, extra dice, and version number.
/// </remarks>
/// <param name="_id">The unique identifier for the database entry</param>
/// <param name="name">The name of the player</param>
/// <param name="faction">The faction to which the character group belong.</param>
/// <param name="charList">The list of characters in the group/faction</param>
/// <param name="initiative">The initiative value</param>
/// <param name="extraDice">The number of extra dice available</param>
/// <param name="__v">The version number of the database entry</param>
{
    public string _id;
    public int id;
    public string playerName;
    public string faction; 
    public Unit[] units;
    public int initiative;
    public int extraDice;
    public int __v;
}

[Serializable]
public class Unit
{
    public string _id;
    public int id;
    public string name;
    public string faction; /* will be a unique player */
    public int life;
    public int armor;
    public Weapons weapons;
    public int moveDistance;
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
public class Weapons
{
    public Weapon first;
    public Weapon second;
}

[Serializable]
public class Weapon
{
    public string _id;
    public string name;
    public int strength;
    public int range;
    public int attacks;
    public int attackPower;
    public int critPower;

}