using UnityEngine;
using SELA.Core;

namespace SELA.Gameplay
{
    public class RoomStateVisualizer : MonoBehaviour
    {
        [Header("3D Evidence Cards on Papan Bukti")]
        [SerializeField] GameObject cardE0;
        [SerializeField] GameObject cardE1;
        [SerializeField] GameObject cardE2;

        [Header("Desk Clutter & Tidy State")]
        [SerializeField] GameObject deskClutterGroup;
        [SerializeField] GameObject deskTidyStack;
        [SerializeField] GameObject rejectionPaperExtra;

        [Header("Offer Folder & Bus Ticket")]
        [SerializeField] GameObject offerFolderObject;
        [SerializeField] GameObject busTicketObject;

        bool subscribed;

        void Start()
        {
            TrySubscribe();
        }

        void Update()
        {
            if (!subscribed)
                TrySubscribe();
        }

        void OnDestroy()
        {
            if (subscribed && GameLoopController.Instance != null && GameLoopController.Instance.Engine != null)
                GameLoopController.Instance.Engine.OnStateChanged -= Refresh;
        }

        void TrySubscribe()
        {
            if (GameLoopController.Instance == null || GameLoopController.Instance.Engine == null) return;
            GameLoopController.Instance.Engine.OnStateChanged -= Refresh;
            GameLoopController.Instance.Engine.OnStateChanged += Refresh;
            subscribed = true;
            Refresh(GameLoopController.Instance.State);
        }

        public void Refresh(GameState state)
        {
            if (state == null) return;

            if (cardE0 != null) cardE0.SetActive(state.hasE0);
            if (cardE1 != null) cardE1.SetActive(state.hasE1);
            if (cardE2 != null) cardE2.SetActive(state.hasE2);

            bool hasRejection = (state.applicationA != null && state.applicationA.status == ApplicationStatus.Rejected) ||
                                (state.applicationB != null && state.applicationB.status == ApplicationStatus.Rejected);

            if (deskClutterGroup != null) deskClutterGroup.SetActive(!state.isDeskTidy);
            if (deskTidyStack != null) deskTidyStack.SetActive(state.isDeskTidy);
            if (rejectionPaperExtra != null) rejectionPaperExtra.SetActive(hasRejection && !state.isDeskTidy);

            bool showOfferFolder = state.day >= GameState.EarlyFinalizationMinDay ||
                                   state.HasOffer(CompanyId.StudioLajur) ||
                                   state.HasOffer(CompanyId.KoperasiPijar);
            if (offerFolderObject != null) offerFolderObject.SetActive(showOfferFolder);
            if (busTicketObject != null) busTicketObject.SetActive(true);
        }
    }
}
