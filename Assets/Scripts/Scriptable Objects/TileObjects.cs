using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileObjects", menuName = "Tile Object", order = 51)]
public class TileObjects : ScriptableObject
{
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public int[] tileNums;
    [SerializeField] public bool[] chisel;
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool hasGlow;
    [SerializeField] public Color glow;
    [SerializeField, Range(0, 3)] public int colorScheme;
    [SerializeField, Range(1, 2)] public int palette;

}