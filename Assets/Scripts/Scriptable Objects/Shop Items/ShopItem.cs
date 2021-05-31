using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item", menuName = "Shop Item", order = 51)]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public string description;
    public int[] price;
    [Range(0, 3)] public int level;
    public Sprite[] itemImage;

}