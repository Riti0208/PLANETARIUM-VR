using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarLineDataAssets : ScriptableObject {
    public List<StarLineData> starLineDataList = new List<StarLineData>();

    [System.SerializableAttribute]
    public class StarLineData {
        public float hDegFirst;
        public float sDegFirst;
        public float hDegSecond;
        public float sDegSecond;
        public string constellationName;

        public StarLineData(string _constellationName, float _hDegFirst, float _sDegFirst, float _hDegSecond, float _sDegSecond) {
            hDegFirst = _hDegFirst;
            sDegFirst = _sDegFirst;
            hDegSecond = _hDegSecond;
            sDegSecond = _sDegSecond;
            constellationName = _constellationName;
        }
    }
}