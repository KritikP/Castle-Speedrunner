using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TileObjects", menuName = "Tile Object", order = 51)]
public class TileObjects : ScriptableObject
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public int[] tileNums;
}