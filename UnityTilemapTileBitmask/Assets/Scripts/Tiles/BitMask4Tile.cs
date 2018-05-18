using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {

    [Serializable]
    public class BitMask4Tile:TileBase {

        [SerializeField]
        public Sprite[] m_BitSprites;
        public Tile.ColliderType m_TileColliderType = Tile.ColliderType.None;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(location, tilemap, ref tileData);

            if (m_BitSprites == null || m_BitSprites.Length <= 0) return;

            UpdateTile(location, tilemap, ref tileData);
        }

        public override void RefreshTile(Vector3Int location, ITilemap tilemap) {
            //base.RefreshTile(location, tilemap);
            checkNeighboringTiles(location, tilemap);
        }

        // Looks for tiles around this tile and calls RefreshTile on any found.
        public void checkNeighboringTiles(Vector3Int location, ITilemap tilemap) {
            for (int yd = -1; yd <= 1; yd++) {
                for (int xd = -1; xd <= 1; xd++) {
                    Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    if (NeighboringTileAtPos(tilemap, position))
                        tilemap.RefreshTile(position);
                }
            }
        }

        private bool NeighboringTileAtPos(ITilemap tileMap, Vector3Int position) {
            TileBase tile = tileMap.GetTile(position);
            return (tile != null && tile == this);
        }

        private void UpdateTile(Vector3Int location, ITilemap tilemap, ref TileData tileData) {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.colliderType = m_TileColliderType;

            // 1 is x: right y: up.
            int mask = NeighboringTileAtPos(tilemap, location + new Vector3Int(-1, 1, 0)) ? 1 : 0;  // nw
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(0, 1, 0)) ? 2 : 0;      // n
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(1, 1, 0)) ? 4 : 0;      // ne
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;     // w
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(1, 0, 0)) ? 16 : 0;     // e
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;   // sw
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(0, -1, 0)) ? 64 : 0;    // s
            mask += NeighboringTileAtPos(tilemap, location + new Vector3Int(1, -1, 0)) ? 128 : 0;   // se

            int index = NeighborsToTileIndexTerrain(mask);

            //if(index == 0) {
            //    index = NeighborsToTileIndexPipe(mask);
            //}

            if (index >= 0 && index < m_BitSprites.Length && NeighboringTileAtPos(tilemap, location)) {

                tileData.sprite = m_BitSprites[index];
            }
        }

        private int NeighborsToTileIndexTerrain(int mask) {
            // Cases grouped by sprite selection.

            switch (mask) {

                case 15:                // nw,n,ne,w
                case 11: return 3;      // nw,n,w

                case 23:                // nw,n,ne,e
                case 150:               // nw,n,sw,w
                case 22: return 5;      // n,ne,e

                case 190:               // n,ne,e,sw,w
                case 63:                // nw,n,ne,e,sw,w
                case 31:                // nw,n,ne,w
                case 7:                 // ne,n,nw
                case 2: return 7;       // n

                case 104: return 10;    // s,sw,w

                case 235:               // nw,n,se,s,sw,w
                case 111:               // nw,n,ne,s,sw,w
                case 107:               // nw,n,s,sw,w
                case 41:                // nw,w,sw
                case 8: return 11;      // e

                case 240:               // e,se,s,sw
                case 208: return 12;    // s,se,e

                case 214:               // n,ne,e,se,s
                case 215:               // nw,n,ne,e,se,s
                case 148:               // ne,e,se
                case 16: return 13;     // w

                case 124:               // ne,e,s,sw,w
                case 248:               // e,se,s,sw,w
                case 224:               // se,s,sw
                case 64: return 14;     // s

                case 127:               // nw,n,ne,e,s,sw,w
                case 223:               // nw,n,ne,e,se,s,w
                case 90:                // n,s,e,w
                case 255: return 15;    // all

            }
            return 0;
        }

        private int NeighborsToTileIndexPipe(int mask) {
            // Cases grouped by sprite selection.

            switch (mask) {

                case 2: return 1;       // n

                case 16: return 2;      // e

                case 10: return 3;      // w,s

                case 8: return 4;       // w

                case 18: return 5;      // n,e

                case 64: return 8;      // s

                case 72: return 10;     // w,s

                case 80: return 12;     // e,s
            }
            return 0;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/BitMask4Tile")]
        public static void CreateBitMask4Tile() {
            string path = EditorUtility.SaveFilePanelInProject("Save BitMask 4-bit Tile", "New 4-bit Bitmask Tile", "asset", "Save BitMask 4-bit Tile", "Assets");
            if (path == "") {
                return;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BitMask4Tile>(), path);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BitMask4Tile))]
    [CanEditMultipleObjects]
    internal class BitMask4TileEditor:Editor {
        private BitMask4Tile tile { get { return (target as BitMask4Tile); } }
        private const int k_bitSpriteCount = 16;
        public int[] m_selectedBitMaskValues = new int[]{0,0,0,0,0,0,0,0,0};
        public int m_BitMaskTotal = 0;
        public static readonly int[] k_BitMaskValues = { 1, 2, 4, 8, 0, 16, 32, 64, 128 };

        private const string s_spriteIcon0 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFklEQVQI12MAgvp/IJTAhgdB1ADVAgDvdAnxdKVuwAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon1 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFElEQVQI12MAggQ2wqj+HxAB1QIAlMEHw4qUPRAAAAAASUVORK5CYII=";
        private const string s_spriteIcon2 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFUlEQVQI12MAgvp/IMTAhgfB1DAAAJOUBjEmAwzVAAAAAElFTkSuQmCC";
        private const string s_spriteIcon3 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAATbCqP4fEAGVAgAZAQND5NXRlgAAAABJRU5ErkJggg==";
        private const string s_spriteIcon4 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFklEQVQI12MAgvp/IJTAgBtB1QDVAgDp8gm1mW00CAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon5 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAggQiUP0/IAKqBQCNTQd7L2yUwAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon6 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEklEQVQI12MAgvp/IIQPINQAAI4SBfW8z+asAAAAAElFTkSuQmCC";
        private const string s_spriteIcon7 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MgEtT/AyIgDQARjQL7N/HyZAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon8 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFElEQVQI12MAgvp/IJTARhAB1QIA4zkHw20XQZEAAAAASUVORK5CYII=";
        private const string s_spriteIcon9 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MAggQ2IhFQLQCIhgWVP/96aQAAAABJRU5ErkJggg==";
        private const string s_spriteIcon10 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IMTARgRiAACCeQNDJCF45wAAAABJRU5ErkJggg==";
        private const string s_spriteIcon11 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAADklEQVQI12MAATaiEQMAB+YAVQErUaIAAAAASUVORK5CYII=";
        private const string s_spriteIcon12 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IJTAQBABAQDddQd7cGz9TwAAAABJRU5ErkJggg==";
        private const string s_spriteIcon13 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAD0lEQVQI12MAggRiERAAAIDQBUEcq+81AAAAAElFTkSuQmCC";
        private const string s_spriteIcon14 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MAgvp/IEQcAAB8tQL7Ry7GCQAAAABJRU5ErkJggg==";
        private const string s_spriteIcon15 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAAA1BMVEUB/wA1nKqfAAAAC0lEQVQI12MgEQAAADAAAWV61nwAAAAASUVORK5CYII=";

        private static Texture2D[] s_spriteIcons;
        public static Texture2D[] spriteIcons {
            get {
                if(s_spriteIcons == null) {
                    s_spriteIcons = new Texture2D[k_bitSpriteCount];
                    s_spriteIcons[0] = Base64ToTexture(s_spriteIcon0);
                    s_spriteIcons[1] = Base64ToTexture(s_spriteIcon1);
                    s_spriteIcons[2] = Base64ToTexture(s_spriteIcon2);
                    s_spriteIcons[3] = Base64ToTexture(s_spriteIcon3);
                    s_spriteIcons[4] = Base64ToTexture(s_spriteIcon4);
                    s_spriteIcons[5] = Base64ToTexture(s_spriteIcon5);
                    s_spriteIcons[6] = Base64ToTexture(s_spriteIcon6);
                    s_spriteIcons[7] = Base64ToTexture(s_spriteIcon7);
                    s_spriteIcons[8] = Base64ToTexture(s_spriteIcon8);
                    s_spriteIcons[9] = Base64ToTexture(s_spriteIcon9);
                    s_spriteIcons[10] = Base64ToTexture(s_spriteIcon10);
                    s_spriteIcons[11] = Base64ToTexture(s_spriteIcon11);
                    s_spriteIcons[12] = Base64ToTexture(s_spriteIcon12);
                    s_spriteIcons[13] = Base64ToTexture(s_spriteIcon13);
                    s_spriteIcons[14] = Base64ToTexture(s_spriteIcon14);
                    s_spriteIcons[15] = Base64ToTexture(s_spriteIcon15);
                }
                return s_spriteIcons;
            }
        }

        const float k_tHW = 16f*4f;

        public override void OnInspectorGUI() {
            createEditorLayout();
        }

        private void createEditorLayout() {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (tile.m_BitSprites == null || tile.m_BitSprites.Length != k_bitSpriteCount) {
                Array.Resize<Sprite>(ref tile.m_BitSprites, k_bitSpriteCount);
            }

            EditorGUILayout.LabelField("Place sprites to match the example images.");
            EditorGUILayout.LabelField("Sprites required " + k_bitSpriteCount);

            EditorGUILayout.Space();

            // TODO: Make editor option to have individual ColliderType values for each sprite. If selected add a ColliderType popup for every sprite.
            tile.m_TileColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Collider Type", tile.m_TileColliderType);

            EditorGUILayout.Space();

            createBitMaskCalculator();

            for (int i = 0; i < k_bitSpriteCount; i++) {
                Rect r = (Rect)EditorGUILayout.BeginVertical();
                tile.m_BitSprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + (i), tile.m_BitSprites[i], typeof(Sprite), false, null);
                spriteIcons[i].filterMode = FilterMode.Point;
                EditorGUI.DrawPreviewTexture(new Rect((EditorGUIUtility.currentViewWidth) - (k_tHW * 2.5f), r.y, k_tHW, k_tHW), spriteIcons[i]);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUI.EndChangeCheck();
        }

        private void createBitMaskCalculator() {
            Rect BitMaskFoldoutRect = new Rect(EditorGUIUtility.currentViewWidth-(k_tHW * 1.25f)-20f, 118f, EditorGUIUtility.currentViewWidth, k_tHW*1.25f);

            Rect matrixRect = new Rect(BitMaskFoldoutRect.x, BitMaskFoldoutRect.y, k_tHW*1.25f, k_tHW*1.25f);
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            int index = 0;
            float w = matrixRect.width / 3f;
            float h = matrixRect.height / 3f;

            GUIStyle BitMaskValuesStyle = new GUIStyle();
            BitMaskValuesStyle.alignment = TextAnchor.MiddleCenter;
            BitMaskValuesStyle.normal.textColor = new Color(1f, 1f, 1f);
            BitMaskValuesStyle.normal.background = MakeTex((int)w, (int)h, new Color(0.5f, 0.7f, 1f, 0.5f));

            GUIStyle BitMaskTotalStyle = new GUIStyle();
            BitMaskTotalStyle.alignment = TextAnchor.MiddleCenter;
            BitMaskTotalStyle.font = Font.CreateDynamicFontFromOSFont("Arial", 20);


            // Draw matrix grid
            for (int y = 0; y <=3; y++) {
                float top = matrixRect.yMin + y * h;
                Handles.DrawLine(new Vector3(matrixRect.xMin, top), new Vector3(matrixRect.xMax, top));
            }
            for (int x = 0; x <= 3; x++) {
                float left = matrixRect.xMin + x * w;
                Handles.DrawLine(new Vector3(left, matrixRect.yMin), new Vector3(left, matrixRect.yMax));
            }
            Handles.color = Color.white;

            for (int y = 0; y <= 2; y++) {
                for (int x = 0; x <= 2; x++) {

                    Rect r = new Rect(matrixRect.x + (w * x) - 1, matrixRect.y + (h * y) - 1, w + 1, h + 1);

                    if (x != 1 || y != 1) {
                        if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition) && Event.current.button == 0) {
                            GUI.changed = true;
                            Event.current.Use();

                            if (m_selectedBitMaskValues[index] == 0) {
                                m_selectedBitMaskValues[index] = 1;
                            } else {
                                m_selectedBitMaskValues[index] = 0;
                            }

                            sumBitMaskValues(index);
                        }

                        if (m_selectedBitMaskValues[index] == 0) {
                            BitMaskValuesStyle.normal.background = MakeTex((int)w, (int)h, new Color(0.5f, 0.7f, 1f, 0.5f));
                        } else {
                            BitMaskValuesStyle.normal.background = MakeTex((int)w, (int)h, new Color(0.5f, 0.7f, 1f, 1f));
                        }

                        GUI.Box(r, k_BitMaskValues[index].ToString(), BitMaskValuesStyle);
                    }

                    index++;
                }
            }

            GUI.Box(new Rect(matrixRect.x - (matrixRect.width * 1.625f), matrixRect.y, matrixRect.width, matrixRect.height), m_BitMaskTotal.ToString(), BitMaskTotalStyle);

            for (int i = 0; i < 14; i++) { EditorGUILayout.Space(); }
        }

        private static Texture2D Base64ToTexture(string base64) {
            Texture2D t = new Texture2D(1, 1);
            t.hideFlags = HideFlags.HideAndDontSave;
            t.LoadImage(System.Convert.FromBase64String(base64));
            return t;
        }

        private Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width-1, height-1);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void sumBitMaskValues(int index) {
            m_BitMaskTotal += k_BitMaskValues[index] * (m_selectedBitMaskValues[index] == 0 ? -1 : m_selectedBitMaskValues[index]);
        }
    }
#endif
}
