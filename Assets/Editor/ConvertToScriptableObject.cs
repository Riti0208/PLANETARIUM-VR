using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ConvertToScriptableObject : EditorWindow {

    /**
     * https://heasarc.gsfc.nasa.gov/cgi-bin/W3Browse/w3query.pl
     * ヒッパルコス星座表の出力データをスクリプタブルオブジェクトに変更するメソッド
     */

    [MenuItem("PlanetaliumTools/ConvertToScriptableObject")]
    static void Open() {

        StarDataAssets starDataAsset = CreateInstance<StarDataAssets>();

        string path = EditorUtility.OpenFilePanel("星座データを選んでね", "", "");
        if (path.Length != 0) {
            string[] lines = File.ReadAllLines(path);

            int cnt = 0;

            foreach (string strLine in lines) {

                cnt++;

                if (cnt > 5) {

                    string[] strLineArr = strLine.Split('|');

                    string[] raArray = strLineArr[2].Split(' ');
                        
                    string[] decArray = strLineArr[3].Split(' ');

                    char spectType = strLineArr[5].Substring(0, 1).ToCharArray()[0];

                    Color spectTypeColor = new Color();
                        
                    switch (spectType) {
                        case 'O':
                            spectTypeColor = new Color(155f, 176f, 255f) / 255f;
                            break;
                        case 'B':
                            spectTypeColor = new Color(170f, 191f, 255f) / 255f;
                            break;
                        case 'A':
                            spectTypeColor = new Color(202f, 215f, 255f) / 255f;
                            break;
                        case 'F':
                            spectTypeColor = new Color(248f, 247f, 255f) / 255f;
                            break;
                        case 'G':
                            spectTypeColor = new Color(255f, 244f, 234f) / 255f;
                            break;
                        case 'K':
                            spectTypeColor = new Color(255f, 210f, 161f) / 255f;
                            break;
                        case 'M':
                            spectTypeColor = new Color(255f, 204f, 111f) / 255f;
                            break;
                        default:
                            spectTypeColor = new Color(0f, 0f, 0f) / 255f;
                            break;
                    }

                    if (strLineArr[4] == "        ") {
                        strLineArr[4] = "10.0";
                    }

                    starDataAsset.starDataList.Add(
                        new StarDataAssets.StarData(
                            strLineArr[1],
                            new Vector3(float.Parse(raArray[0]), float.Parse(raArray[1]), float.Parse(raArray[2])),
                            new Vector3(float.Parse(decArray[0]), float.Parse(decArray[1]), float.Parse(decArray[2])),
                            float.Parse(strLineArr[4]),
                            spectTypeColor,
                            float.Parse(strLineArr[6])
                            )
                        );
                }
            }
        }

        AssetDatabase.CreateAsset(starDataAsset, "Assets/Resources/StarDataAsset.asset");
        AssetDatabase.Refresh();
    }
}
