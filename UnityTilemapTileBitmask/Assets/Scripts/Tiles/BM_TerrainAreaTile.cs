using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {

    [Serializable]
    public class BM_TerrainAreaTile:TileBase {

        [SerializeField]
        public Sprite[] m_BitSprites;
        public Tile.ColliderType m_TileColliderType = Tile.ColliderType.None;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(location, tilemap, ref tileData);

            if (m_BitSprites == null || m_BitSprites.Length <= 0) return;

            UpdateTile(location, tilemap, ref tileData);
        }

        public override void RefreshTile(Vector3Int location, ITilemap tilemap) {
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

            int bitMaskValue = NeighborsToTileIndexTerrain(mask);

            if (bitMaskValue >= 0 && bitMaskValue < m_BitSprites.Length && NeighboringTileAtPos(tilemap, location)) {

                tileData.sprite = m_BitSprites[bitMaskValue];
            }
        }

        private int NeighborsToTileIndexTerrain(int mask) {
            // Cases grouped by sprite selection.

            switch (mask) {

                case 11: return 1;      // nw n w
                case 22: return 2;      // n ne e
                case 31: return 3;      // nw n ne w e
                case 104: return 4;    // w sw s
                case 107: return 5;    // nw n w sw s
                case 208: return 6;    // e s se
                case 214: return 7;    // n ne e s se
                case 248: return 8;    // w e sw s se
                case 255: return 9;    // all
            }
            return 0;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/BM_TerrainAreaTile")]
        public static void CreateBM_TerrainAreaTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save BitMask Terrain Area Tile", "New 4-bit Bitmask Tile", "asset", "Save BitMask Terrain Area Tile", "Assets");
            if (path == "") {
                return;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BM_TerrainAreaTile>(), path);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BM_TerrainAreaTile))]
    [CanEditMultipleObjects]
    internal class BM_TerrainAreaTileEditor:Editor {
        private BM_TerrainAreaTile tile { get { return (target as BM_TerrainAreaTile); } }
        private const int k_bitSpriteCount = 10;

        // Binary directonal grid component settings.
        private int m_bitMaskTotal = 0;
        private string m_binDirString = "// ";
        private int[] m_binDirSelectedStates = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private string[] m_binDirDirections = new string[] { "nw", "n", "ne", "w", "", "e", "sw", "s", "se" };
        private static readonly int[] k_bitMaskValues = { 1, 2, 4, 8, 0, 16, 32, 64, 128 };
        private const float k_spriteWH = 16f * 4f;

        // Colors
        private static readonly Color k_binDirButtonOffColor = new Color(0.5f, 0.7f, 1f, 0.5f);
        private static readonly Color k_binDirButtonOnColor = new Color(0.5f, 0.7f, 1f, 1f);
        private static readonly Color k_textWhite = new Color(1f,1f,1f);
        private static readonly Color k_cellTextDarkBlue = new Color(0f, 0.2f, 0.5f, 0.5f);

        // Tile Example Sprite Icons
        private const string s_spriteIcon0 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFklEQVQI12MAgvp/IJTAhgdB1ADVAgDvdAnxdKVuwAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon1 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAATbCqP4fEAGVAgAZAQND5NXRlgAAAABJRU5ErkJggg==";
        private const string s_spriteIcon2 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAggQiUP0/IAKqBQCNTQd7L2yUwAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon3 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MgEtT/AyIgDQARjQL7N/HyZAAAAABJRU5ErkJggg==";
        private const string s_spriteIcon4 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IMTARgRiAACCeQNDJCF45wAAAABJRU5ErkJggg==";
        private const string s_spriteIcon5 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAADklEQVQI12MAATaiEQMAB+YAVQErUaIAAAAASUVORK5CYII=";
        private const string s_spriteIcon6 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IJTAQBABAQDddQd7cGz9TwAAAABJRU5ErkJggg==";
        private const string s_spriteIcon7 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAD0lEQVQI12MAggRiERAAAIDQBUEcq+81AAAAAElFTkSuQmCC";
        private const string s_spriteIcon8 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MAgvp/IEQcAAB8tQL7Ry7GCQAAAABJRU5ErkJggg==";
        private const string s_spriteIcon9 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAAA1BMVEUB/wA1nKqfAAAAC0lEQVQI12MgEQAAADAAAWV61nwAAAAASUVORK5CYII=";

        private static Texture2D[] s_spriteIcons;
        private static Texture2D[] spriteIcons {
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
                }
                return s_spriteIcons;
            }
        }

        public override void OnInspectorGUI() {
            createEditorLayout();
        }

        private void createEditorLayout() {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Reset size of sprite array if it has somehow changed (Defensive coding for future users)
            if (tile.m_BitSprites == null || tile.m_BitSprites.Length != k_bitSpriteCount) {
                Array.Resize<Sprite>(ref tile.m_BitSprites, k_bitSpriteCount);
            }

            EditorGUILayout.LabelField("Place sprites to match the example images.");
            EditorGUILayout.LabelField("Sprites required " + k_bitSpriteCount);

            EditorGUILayout.Space();

            // TODO: Make editor option to have individual ColliderType values for each sprite. If selected add a ColliderType popup for every sprite
            tile.m_TileColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Collider Type", tile.m_TileColliderType);

            EditorGUILayout.Space();

            createBitMaskCalculator();

            // Draw actual tile sprite fields. These are used to assign the correct sprite to the tile when placing
            for (int i = 0; i < k_bitSpriteCount; i++) {
                Rect r = (Rect)EditorGUILayout.BeginVertical();
                tile.m_BitSprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + (i), tile.m_BitSprites[i], typeof(Sprite), false, null);
                spriteIcons[i].filterMode = FilterMode.Point;
                EditorGUI.DrawPreviewTexture(new Rect((EditorGUIUtility.currentViewWidth) - (k_spriteWH * 2.5f), r.y, k_spriteWH, k_spriteWH), spriteIcons[i]);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUI.EndChangeCheck();
        }

        private void createBitMaskCalculator() {

            // Binary directional settings
            Rect binDirRect = new Rect(EditorGUIUtility.currentViewWidth - (k_spriteWH * 1.25f) - 20f, 118f, k_spriteWH*1.25f, k_spriteWH*1.25f);
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
            // Binary directional grid cell width/height
            float binDirWH = binDirRect.width / 3f;

            GUIStyle BitMaskValuesStyle = new GUIStyle();
            BitMaskValuesStyle.alignment = TextAnchor.MiddleCenter;
            BitMaskValuesStyle.normal.textColor = k_textWhite;
            BitMaskValuesStyle.normal.background = MakeTex((int)binDirWH, (int)binDirWH, k_binDirButtonOffColor);

            GUIStyle BitMaskTotalStyle = new GUIStyle();
            BitMaskTotalStyle.alignment = TextAnchor.MiddleCenter;
            BitMaskTotalStyle.fontSize = 20;

            GUIStyle BinDirDirectionStringStyle = new GUIStyle();
            BinDirDirectionStringStyle.alignment = TextAnchor.MiddleLeft;
            BinDirDirectionStringStyle.normal.textColor = new Color(0f,0.5f,0f);
            BinDirDirectionStringStyle.fontSize = 10;

            GUIStyle BinDirGridDirectionsStyle = new GUIStyle();
            BinDirGridDirectionsStyle.alignment = TextAnchor.UpperCenter;
            BinDirGridDirectionsStyle.normal.textColor = k_cellTextDarkBlue;
            BinDirGridDirectionsStyle.fontSize = 8;

            m_binDirString = "// ";

            // Draw Binary directional grid
            for (int y = 0; y <=3; y++) {
                float top = binDirRect.yMin + y * binDirWH;
                Handles.DrawLine(new Vector3(binDirRect.xMin, top), new Vector3(binDirRect.xMax, top));
            }
            for (int x = 0; x <= 3; x++) {
                float left = binDirRect.xMin + x * binDirWH;
                Handles.DrawLine(new Vector3(left, binDirRect.yMin), new Vector3(left, binDirRect.yMax));
            }
            Handles.color = Color.white;

            int binDirCellIndex = 0;
            for (int y = 0; y <= 2; y++) {
                for (int x = 0; x <= 2; x++) {

                    Rect r = new Rect(binDirRect.x + (binDirWH * x) - 1, binDirRect.y + (binDirWH * y) - 1, binDirWH + 1, binDirWH + 1);

                    if (x != 1 || y != 1) {
                        if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition) && Event.current.button == 0) {

                            Event.current.Use();

                            // Set selected states for binary directional cells
                            if (m_binDirSelectedStates[binDirCellIndex] == 0) {
                                m_binDirSelectedStates[binDirCellIndex] = 1;
                            } else {
                                m_binDirSelectedStates[binDirCellIndex] = 0;
                            }

                            sumBitMaskValues(binDirCellIndex);
                        }

                        // Set background color of binary directional cells
                        if (m_binDirSelectedStates[binDirCellIndex] == 0) {
                            BitMaskValuesStyle.normal.background = MakeTex((int)binDirWH, (int)binDirWH, k_binDirButtonOffColor);
                        } else {
                            BitMaskValuesStyle.normal.background = MakeTex((int)binDirWH, (int)binDirWH, k_binDirButtonOnColor);
                        }

                        GUI.Box(r, k_bitMaskValues[binDirCellIndex].ToString(), BitMaskValuesStyle);

                        // Set correct direction text alignment
                        if(y == 0 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.UpperLeft; }
                        else if(y == 0 && x == 1){ BinDirGridDirectionsStyle.alignment = TextAnchor.UpperCenter; }
                        else if(y == 0 && x == 2){ BinDirGridDirectionsStyle.alignment = TextAnchor.UpperRight; }
                        else if (y == 1 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.MiddleLeft; }
                        else if (y == 1 && x == 2) { BinDirGridDirectionsStyle.alignment = TextAnchor.MiddleRight; }
                        else if (y == 2 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerLeft; }
                        else if (y == 2 && x == 1) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerCenter; }
                        else if (y == 2 && x == 2) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerRight; }

                        GUI.Box(r, m_binDirDirections[binDirCellIndex], BinDirGridDirectionsStyle);
                    }

                    binDirCellIndex++;
                }
            }

            for(int i = 0; i < m_binDirSelectedStates.Length; i++) {
                if (m_binDirSelectedStates[i] == 1) {
                    m_binDirString += m_binDirDirections[i] + " ";
                }
            }

            // Selected direction string as code comment for quicker pasting next to your BitMask values in code for referencing later in custom tile scripts.
            if (m_binDirString != "// ") {
                EditorGUI.TextArea(new Rect(binDirRect.x - (binDirRect.width * 1.9f), binDirRect.y, binDirRect.width, binDirRect.height / 3), m_binDirString, BinDirDirectionStringStyle);
            }

            // Bit Mask Value
            EditorGUI.TextArea(new Rect(binDirRect.x - (binDirRect.width * 1.625f), binDirRect.y, binDirRect.width, binDirRect.height), m_bitMaskTotal.ToString(), BitMaskTotalStyle);

            // Lazy way to get spacing. Might mess up on different size screens
            for (int i = 0; i < 14; i++) { EditorGUILayout.Space(); }
        }

        private static Texture2D Base64ToTexture(string base64) {
            Texture2D t = new Texture2D(1, 1);
            t.hideFlags = HideFlags.HideAndDontSave;
            t.LoadImage(System.Convert.FromBase64String(base64));
            return t;
        }

        private Texture2D MakeTex(int width, int height, Color col) {
            // Used for GUI.Box background colors
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width-1, height-1);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void sumBitMaskValues(int binDirCellIndex) {

            // pow2 value * binaryValue (-1 if 0 to subtract pow2 value from BitMaskTotal)
            m_bitMaskTotal += k_bitMaskValues[binDirCellIndex] * (m_binDirSelectedStates[binDirCellIndex] == 0 ? -1 : m_binDirSelectedStates[binDirCellIndex]);
        }
    }
#endif
}
