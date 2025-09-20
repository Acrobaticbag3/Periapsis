using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlanetType
{
    Cryogenic,      // Avali reduced upkeep | Humans/Zyrix increased upkeep
    Arctic,         // Avali reduced upkeep | Humans/Zyrix increased upkeep
    Tundra,         // Avali reduced upkeep | Humans/Zyrix increased upkeep
    Desert,         // Neutral | Avali increased upkeep
    Savanna,        // Neutral | Avali increased upkeep
    Continental,    // Humans reduced upkeep | Avali/Zyrix/Velethari increased upkeep
    Ocean,          // Neutral | Avali increased upkeep
    Volcanic,       // Velethari reduced upkeep | Humans/Avali/Zyrix increased upkeep
    Toxic,          // Velethari reduced upkeep | Humans/Avali/Zyrix increased upkeep
    Hazardous,      // Velethari reduced upkeep | Humans/Avali/Zyrix increased upkeep
    GasGiant,       // Neutral - everyone pays normal upkeep
    Moon,           // Zyrix reduced upkeep
    // Derelict        // Zyrix reduced upkeep
}

