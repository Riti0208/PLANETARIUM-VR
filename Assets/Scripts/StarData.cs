using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarData {
    public int hipNum;

    public Vector3 ra, dec;

    public float parallax;

    public Color spectTypeColor;

    public float vMag;

    public StarData(int _hipNum, Vector3 _ra, Vector3 _dec, float _parallax, Color _spectTypeColor, float _vMag) {
        hipNum = _hipNum;
        ra = _ra;
        dec = _dec;
        parallax = _parallax;
        spectTypeColor = _spectTypeColor;
        vMag = _vMag;
    }
}