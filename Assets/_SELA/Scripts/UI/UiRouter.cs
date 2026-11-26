using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace SELA.UI
{
    /// <summary>
    /// Holder stack panel, mengatur fokus, cursor, dan pemblokiran input dunia selama panel terbuka (DESIGN 9.1).
    /// </summary>
    public class UiRouter : MonoBehaviour
    {
        public static UiRouter Instance { get; private set; }

        private readonly List<UiScreen> stack = new List<UiScreen>();
        private readonly Dictionary<UiScreen, RectTransform> built = new Dictionary<UiScreen, RectTransform>();

        public RectTransform ScreenRoot { get; private set; }

        /// <summary>Dipanggil saat tidak ada panel terbuka dan pemain menekan Esc di gameplay.</summary>
        public System.Action OnEscapeInGameplay;

        /// <summary>Dipanggil setiap kali ada panel terbuka atau tertutup. Nilainya true bila dunia diblokir.</summary>
        public System.Action<bool> OnBlockedChanged;

        public bool AnyOpen
        {
            get { return stack.Count > 0; }
        }

        /// <summary>
        /// True bila ada panel modal yang terbuka. HUD bukan modal, jadi dunia tetap bisa
        /// dipakai while HUD tampil (DESIGN 9.4).
        /// </summary>
        public bool AnyModalOpen
        {
            get { return TopModal() != null; }
        }

        private UiScreen TopModal()
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i].IsModal)
                {
                    return stack[i];
                }
            }
            return null;
        }

        public void Initialise(RectTransform screenRoot)
        {
            Instance = this;
            ScreenRoot = screenRoot;
        }

        public void Register(UiScreen screen, RectTransform parent)
        {
            screen.Initialise(this, parent);
            built[screen] = screen.Root;
        }

        public void Open(UiScreen screen)
        {
            if (screen.IsOpen)
            {
                return;
            }

            screen.OnOpen();
            stack.Add(screen);
            ApplyInputState();
            screen.FocusFirst();
        }

        public void Close(UiScreen screen)
        {
            if (!stack.Contains(screen))
            {
                return;
            }

            stack.Remove(screen);
            screen.OnClose();
            ApplyInputState();

            if (stack.Count > 0)
            {
                stack[stack.Count - 1].FocusFirst();
            }
        }

        public void CloseTop()
        {
            UiScreen top = TopModal();
            if (top == null)
            {
                return;
            }

            if (top.CloseFromEscape())
            {
                Close(top);
            }
        }

        public void CloseAll()
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                stack[i].OnClose();
            }
            stack.Clear();
            ApplyInputState();
        }

        public bool IsOpen(UiScreen screen)
        {
            return screen != null && screen.IsOpen;
        }

        /// <summary>Menyegarkan semua panel yang sedang terbuka, dipakai saat state berubah.</summary>
        public void RefreshOpen()
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                stack[i].Refresh();
            }
        }

        private void ApplyInputState()
        {
            bool blocked = AnyModalOpen;
            PlayerInteractor.Blocked = blocked;

            if (blocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (OnBlockedChanged != null)
            {
                OnBlockedChanged(blocked);
            }
        }

        private void Update()
        {
            if (EventSystem.current == null) { return; }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) { return; }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                if (AnyModalOpen)
                {
                    CloseTop();
                }
                else if (OnEscapeInGameplay != null)
                {
                    OnEscapeInGameplay();
                }
            }
        }
    }
}
