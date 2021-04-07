using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectsList", menuName = "Tile Objects List", order = 51)]
public class TileObjectsList : ScriptableObject
{
    public Dictionary<string, TileObjects> tileObjectsDictionary;

    private void OnEnable()
    {
        tileObjectsDictionary = new Dictionary<string, TileObjects>();
        TileObjects[] temp;
        temp = Resources.LoadAll<TileObjects>("Tile Objects/");
        foreach(TileObjects obj in temp)
        {
            tileObjectsDictionary.Add(obj.name, obj);
        }

    }
}
