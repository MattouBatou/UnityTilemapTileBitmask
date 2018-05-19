using System;

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

            int bitMaskValue = NeighborsToTileIndexTerrain(mask);

            //if(binDirCellIndex == 0) {
            //    binDirCellIndex = NeighborsToTileIndexPipe(mask);
            //}

            if (bitMaskValue >= 0 && bitMaskValue < m_BitSprites.Length && NeighboringTileAtPos(tilemap, location)) {

                tileData.sprite = m_BitSprites[bitMaskValue];
            }
        }

        private int NeighborsToTileIndexTerrain(int mask) {
            // Cases grouped by sprite selection.

            switch (mask) {

                case 11: return 3;      // nw n w

                case 22: return 5;      // n ne e

                case 31: return 7;      // nw n ne w e

                case 208: return 12;    // e s se

                case 214: return 13;    // n ne e s se

                case 248: return 14;    // w e sw s se

                case 104: return 10;    // w sw s

                case 107: return 11;    // nw n w sw s

                case 255: return 15;    // all
            }
            return 0;
        }

        private int NeighborsToTileIndexPipe(int mask) {
            // Cases grouped by sprite selection.

            switch (mask) {

                case 2: return 1;       // n

                case 16: return 2;      // e

                case 10: return 3;      // w s

                case 8: return 4;       // w

                case 18: return 5;      // n e

                case 64: return 8;      // s

                case 72: return 10;     // w s

                case 80: return 12;     // e s
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
        private const float k_spriteWH = 16f * 4f;

        // Tile Example Sprite Icons
        private const string s_spriteIcon0 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFklEQVQI12MAgvp/IJTAhgdB1ADVAgDvdAnxdKVuwAAAAABJRU5ErkJggg==";    // single block
        private const string s_spriteIcon1 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFElEQVQI12MAggQ2wqj+HxAB1QIAlMEHw4qUPRAAAAAASUVORK5CYII=";        // pipe cap bottom
        private const string s_spriteIcon2 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFUlEQVQI12MAgvp/IMTAhgfB1DAAAJOUBjEmAwzVAAAAAElFTkSuQmCC";        // pipe cap right
        private const string s_spriteIcon3 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAATbCqP4fEAGVAgAZAQND5NXRlgAAAABJRU5ErkJggg==";        // bottom right outer corner
        private const string s_spriteIcon4 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFklEQVQI12MAgvp/IJTAgBtB1QDVAgDp8gm1mW00CAAAAABJRU5ErkJggg==";    // pipe cap left
        private const string s_spriteIcon5 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAggQiUP0/IAKqBQCNTQd7L2yUwAAAAABJRU5ErkJggg==";        // bottom left outer corner
        private const string s_spriteIcon6 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEklEQVQI12MAgvp/IIQPINQAAI4SBfW8z+asAAAAAElFTkSuQmCC";            // pipe horizontal
        private const string s_spriteIcon7 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MgEtT/AyIgDQARjQL7N/HyZAAAAABJRU5ErkJggg==";            // bottom edge
        private const string s_spriteIcon8 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAFElEQVQI12MAgvp/IJTARhAB1QIA4zkHw20XQZEAAAAASUVORK5CYII=";        // pipe cap top
        private const string s_spriteIcon9 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MAggQ2IhFQLQCIhgWVP/96aQAAAABJRU5ErkJggg==";            // pipe vertical
        private const string s_spriteIcon10 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IMTARgRiAACCeQNDJCF45wAAAABJRU5ErkJggg==";       // top right outer corner
        private const string s_spriteIcon11 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAADklEQVQI12MAATaiEQMAB+YAVQErUaIAAAAASUVORK5CYII=";               // right edge
        private const string s_spriteIcon12 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAE0lEQVQI12MAgvp/IJTAQBABAQDddQd7cGz9TwAAAABJRU5ErkJggg==";       // top left outer corner
        private const string s_spriteIcon13 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAD0lEQVQI12MAggRiERAAAIDQBUEcq+81AAAAAElFTkSuQmCC";               // left edge
        private const string s_spriteIcon14 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12MAgvp/IEQcAAB8tQL7Ry7GCQAAAABJRU5ErkJggg==";           // top edge
        private const string s_spriteIcon15 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAAA1BMVEUB/wA1nKqfAAAAC0lEQVQI12MgEQAAADAAAWV61nwAAAAASUVORK5CYII=";                       // Filled

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
    }
#endif
}
