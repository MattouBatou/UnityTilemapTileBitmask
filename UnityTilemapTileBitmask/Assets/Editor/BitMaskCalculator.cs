using UnityEngine;
using UnityEditor;

public class BitMaskCalculator : EditorWindow {

    // Base layout size (all measurements are relative to this)
    private const float k_baseUnit = 25f;
    private const float k_gridWH = k_baseUnit * 4f;

    // Calculator values
    private int m_bitMaskTotal = 0;
    private string m_binDirString = "// ";
    private int[] m_binDirSelectedStates = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private string[] m_binDirDirections = new string[] { "nw", "n", "ne", "w", "", "e", "sw", "s", "se" };
    private static readonly int[] k_bitMaskValues = { 1, 2, 4, 8, 0, 16, 32, 64, 128 };

    // Colors
    private static readonly Color k_binDirButtonOffColor = new Color(0.5f, 0.7f, 1f, 0.5f);
    private static readonly Color k_binDirButtonOnColor = new Color(0.5f, 0.7f, 1f, 1f);
    private static readonly Color k_textWhite = new Color(1f, 1f, 1f);
    private static readonly Color k_cellTextDarkBlue = new Color(0f, 0.2f, 0.5f, 0.5f);

    // Binary directional grid cell width/height
    private static Rect binDirRect;
    private static Rect directionsTextRect;
    private static Rect bitMaskValueRect;
    public float binDirWH;

    [MenuItem("Window/Tile Map Tools/Bitmask Calculator")]
    public static void Init() {
        binDirRect = new Rect(k_baseUnit, k_baseUnit, k_gridWH * 1.25f, k_gridWH * 1.25f);
        directionsTextRect = new Rect(binDirRect.x + (binDirRect.width * 1.25f), binDirRect.y, binDirRect.width, binDirRect.height / 3);
        bitMaskValueRect = new Rect(binDirRect.x + (binDirRect.width), binDirRect.y, binDirRect.width, binDirRect.height);

        // Get existing open window or if none, make a new one:
        BitMaskCalculator window = (BitMaskCalculator)EditorWindow.GetWindow(typeof(BitMaskCalculator), false, "Bitmask Calculator", true);
        window.maxSize = new Vector2((binDirRect.width * 2.5f) + k_baseUnit, binDirRect.height + (k_baseUnit * 2));
        window.minSize = window.maxSize;
    }

    private void OnGUI() {
        createBitMaskCalculator();
    }

    private void createBitMaskCalculator() {
        // Have to set values again due to positions being incorrect when set onload.
        binDirRect = new Rect(k_baseUnit, k_baseUnit, k_gridWH * 1.25f, k_gridWH * 1.25f);
        directionsTextRect = new Rect(binDirRect.x + (binDirRect.width * 1.25f), binDirRect.y, binDirRect.width, binDirRect.height / 3);
        bitMaskValueRect = new Rect(binDirRect.x + (binDirRect.width), binDirRect.y, binDirRect.width, binDirRect.height);
        binDirWH = (k_gridWH * 1.25f) / 3f;

        // Binary directional settings
        Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);

        GUIStyle BitMaskValuesStyle = new GUIStyle();
        BitMaskValuesStyle.alignment = TextAnchor.MiddleCenter;
        BitMaskValuesStyle.normal.textColor = k_textWhite;
        BitMaskValuesStyle.normal.background = MakeTex((int)binDirWH, (int)binDirWH, k_binDirButtonOffColor);

        GUIStyle BitMaskTotalStyle = new GUIStyle();
        BitMaskTotalStyle.alignment = TextAnchor.MiddleCenter;
        BitMaskTotalStyle.fontSize = 20;

        GUIStyle BinDirDirectionStringStyle = new GUIStyle();
        BinDirDirectionStringStyle.alignment = TextAnchor.MiddleLeft;
        BinDirDirectionStringStyle.normal.textColor = new Color(0f, 0.5f, 0f);
        BinDirDirectionStringStyle.fontSize = 10;

        GUIStyle BinDirGridDirectionsStyle = new GUIStyle();
        BinDirGridDirectionsStyle.alignment = TextAnchor.UpperCenter;
        BinDirGridDirectionsStyle.normal.textColor = k_cellTextDarkBlue;
        BinDirGridDirectionsStyle.fontSize = 10;

        m_binDirString = "// ";

        // Draw Binary directional grid
        for (int y = 0; y <= 3; y++) {
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

                Rect gridCellRect = new Rect(binDirRect.x + (binDirWH * x) - 1, binDirRect.y + (binDirWH * y) - 1, binDirWH + 1, binDirWH + 1);

                if (x != 1 || y != 1) {
                    if (Event.current.type == EventType.MouseDown && gridCellRect.Contains(Event.current.mousePosition) && Event.current.button == 0) {

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

                    GUI.Box(gridCellRect, k_bitMaskValues[binDirCellIndex].ToString(), BitMaskValuesStyle);

                    // Set correct direction text alignment
                    if (y == 0 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.UpperLeft; } else if (y == 0 && x == 1) { BinDirGridDirectionsStyle.alignment = TextAnchor.UpperCenter; } else if (y == 0 && x == 2) { BinDirGridDirectionsStyle.alignment = TextAnchor.UpperRight; } else if (y == 1 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.MiddleLeft; } else if (y == 1 && x == 2) { BinDirGridDirectionsStyle.alignment = TextAnchor.MiddleRight; } else if (y == 2 && x == 0) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerLeft; } else if (y == 2 && x == 1) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerCenter; } else if (y == 2 && x == 2) { BinDirGridDirectionsStyle.alignment = TextAnchor.LowerRight; }

                    GUI.Box(gridCellRect, m_binDirDirections[binDirCellIndex], BinDirGridDirectionsStyle);
                }

                binDirCellIndex++;
            }
        }

        for (int i = 0; i < m_binDirSelectedStates.Length; i++) {
            if (m_binDirSelectedStates[i] == 1) {
                m_binDirString += m_binDirDirections[i] + " ";
            }
        }

        // Selected direction string as code comment for quicker pasting next to your BitMask values in code for referencing later in custom tile scripts.
        if (m_binDirString != "// ") {
            EditorGUI.TextArea(directionsTextRect, m_binDirString, BinDirDirectionStringStyle);
        }

        // Bit Mask Value
        EditorGUI.TextArea(bitMaskValueRect, m_bitMaskTotal.ToString(), BitMaskTotalStyle);
    }

    private Texture2D MakeTex(int width, int height, Color col) {
        // Used for GUI.Box background colors
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width - 1, height - 1);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void sumBitMaskValues(int binDirCellIndex) {

        // pow2 value * binaryValue (-1 if 0 to subtract pow2 value from BitMaskTotal)
        m_bitMaskTotal += k_bitMaskValues[binDirCellIndex] * (m_binDirSelectedStates[binDirCellIndex] == 0 ? -1 : m_binDirSelectedStates[binDirCellIndex]);
    }
}
