using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class DataSystem
{
    public static void SaveData( DataProcessing DtP)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path =  Application.persistentDataPath+"/Data.fun";

        FileStream stream =new FileStream(path,FileMode.Create);
        
        DataStorage DtS = new DataStorage(DtP);
        formatter.Serialize(stream,DtS);
        stream.Close();
    }

    public static DataStorage LoadData()
    {
        string path = Application.persistentDataPath+"/Data.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);

            DataStorage DtS = formatter.Deserialize(stream) as DataStorage;
            stream.Close();

            return DtS;
        }
        else
        {
            Debug.Log("404");
            return null;
        }
    }
}
