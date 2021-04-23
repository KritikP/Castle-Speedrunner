using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map_Data", menuName = "Map Data", order = 51)]
public class Map_Data : ScriptableObject
{
    [SerializeField] public List<MapSection> mapSections = new List<MapSection>();
    public int currentRoom = 0;
}
