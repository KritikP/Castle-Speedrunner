using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player_Data))]
public class PlayerDataGUI : Editor
{
    private bool foldoutStateData;
    private bool baseStats;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Player_Data playerData = (Player_Data) target;

        foldoutStateData = EditorGUILayout.Foldout(foldoutStateData, "State Data", true);
        if (foldoutStateData)
        {
            playerData.health = EditorGUILayout.IntField("Health", playerData.health);
            playerData.stamina = EditorGUILayout.FloatField("Stamina", playerData.stamina);
            playerData.invincible = EditorGUILayout.Toggle("Invincible", playerData.invincible);

            EditorGUILayout.Toggle("Rolling", playerData.rolling);
            EditorGUILayout.Toggle("Can Move", playerData.canMove);
            EditorGUILayout.Toggle("Is Dead", playerData.isDead);

        }
    }
}