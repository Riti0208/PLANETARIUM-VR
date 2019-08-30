using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateStarObjects : EditorWindow {
    [MenuItem("PlanetaliumTools/GenerateStars")]
    static public void Generate() {
        string path = "Assets/Prefabs/Stars.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        StarDataAssets starDatas = Resources.Load<StarDataAssets>("StarDataAsset");

        foreach (StarDataAssets.StarData starData in starDatas.starDataList) {
            float hDeg = (360f / 24f) * (starData.ra.x + starData.ra.y / 60f + starData.ra.z / 3600f);

            float signNumber = -1f;

            if (starData.dec.x < 0) {
                starData.dec.x *= -1;
                signNumber = 1f;
            }

            float sDeg = (starData.dec.x + starData.dec.y / 60f + starData.dec.z / 3600f) * signNumber;

            Quaternion rotL = Quaternion.AngleAxis(hDeg, Vector3.up);
            Quaternion rotS = Quaternion.AngleAxis(sDeg, Vector3.right);

            Vector3 pos = rotL * rotS * Vector3.forward * 100;

            var obj = Object.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
        }

        
    }
}
