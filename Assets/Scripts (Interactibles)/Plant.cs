using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlantStage { Seedling, Sapling, Mature, Ancient }

[RequireComponent(typeof(SpriteRenderer))]
public class Plant : MonoBehaviour, IPointerClickHandler
{
    [Header("One sprite per stage (Seedling, Sapling, Mature, Ancient)")]
    [SerializeField] private Sprite[] stageSprites;
    [SerializeField] private PlantStage startStage = PlantStage.Seedling;
    [SerializeField] private float maxInteractDistance = 3f;

    public PlantStage Stage { get; private set; }
    public bool WateredToday { get; private set; }

    private SpriteRenderer sr;
    private bool busy;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetStage(startStage);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (busy) return;

        var tools = PlayerTools.Instance;
        if (!tools.HasWateringCan) { Debug.Log("You need a watering can."); return; }
        if (WateredToday) { Debug.Log("Already watered today."); return; }
        if (IsMaxStage) { Debug.Log("Fully grown."); return; }
        if (Vector2.Distance(tools.transform.position, transform.position) > maxInteractDistance)
        { Debug.Log("Too far away."); return; }

        ConfirmDialog.Instance.Show("Water this plant?", () => StartCoroutine(WaterRoutine()));
    }

    private IEnumerator WaterRoutine()
    {
        busy = true;
        var tools = PlayerTools.Instance;
        bool fromLeft = tools.transform.position.x < transform.position.x;

        yield return tools.ShowCan(transform.position, fromLeft); // can appears
        yield return tools.Pour();                                // watering animation
        Grow();                                                   // stage changes
        yield return tools.HideCan();                             // can goes back to inventory

        busy = false;
    }

    private void Grow()
    {
        WateredToday = true;
        if (!IsMaxStage) SetStage(Stage + 1);
    }

    // Call this from your day/sleep system
    public void OnNewDay() => WateredToday = false;

    private bool IsMaxStage => (int)Stage >= stageSprites.Length - 1;

    private void SetStage(PlantStage stage)
    {
        Stage = stage;
        int i = Mathf.Clamp((int)stage, 0, stageSprites.Length - 1);
        sr.sprite = stageSprites[i];
    }
}