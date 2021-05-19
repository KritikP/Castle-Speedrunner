using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectsList", menuName = "Tile Objects List", order = 51)]
public class TileObjectsList : ScriptableObject
{
    public Dictionary<string, List<TileObjects>> tileObjects;
    public Dictionary<string, List<TileObjects>> groundedTileObjects;

    private List<string> baseNames = new List<string>();
    private List<string> groundedBaseNames = new List<string>();

    private string baseName;
    
    private string randName;
    private System.Random rand = new System.Random();

    private void OnEnable()
    {
        tileObjects = new Dictionary<string, List<TileObjects>>();
        groundedTileObjects = new Dictionary<string, List<TileObjects>>();

        foreach (TileObjects obj in Resources.LoadAll<TileObjects>("Tile Objects/"))
        {
            baseName = obj.name.Substring(0, obj.name.LastIndexOf('_'));

            //If not grounded, add to regular dictionary
            if (!obj.isGrounded)
            {
                if (!tileObjects.ContainsKey(baseName))
                {
                    tileObjects.Add(baseName, new List<TileObjects>());
                    baseNames.Add(baseName);
                    //Debug.Log("Created type '" + baseName + "'");
                }

                tileObjects[baseName].Add(obj);
                //Debug.Log("Added " + "'" + obj.name + "'" + "to type '" + baseName + "'");
            }
            //If grounded, add to grounded dictionary
            else
            {
                if (!groundedTileObjects.ContainsKey(baseName))
                {
                    groundedTileObjects.Add(baseName, new List<TileObjects>());
                    groundedBaseNames.Add(baseName);
                    //Debug.Log("Created grounded type '" + baseName + "'");
                }

                groundedTileObjects[baseName].Add(obj);
                //Debug.Log("Added grounded object" + "'" + obj.name + "'" + "to type '" + baseName + "'");
            }

        }

    }

    public TileObjects GetRandomTileObject()
    {
        randName = baseNames[rand.Next(0, baseNames.Count)];
        if (tileObjects.Count > 0)
            return tileObjects[randName][rand.Next(0, tileObjects[randName].Count)];
        else
            return null;
    }

    public TileObjects GetRandomTileObject(string baseName)
    {
        if (tileObjects.ContainsKey(baseName))
        {
            return tileObjects[baseName][rand.Next(0, tileObjects[baseName].Count)];
        }
        else
        {
            //Debug.Log("Object type " + name + " not found.");
            return null;
        }
    }

    public TileObjects GetRandomGroundedTileObject()
    {
        randName = groundedBaseNames[rand.Next(0, groundedBaseNames.Count)];
        if (groundedTileObjects.Count > 0)
            return groundedTileObjects[randName][rand.Next(0, groundedTileObjects[randName].Count)];
        else
            return null;
    }

    public TileObjects GetRandomGroundedTileObject(string baseName)
    {
        if (groundedTileObjects.ContainsKey(baseName))
        {
            return groundedTileObjects[baseName][rand.Next(0, groundedTileObjects[baseName].Count)];
        }
        else
        {
            //Debug.Log("Object type " + name + " not found.");
            return null;
        }
    }
}