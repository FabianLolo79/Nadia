using UnityEngine;

//
// ScriptableObject que almacena los datos **fijos** de una especie marina.
// Usar únicamente para datos inmutables (ID, nombre, sprite, descripción).
//
[CreateAssetMenu(fileName = "NewSpecies", menuName = "Album/Marine Species")]
public class SpeciesSO : ScriptableObject
{
    public string speciesID;       // Identificador único (ej. "octopus", "sea_star")
    public string speciesName;     // Nombre a mostrar en UI
    public Sprite speciesImage;    // Sprite/imagen en color para el álbum
    public Sprite speciesImagePixel;   // Imagen en pixel art (juego)
    public string description;     // Texto descriptivo (opcional)
}

