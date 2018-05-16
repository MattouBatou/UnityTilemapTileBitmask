using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {

    [Serializable]
    public class BitMask4Tile:TileBase {

        public Sprite[] m_BitSprites;
        public Tile.ColliderType m_TileColliderType;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(location, tilemap, ref tileData);

            if (m_BitSprites == null || m_BitSprites.Length <= 0) return;

            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.sprite = m_BitSprites[0];
            tileData.colliderType = m_TileColliderType;
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

            for (int i = 0; i < k_bitSpriteCount; i++) {
                Rect r = (Rect)EditorGUILayout.BeginVertical();
                tile.m_BitSprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + (i), tile.m_BitSprites[i], typeof(Sprite), false, null);
                spriteIcons[i].filterMode = FilterMode.Point;
                EditorGUI.DrawPreviewTexture(new Rect((EditorGUIUtility.currentViewWidth) - (k_tHW * 2.5f), r.y, k_tHW, k_tHW), spriteIcons[i]);
                EditorGUILayout.EndVertical();
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
