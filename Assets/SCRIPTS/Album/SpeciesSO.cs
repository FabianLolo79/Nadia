using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecies", menuName = "Album/Marine Species")]
public class SpeciesSO : ScriptableObject
{
    public string speciesID;
    public string speciesName;
    public Sprite speciesImage;
    public string description;
}

