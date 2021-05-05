using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thanks august_unity496 for providing this script at https://answers.unity.com/questions/361573/how-to-make-a-texture-tile-and-not-stretch.html
public class GifTexture : MonoBehaviour
{
    [SerializeField] private float tileX = 1;
    [SerializeField] private float tileY = 1;

    Mesh mesh;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mesh = GetComponent<MeshFilter>().mesh;

    }

    void Update()
    {
        mat.mainTextureScale = new Vector2((mesh.bounds.size.x *
transform.localScale.x) / 100 * tileX, (mesh.bounds.size.y * transform.localScale.y) / 100 * tileY);
    }
}
