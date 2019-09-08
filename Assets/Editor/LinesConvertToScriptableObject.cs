using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LinesConvertToScriptableObject : EditorWindow {

    /**
     * https://heasarc.gsfc.nasa.gov/cgi-bin/W3Browse/w3query.pl
     * ヒッパルコス星座表の出力データをスクリプタブルオブジェクトに変更するメソッド
     */

    [MenuItem("PlanetaliumTools/LinesConvertToScriptableObject")]
    static void Open() {

        StarLineDataAssets starLineDataAsset = CreateInstance<StarLineDataAssets>();
        StarDataAssets starDataAssets = Resources.Load<StarDataAssets>("StarDataAsset");

        string path = EditorUtility.OpenFilePanel("星座データを選んでね", "", "");
        if (path.Length != 0) {
            string[] lines = File.ReadAllLines(path);

            int cnt = 0;

            foreach (string strLine in lines) {
                cnt++;

                string[] strLineArr = strLine.Split(',');

                StarDataAssets.StarData starDataFirst = null;
                StarDataAssets.StarData starDataSecond = null;
                
                starDataFirst = starDataAssets.starDataList.Find(c => c.hipNum == int.Parse(strLineArr[1]));
                starDataSecond = starDataAssets.starDataList.Find(c => c.hipNum == int.Parse(strLineArr[2]));

                Vector2 posFirst = getStarPos(starDataFirst);
                Vector2 posSecond = getStarPos(starDataSecond);

                starLineDataAsset.starLineDataList.Add(
                    new StarLineDataAssets.StarLineData(
                        strLineArr[0],
                        posFirst.x,
                        posFirst.y,
                        posSecond.x,
                        posSecond.y
                    )
                    );
            }
        }

        AssetDatabase.CreateAsset(starLineDataAsset, "Assets/Resources/StarLineDataAsset.asset");
        AssetDatabase.Refresh();
    }

    static Vector2 getStarPos(StarDataAssets.StarData starData) {
        if (starData == null) {
            return Vector3.zero;
        }

        float hDeg = (360f / 24f) * (starData.ra.x + starData.ra.y / 60f + starData.ra.z / 3600f);

        int hsSgn = 1;

        float decX = starData.dec.x;

        if (decX < 0) {
            decX *= -1;
            hsSgn = -1;
        }

        float sDeg = (decX + starData.dec.y / 60f + starData.dec.z / 3600f) * hsSgn;

        return new Vector2(hDeg, sDeg);
    }
}
