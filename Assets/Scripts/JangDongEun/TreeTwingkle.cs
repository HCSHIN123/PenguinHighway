using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TreeTwingkle : MonoBehaviour
{
    private MeshRenderer mesh;
    private Material mat;
    private float ground = 0f;
    private float maxHeight = 2f;
    private float twingkleSpeed = 5f;
    private float twingkleBright = 1f;
    private bool isTwinkle = false;
    private Color color = Color.white;
    private WaitForSeconds waits = new WaitForSeconds(1f);

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.sharedMaterial;
    }

    private void Update()
    {
        float emission = ground + Mathf.PingPong(Time.time * twingkleSpeed, maxHeight);
        Color baseColor = color;
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor * twingkleBright);
    }

    private IEnumerator TreeSprinkle()
    {
        while (isTwinkle)
        {
            Color background = new Color(1, 0, 0);
            color = background;
            yield return waits;
            color = Color.white;
            yield return waits;
            background = new Color(0, 1, 0);
            color = background;
            yield return waits;
            color = Color.white;
            yield return waits;
            twingkleBright = 32f;
            background = new Color(0, 0, 1f);
            color = background;
            yield return waits;
            color = Color.white;
            twingkleBright = 1f;
            yield return waits;
            isTwinkle = false;

            yield break;
        }
    }

    public void IsTwinkle()
    {
        isTwinkle = !isTwinkle;
        color = Color.white;

        StopAllCoroutines();
        StartCoroutine("TreeSprinkle");
    }
}
