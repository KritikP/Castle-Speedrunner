using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectsList", menuName = "Tile Objects List", order = 51)]
public class TileObjectsList : ScriptableObject
{
    public Dictionary<string, List<TileObjects>> tileObjects;
    string baseName;
    private void OnEnable()
    {
        tileObjects = new Dictionary<string, List<TileObjects>>();

        foreach (TileObjects obj in Resources.LoadAll<TileObjects>("Tile Objects/"))
        {
            baseName = obj.name.Substring(0, obj.name.LastIndexOf('_'));

            if (!tileObjects.ContainsKey(baseName))
            {
                tileObjects.Add(baseName, new List<TileObjects>());
                Debug.Log("Created type '" + baseName + "'");
            }

            tileObjects[baseName].Add(obj);
            Debug.Log("Added " + "'" + obj.name + "'" + "to type '" + baseName + "'");

        }

    }
}