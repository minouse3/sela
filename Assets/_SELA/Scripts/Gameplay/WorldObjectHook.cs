using UnityEngine;
using SELA.Core;

[RequireComponent(typeof(Interactable))]
public class WorldObjectHook : MonoBehaviour
{
    public enum StationKind
    {
        Laptop,
        EvidenceBoard,
        Bed,
        Bathroom,
        DitaRoom = 5,
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
        var loop = GameLoopController.Instance;
        if (loop == null) return;
        if (station == StationKind.DitaRoom)
        {
            if (loop.TryGetComponent<CoreLoopDebugUI>(out var panel)) panel.OpenDita(transform);
            return;
        }
        loop.RequestWorldFocus(station.ToString());
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
                interactable.prompt = "Papan bukti";
                break;
            case StationKind.Bed:
                if (state.CanSleepNow)
                    interactable.prompt = state.day >= GameState.MaxDay ? "Keputusan akhir" : "Tidur";
                else
                    interactable.prompt = "Istirahat (2 jam)";
                break;
            case StationKind.Bathroom:
                interactable.prompt = state.showeredToday ? "Kamar mandi" : "Mandi (1 jam)";
                break;
            case StationKind.DitaRoom:
                interactable.prompt = "Ngobrol dengan Dita";
                break;
            case StationKind.Desk:
                interactable.prompt = state.deskTidiedToday ? "Lihat meja" : "Rapikan meja (1 jam)";
                break;
            case StationKind.WindowChair:
                interactable.prompt = "Duduk di jendela (1 jam)";
                break;
            case StationKind.BagAndOffers:
                interactable.prompt = state.day >= GameState.EarlyFinalizationMinDay ? "Kemas barang" : "Map tawaran";
                break;
            case StationKind.Calendar:
                interactable.prompt = "Lihat kalender";
                break;
        }
    }
}
