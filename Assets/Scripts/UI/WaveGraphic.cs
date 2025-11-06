using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasRenderer))]
public class WaveGraphic : MaskableGraphic
{
    [Header("Wave Settings")]
    public int points = 200;
    public float widthLocal = 800f;     // width in pixels inside rect
    public float heightLocal = 200f;    // height in pixels inside rect

    // Target wave (read-only in inspector at runtime)
    [HideInInspector] public float targetAmplitude = 1f;
    [HideInInspector] public float targetFrequency = 1f;
    [HideInInspector] public float targetPhase = 0f;

    // Player wave (updated by controller)
    [HideInInspector] public float playerAmplitude = 1f;
    [HideInInspector] public float playerFrequency = 1f;
    [HideInInspector] public float playerPhase = 0f;

    [Header("Visual")]
    public Color targetColor = new Color(0f, 1f, 1f, 1f); // cyan
    public Color playerColor = new Color(1f, 0f, 1f, 1f); // magenta
    public float lineThickness = 2f;

    // Draw both waves
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = rectTransform.rect;
        float left = r.xMin;
        float right = r.xMax;
        float top = r.yMax;
        float bottom = r.yMin;

        // We'll map local X from 0..points-1 to left..right
        for (int pass = 0; pass < 2; pass++)
        {
            bool drawTarget = (pass == 0);
            Color col = drawTarget ? targetColor : playerColor;
            float amp = drawTarget ? targetAmplitude : playerAmplitude;
            float freq = drawTarget ? targetFrequency : playerFrequency;
            float ph = drawTarget ? targetPhase : playerPhase;

            // Build strip (two verts per sample to make a quad strip)
            for (int i = 0; i < points - 1; i++)
            {
                float t0 = i / (float)(points - 1);
                float t1 = (i + 1) / (float)(points - 1);

                float x0 = Mathf.Lerp(left, right, t0);
                float x1 = Mathf.Lerp(left, right, t1);

                float normX0 = t0 * widthLocal;
                float normX1 = t1 * widthLocal;

                float y0 = Mathf.Sin((normX0 * freq) + ph) * (amp * (heightLocal * 0.5f));
                float y1 = Mathf.Sin((normX1 * freq) + ph) * (amp * (heightLocal * 0.5f));

                Vector2 v0 = new Vector2(x0, Mathf.Lerp(bottom, top, 0.5f) + y0);
                Vector2 v1 = new Vector2(x1, Mathf.Lerp(bottom, top, 0.5f) + y1);

                // Create quad between v0 and v1 with thickness
                Vector2 dir = (v1 - v0).normalized;
                Vector2 normal = new Vector2(-dir.y, dir.x) * (lineThickness * 0.5f);

                UIVertex vert = UIVertex.simpleVert;
                vert.color = col;

                // Four corners of the quad
                vert.position = v0 + normal;
                vh.AddVert(vert);
                vert.position = v0 - normal;
                vh.AddVert(vert);
                vert.position = v1 + normal;
                vh.AddVert(vert);
                vert.position = v1 - normal;
                vh.AddVert(vert);

                int baseIndex = (i * 4) + (pass * (points - 1) * 4);
                // Add two triangles for quad
                vh.AddTriangle(baseIndex + 0, baseIndex + 2, baseIndex + 1);
                vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 1);
            }
        }
    }

#if UNITY_EDITOR
    // Editor helper to repaint while editing parameters
    protected override void OnValidate()
    {
        base.OnValidate();
        SetAllDirty();
    }
#endif
}
