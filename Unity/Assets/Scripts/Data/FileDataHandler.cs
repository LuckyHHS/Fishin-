using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler : MonoBehaviour
{
    //* This writes and loads from the files.

    // PRivates
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string codeWord = "fiszpoiSgm";

    // PUBLICS
    public FileDataHandler(string dataDir, string dataFile, bool useEncryption)

    {
        this.useEncryption = useEncryption;
        this.dataDirPath = dataDir;
        this.dataFileName = dataFile;
    }

    public GameData Load()
    {
        // Use path.combine to work on multiple ssystem.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load serialized data.
                string dataToLoad = "";
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        dataToLoad = sr.ReadToEnd();
                    }
                }

                 // Encrypt if needed.
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Desliarze
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while loading from path " + fullPath + "\n " + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Use path.combine to work on multiple ssystem.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {   
            // Create the dir path
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize data
            string dataToStore = JsonUtility.ToJson(data, true);

            // Encrypt if needed.
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write the file to system.
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving to path " + fullPath + "\n " + e);
        }
    }

    // Encryption method.
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i <data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }
}
