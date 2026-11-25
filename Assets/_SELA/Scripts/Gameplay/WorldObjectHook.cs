using UnityEngine;
using SELA.Core;

/// <summary>
/// Connects a 3D room prop's Interactable event to the core loop & temporary/final UI.
/// Automatically updates dynamic prompts (such as the Bed switching to "Tutup hari" at night).
/// </summary>
[RequireComponent(typeof(Interactable))]
public class WorldObjectHook : MonoBehaviour
{
    public enum StationKind
    {
        Laptop,
        EvidenceBoard,
        Bed,
        Phone,
        Desk,
        WindowChair,
        Calendar
    }

    public StationKind station = StationKind.Laptop;

    Interactable interactable;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    void OnEnable()
    {
        if (GameLoopController.Instance != null && GameLoopController.Instance.Engine != null)
        {
            GameLoopController.Instance.Engine.OnStateChanged += RefreshPrompt;
            RefreshPrompt(GameLoopController.Instance.State);
        }
    }

    void Start()
    {
        if (GameLoopController.Instance != null && GameLoopController.Instance.Engine != null)
        {
            GameLoopController.Instance.Engine.OnStateChanged -= RefreshPrompt;
            GameLoopController.Instance.Engine.OnStateChanged += RefreshPrompt;
            RefreshPrompt(GameLoopController.Instance.State);
        }
    }

    void OnDisable()
    {
        if (GameLoopController.Instance != null && GameLoopController.Instance.Engine != null)
            GameLoopController.Instance.Engine.OnStateChanged -= RefreshPrompt;
    }

    public void TriggerStation()
    {
        if (GameLoopController.Instance == null) return;
        GameLoopController.Instance.RequestWorldFocus(station.ToString());
    }

    void RefreshPrompt(GameState state)
    {
        if (interactable == null || state == null) return;

        switch (station)
        {
            case StationKind.Laptop:
                interactable.prompt = "Buka laptop";
                break;
            case StationKind.EvidenceBoard:
                interactable.prompt = "Lihat papan bukti";
                break;
            case StationKind.Bed:
                if (state.IsNightFinished)
                    interactable.prompt = state.day >= GameState.MaxDay ? "Tinjau keputusan terakhir" : "Tutup hari";
                else
                    interactable.prompt = "Beristirahat";
                break;
            case StationKind.Phone:
                interactable.prompt = "Periksa ponsel / Hubungi Dita";
                break;
            case StationKind.Desk:
                interactable.prompt = state.deskTidiedToday ? "Lihat meja" : "Rapikan meja";
                break;
            case StationKind.WindowChair:
                interactable.prompt = "Duduk tanpa layar";
                break;
            case StationKind.Calendar:
                interactable.prompt = $"Lihat kalender (Hari {state.day}/7)";
                break;
        }
    }
}
