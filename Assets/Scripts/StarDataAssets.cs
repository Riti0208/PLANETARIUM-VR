using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDataAssets : ScriptableObject {
    public List<StarData> starDataList = new List<StarData>();

    [System.SerializableAttribute]
    public class StarData {
        public string hipName;

        public Vector3 ra, dec;

        public float parallax;

        public Color spectTypeColor;

        public float vMag;

        public StarData(string _hipName, Vector3 _ra, Vector3 _dec,float _parallax, Color _spectTypeColor, float _vMag) {
            hipName = _hipName;
            ra = _ra;
            dec = _dec;
            parallax = _parallax;
            spectTypeColor = _spectTypeColor;
            vMag = _vMag;
        }
    }
}
