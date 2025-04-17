using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public int carValue;
    public int coin;
    public int highScore;
}
