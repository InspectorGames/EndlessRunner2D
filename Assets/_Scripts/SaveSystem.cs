using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    public static void SaveGameRecord(GameRecord gameRecord)
    {
        BinaryFormatter formatter= new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameRecord.insp";
        FileStream stream = new FileStream(path, FileMode.Create);
        
        GameRecordData data = new GameRecordData(gameRecord);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameRecordData LoadGameRecord()
    {
        string path = Application.persistentDataPath + "/gameRecord.insp";
        Debug.Log("Loading from " + path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameRecordData data = new GameRecordData(new GameRecord(0));

            try
            {
                data = formatter.Deserialize(stream) as GameRecordData;
            }
            catch
            {
                Debug.LogError("Save file not found in " + path);
            }
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
