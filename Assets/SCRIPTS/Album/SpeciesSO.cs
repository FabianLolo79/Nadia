using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecies", menuName = "Album/Marine Species")]
public class SpeciesSO : ScriptableObject
{
    public string speciesID;            // e.g. "octopus", "sea_star"
    public string speciesName;          // e.g. "Octopus"
    public Sprite speciesImage;         // Sprite shown in album
    public string description;          // Optional description or trivia
}

