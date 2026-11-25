using UnityEngine;
using SELA.Core;

/// <summary>
/// Connects a 3D room prop's Interactable event to the core loop & temporary/final UI.
/// Automatically updates dynamic prompts (such as the Bed switching to "Tidur / Tutup hari" at 18:00+).
/// </summary>
[RequireComponent(typeof(Interactable))]
public class WorldObjectHook : MonoBehaviour
{
    public enum StationKind
    {
        Laptop,
        EvidenceBoard,
        Bed,
        Bathroom,
        Phone,
        DitaRoom,
        Desk,
        WindowChair,
        BagAndOffers,
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
                if (state.CanSleepNow)
                    interactable.prompt = state.day >= GameState.MaxDay ? "Tinjau keputusan terakhir" : "Tidur / Tutup hari";
                else
                    interactable.prompt = "Beristirahat · 2 jam";
                break;
            case StationKind.Bathroom:
                interactable.prompt = state.showeredToday ? "Kamar mandi (sudah mandi hari ini)" : "Mandi · 1 jam";
                break;
            case StationKind.Phone:
                interactable.prompt = "Periksa ponsel / Hubungi Dita";
                break;
            case StationKind.DitaRoom:
                interactable.prompt = "Temui Dita (Kamar 06) · 2 jam";
                break;
            case StationKind.Desk:
                interactable.prompt = state.deskTidiedToday ? "Lihat meja" : "Rapikan meja · 1 jam";
                break;
            case StationKind.WindowChair:
                interactable.prompt = "Duduk tanpa layar · 1 jam";
                break;
            case StationKind.BagAndOffers:
                interactable.prompt = state.day >= GameState.EarlyFinalizationMinDay
                    ? "Map tawaran & Kemas barang"
                    : "Lihat map tawaran";
                break;
            case StationKind.Calendar:
                interactable.prompt = $"Lihat kalender (Hari {state.day}/7 · {state.FormattedTime})";
                break;
        }
    }
}
