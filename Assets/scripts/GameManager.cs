using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Player;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    public List<string> AllGameNames = new List<string>();

    [SerializeField] GameData _gameData;
    PlayerInputManager _playerInputManager;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputManager= GetComponent<PlayerInputManager>();
        GetComponent<PlayerInputManager>().onPlayerJoined += HandlePlayerJoined;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        String commaSeperatedList = PlayerPrefs.GetString("AllGameNames");
        Debug.Log(commaSeperatedList);
        AllGameNames = commaSeperatedList.Split(',').ToList();
        AllGameNames.Remove("");
    }

    void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        else
        {
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            SaveGame();
        }
        
    }

    private void SaveGame()
    {
        string text = JsonUtility.ToJson(_gameData);
        Debug.Log(text);

        PlayerPrefs.SetString(_gameData.GameName, text);

        if (AllGameNames.Contains(_gameData.GameName) == false)
        AllGameNames.Add(_gameData.GameName);

        string commaSeperatedGameNames = string.Join(",", AllGameNames);

        PlayerPrefs.SetString("AllGameNames", commaSeperatedGameNames);
        PlayerPrefs.Save();
    }

    public void LoadGame(String gameName)
    {
        string text = PlayerPrefs.GetString(gameName);
        _gameData = JsonUtility.FromJson<GameData>(text);
        SceneManager.LoadScene("Level1");
    }

    void HandlePlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("HandlePlayerJoined " + playerInput);
        PlayerData playerData = GetPlayerData(playerInput.playerIndex);

        Player player = playerInput.GetComponent<Player>();
        player.Bind(playerData);
    }

    private PlayerData GetPlayerData(int playerIndex)
    {
        if(_gameData.PlayerDatas.Count <= playerIndex)
        {
            var playerData = new PlayerData();
            _gameData.PlayerDatas.Add(playerData);
        }
        return _gameData.PlayerDatas[playerIndex];
    }

    public void NewGame()
    {
        Debug.Log("New game called");
        _gameData = new GameData();
        _gameData.GameName = DateTime.Now.ToString("G");
        SceneManager.LoadScene("Level1");
    }

    public void DeleteGame(string gameName)
    {
        PlayerPrefs.DeleteKey(gameName);
        AllGameNames.Remove(gameName);

        string commaSeperatedGameNames = string.Join(",", AllGameNames);

        PlayerPrefs.SetString("AllGameNames", commaSeperatedGameNames);
        PlayerPrefs.Save();
    }
}