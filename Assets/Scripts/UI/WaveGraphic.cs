using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasRenderer))]
public class WaveGraphic : MaskableGraphic
{
    [Header("Wave Settings")]
    [Range(50, 300)]
    public int points = 120;                // fewer points = cleaner lines
    public float widthLocal = 500f;         // visible width of the wave
    public float heightLocal = 150f;        // visible height of the wave

    // Target wave (fixed)
    [HideInInspector] public float targetAmplitude = 1f;
    [HideInInspector] public float targetFrequency = 1f;
    [HideInInspector] public float targetPhase = 0f;

    // Player wave (controlled by sliders)
    [HideInInspector] public float playerAmplitude = 1f;
    [HideInInspector] public float playerFrequency = 1f;
    [HideInInspector] public float playerPhase = 0f;

    [Header("Visual")]
    public Color targetColor = new Color(0f, 1f, 1f, 0.6f);   // cyan, semi-transparent
    public Color playerColor = new Color(1f, 0f, 1f, 0.9f);   // magenta, bolder
    public float lineThickness = 3f;                           // easier to see
    public Color baselineColor = new Color(0.6f, 0.6f, 0.6f, 0.4f); // reference line
    public float baselineThickness = 1.5f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        float left = rect.xMin;
        float right = rect.xMax;
        float bottom = rect.yMin;
        float top = rect.yMax;
        float midY = Mathf.Lerp(bottom, top, 0.5f);

        // 1️⃣ Draw Baseline (Center Reference)
        AddLine(vh, new Vector2(left, midY), new Vector2(right, midY), baselineColor, baselineThickness);

        // 2️⃣ Draw both waves (target first, then player)
        for (int pass = 0; pass < 2; pass++)
        {
            bool drawTarget = (pass == 0);
            Color col = drawTarget ? targetColor : playerColor;
            float amp = drawTarget ? targetAmplitude : playerAmplitude;
            float freq = drawTarget ? targetFrequency : playerFrequency;
            float ph = drawTarget ? targetPhase : playerPhase;

            // Loop through and draw smooth sine
            for (int i = 0; i < points - 1; i++)
            {
                float t0 = i / (float)(points - 1);
                float t1 = (i + 1) / (float)(points - 1);

                // Normalize horizontally (show about 1–1.5 wave cycles)
                float x0 = Mathf.Lerp(left, right, t0);
                float x1 = Mathf.Lerp(left, right, t1);
                float normX0 = t0 * Mathf.PI * 2f;
                float normX1 = t1 * Mathf.PI * 2f;

                // Calculate Y positions
                float y0 = Mathf.Sin((normX0 * freq) + ph) * (amp * (heightLocal * 0.5f));
                float y1 = Mathf.Sin((normX1 * freq) + ph) * (amp * (heightLocal * 0.5f));

                Vector2 v0 = new Vector2(x0, midY + y0);
                Vector2 v1 = new Vector2(x1, midY + y1);

                DrawThickLine(vh, v0, v1, col, lineThickness, i, pass);
            }
        }
    }

    // Helper function: draw one line segment (quad)
    private void DrawThickLine(VertexHelper vh, Vector2 start, Vector2 end, Color color, float thickness, int segmentIndex, int pass)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = start + normal; vh.AddVert(vert);
        vert.position = start - normal; vh.AddVert(vert);
        vert.position = end + normal; vh.AddVert(vert);
        vert.position = end - normal; vh.AddVert(vert);

        int baseIndex = (segmentIndex * 4) + (pass * (points - 1) * 4);
        vh.AddTriangle(baseIndex + 0, baseIndex + 2, baseIndex + 1);
        vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 1);
    }

    // Helper function: simple straight baseline
    private void AddLine(VertexHelper vh, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = start + normal; vh.AddVert(vert);
        vert.position = start - normal; vh.AddVert(vert);
        vert.position = end + normal; vh.AddVert(vert);
        vert.position = end - normal; vh.AddVert(vert);

        int baseIndex = vh.currentVertCount - 4;
        vh.AddTriangle(baseIndex + 0, baseIndex + 2, baseIndex + 1);
        vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 1);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetAllDirty();
    }
#endif
}
