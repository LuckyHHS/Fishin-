using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.LookDev;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    //* This just controls data persistence.

    // PUBLICS
    [Header("Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public static DataPersistanceManager instance {get; private set;}

    // PRIVATES
    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;
    private int currentLoadIndex = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLoadIndex++;
        if (currentLoadIndex > 2)
        {
            // Load.
            LoadGame();
        }
        
    }

    public void LoadGame()
    {
        // Get data persists.
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();

        if (dataHandler == null) { dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption); }
        // Load any saved data.
        this.gameData = dataHandler.Load();
        
        // If not data create new game.
        if (this.gameData == null)
        {
            Debug.Log("[DATA] : No data was found.");
            NewGame();
        }

        // Push loaded data to scripts that need it.
        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }

        Debug.Log("[DATA] : Loaded.");
    }

    public void SaveGame()
    {
        // Pass the data to other script
        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }

        // Save that data to file
        dataHandler.Save(gameData);

        Debug.Log("[DATA] : Saved.");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistances = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistances);
    }
}
