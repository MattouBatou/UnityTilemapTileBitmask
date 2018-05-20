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

                case 47:                // nw n ne w sw
                case 15:                // nw n ne w
                case 43:                // nw n w sw
                case 11: return 1;      // nw n w

                case 151:               // nw n ne e se
                case 150:               // n ne e se
                case 23:                // nw n ne e
                case 22: return 2;      // n ne e

                case 63:                // nw n ne w e sw
                case 159:               // nw n ne w e se
                case 191:               // nw n ne w e sw se
                case 31: return 3;      // nw n ne w e

                case 233:               // nw w sw s se
                case 105:               // nw w sw s
                case 232:               // w sw s se
                case 104: return 4;     // w sw s

                case 111:               // nw n ne w sw s
                case 239:               // nw n ne w sw s se
                case 235:               // nw n w sw s se
                case 107: return 5;     // nw n w sw s

                case 244:               // ne e sw s se
                case 212:               // ne e s se
                case 240:               // e sw s se
                case 208: return 6;     // e s se

                case 246:               // n ne e sw s se
                case 247:               // nw n ne e sw s se
                case 215:               // nw n ne e s se
                case 214: return 7;     // n ne e s se

                case 249:               // nw w e sw s se
                case 252:               // ne w e sw s se
                case 253:               // nw ne w e sw s se
                case 248: return 8;     // w e sw s se

                case 254: return 10;    // n ne w e sw s se

                case 251: return 11;    // nw n w e sw s se

                case 127: return 12;    // nw n ne w e sw s

                case 223: return 13;    // nw n ne w e s se

                case 255: return 9;     // all
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
        private const int k_bitSpriteCount = 14;
        private const float k_spriteWH = 16f * 4f;

        // Tile Example Sprite Icons
        private static Texture2D[] s_spriteIcons;
        private static Texture2D[] spriteIcons {
            get {
                if (s_spriteIcons == null) {
                    s_spriteIcons = new Texture2D[k_bitSpriteCount];
                    s_spriteIcons[0] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUB/wAAAACkX63mAAAAEElEQVQI12OAA/kf+BAMAAABkgrnNcHeUAAAAABJRU5ErkJggg==");
                    s_spriteIcons[1] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAAEElEQVQI12Ng4CeM/v+HIwBEvgityIdaBgAAAABJRU5ErkJggg==");
                    s_spriteIcons[2] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAAEElEQVQI12P4wEAY/f8PRwCHLxM5+BugggAAAABJRU5ErkJggg==");
                    s_spriteIcons[3] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADklEQVQI12MgEvz/D0EAGxUF+zzBmK8AAAAASUVORK5CYII=");
                    s_spriteIcons[4] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAAEElEQVQI12P4/x+BGPgJIgBbPQitJKcIoAAAAABJRU5ErkJggg==");
                    s_spriteIcons[5] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADUlEQVQI12NgYCcJAQAKeABxy0hRBgAAAABJRU5ErkJggg==");
                    s_spriteIcons[6] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAAEElEQVQI12P4/x+BPjAQRAAfHhM5IM0xRQAAAABJRU5ErkJggg==");
                    s_spriteIcons[7] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADUlEQVQI12N4wEASAgBXPw4BoMIRRgAAAABJRU5ErkJggg==");
                    s_spriteIcons[8] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADklEQVQI12P4/x+KiAMABDoF+5gADEUAAAAASUVORK5CYII=");
                    s_spriteIcons[9] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAAA1BMVEUB/wA1nKqfAAAAC0lEQVQI12MgEQAAADAAAWV61nwAAAAASUVORK5CYII=");
                    s_spriteIcons[10] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADklEQVQI12N4wABBRAIAc7ACoTz4OKoAAAAASUVORK5CYII=");
                    s_spriteIcons[11] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADklEQVQI12NgYIci4gAAA7cAFr01aMAAAAAASUVORK5CYII=");
                    s_spriteIcons[12] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADUlEQVQI12MgFrBDEAAAhAAW20o0MwAAAABJRU5ErkJggg==");
                    s_spriteIcons[13] = Base64ToTexture("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABlBMVEUAAAAB/wA3gS6KAAAADUlEQVQI12MgEjyAIgANUAKhKazXhgAAAABJRU5ErkJggg==");
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
    }
#endif
}
