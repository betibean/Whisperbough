using System.Collections;
using UnityEngine;

public class PlayerTools : MonoBehaviour
{
    public static PlayerTools Instance { get; private set; }

    [Tooltip("Child object with a SpriteRenderer using 'Layer 36 Copy 4'. Keep it disabled.")]
    [SerializeField] private GameObject wateringCanVisual;

    public bool HasWateringCan { get; private set; }

    [SerializeField] private bool startWithCan;

    private void Awake()
    {
        Instance = this;
        HasWateringCan = startWithCan;
        wateringCanVisual.SetActive(false);
    }

    public void PickUpWateringCan() => HasWateringCan = true;

    public IEnumerator ShowCan(Vector3 plantPos, bool fromLeft)
    {
        Transform t = wateringCanVisual.transform;
        float side = fromLeft ? -1f : 1f;

        t.position = plantPos + new Vector3(side * 0.9f, 0.8f, 0f);
        t.localScale = new Vector3(side * -Mathf.Abs(t.localScale.x), t.localScale.y, 1f); // spout faces plant
        t.rotation = Quaternion.identity;
        wateringCanVisual.SetActive(true);
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator Pour(float duration = 1.2f)
    {
        Transform t = wateringCanVisual.transform;
        float dir = Mathf.Sign(-t.localScale.x); // tilt toward the plant
        float tilt = 28f * dir;

        yield return Rotate(t, 0f, tilt, 0.35f);
        yield return new WaitForSeconds(duration);   // <- spawn water particles here
        yield return Rotate(t, tilt, 0f, 0.3f);
    }

    public IEnumerator HideCan()
    {
        yield return new WaitForSeconds(0.15f);
        wateringCanVisual.SetActive(false); // back to the inventory
    }

    private static IEnumerator Rotate(Transform t, float from, float to, float time)
    {
        for (float e = 0; e < time; e += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0, 1, e / time);
            t.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(from, to, k));
            yield return null;
        }
        t.rotation = Quaternion.Euler(0, 0, to);
    }
}