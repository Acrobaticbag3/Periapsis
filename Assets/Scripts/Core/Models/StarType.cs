using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    O & B --> very bright, radiation-heavy
    A --> Some "normal" worlds, mostly extreams
    F & G --> Best for habitable, "earth-like" worlds
    K & M --> Habitable, dim
*/
public enum StarType
{
    O,  // Blue, rare, massive, bright
    B,  // Blue-white
    A,  // White/blue-white, common
    F,  // Yellow-white
    G,  // Yellow, like Sol, stable & habitable zone friendly
    K,  // Orange, good for habitable worlds
    M   // Red dwarf, most common star type, dim
}
