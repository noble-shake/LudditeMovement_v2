using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogForm
{
    public List<string> key;
    public List<string> dialog;
}

public static class DialogLibrary
{ 
    public static Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
    public static void Init(TextAsset _Dataset)
    {
        DialogForm LoadData = JsonConvert.DeserializeObject<DialogForm>(_Dataset.text);

        for (int index = 0; index < LoadData.key.Count; index++)
        {
            keyValuePairs[LoadData.key[index]] = LoadData.dialog[index];
        }

    }

    public static string GetValue(string key)
    { 
        if(keyValuePairs.ContainsKey(key)) return keyValuePairs[key];
        return null;

    }
}