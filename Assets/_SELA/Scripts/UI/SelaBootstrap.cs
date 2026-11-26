using System.Collections.Generic;
using System.Reflection;
using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Satu-satunya MonoBehaviour UI di scene. Membangun canvas, mendaftarkan semua layar,
    /// dan menjembatani state permainan dengan panel.
    /// </summary>
    [DisallowMultipleComponent]
    public class SelaBootstrap : MonoBehaviour
    {
        public static SelaBootstrap Instance { get; private set; }

        private UiRouter _router;
        private Canvas _canvas;
        private RectTransform _screenRoot;

        private MainMenuScreen _mainMenu;
        private PauseScreen _pause;
        private SettingsScreen _settings;
        private CreditsScreen _credits;
        private HelpScreen _help;
        private ContentInfoScreen _contentInfo;
        private HudScreen _hud;
        private NotesScreen _notes;
        private LaptopScreen _laptop;
        private CvScreen _cv;
        private ApplicationSnapshotScreen _snapshot;
        private VerificationScreen _verification;
        private InterviewScreen _interview;
        private ActivityPanel _activity;
        private BreakScreen _break;
        private DecisionScreen _decision;
        private EpilogScreen _epilog;
        private RiftPanel _rift;
        private ConfirmDialog _confirm;
        private PhoneScreen _phone;
        private CalendarScreen _calendar;
        private BedScreen _bed;
        private RoomMenuScreen _roomMenu;
        private EvidenceBoardScreen _evidenceBoard;
        private TutorialScreen _tutorial;

        private PlayerInteractor _interactor;
        private FirstPersonController _controller;
        private Camera _playerCamera;

        /// <summary>Disimpan sebagai field Unity supaya tetap benar setelah domain reload.</summary>
        [SerializeField]
        private bool _gameplayActive;

        private void Awake()
        {
            BuildUi();
        }

        /// <summary>
        /// Dipanggil lagi setelah domain reload, yaitu ketika skrip diubah selagi game berjalan.
        /// Awake tidak dijalankan ulang, jadi UI lama harus dibuang dan dibangun ulang supaya
        /// tombol dan panel tetap berfungsi.
        /// </summary>
        private void OnEnable()
        {
            if (Instance != this)
            {
                BuildUi();
            }
        }

        private void BuildUi()
        {
            Instance = this;

            // Buang UI dari sebelum reload, kalau ada.
            Transform previous = transform.Find("SELA UI");
            if (previous != null)
            {
                previous.gameObject.SetActive(false);
                Destroy(previous.gameObject);
            }

            BuildCanvas();
            BuildEventSystem();
            BuildScreens();
            CacheSceneObjects();
            ApplySettings();
            HookState();

            // Kalau permainan sedang berjalan, HUD dibuka lagi supaya pemain tidak
            // melihat layar kosong setelah skrip diubah.
            if (_gameplayActive)
            {
                _router.Open(_hud);
                _hud.Refresh();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void BuildCanvas()
        {
            GameObject canvasGo = new GameObject("SELA UI", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvasGo.layer = LayerMask.NameToLayer("UI");

            _canvas = canvasGo.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(UiTheme.ReferenceWidth, UiTheme.ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            _screenRoot = UiKit.Rect(canvasGo.transform, "Screens");

            _router = canvasGo.GetComponent<UiRouter>() ?? canvasGo.AddComponent<UiRouter>();
            _router.Initialise(_screenRoot);
            _router.OnEscapeInGameplay = TogglePause;
            _router.OnBlockedChanged = OnBlockedChanged;
        }

        private void BuildEventSystem()
        {
            EventSystem existing = FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
                if (existing.GetComponent<InputSystemUIInputModule>() == null)
                {
                    existing.gameObject.AddComponent<InputSystemUIInputModule>();
                }
                return;
            }

            GameObject go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            go.transform.SetParent(transform, false);
        }

        private void BuildScreens()
        {
            _mainMenu = Register(new MainMenuScreen());
            _pause = Register(new PauseScreen());
            _settings = Register(new SettingsScreen());
            _credits = Register(new CreditsScreen());
            _help = Register(new HelpScreen());
            _contentInfo = Register(new ContentInfoScreen());
            _hud = Register(new HudScreen());
            _notes = Register(new NotesScreen());
            _laptop = Register(new LaptopScreen());
            _cv = Register(new CvScreen());
            _snapshot = Register(new ApplicationSnapshotScreen());
            _verification = Register(new VerificationScreen());
            _interview = Register(new InterviewScreen());
            _activity = Register(new ActivityPanel());
            _break = Register(new BreakScreen());
            _decision = Register(new DecisionScreen());
            _epilog = Register(new EpilogScreen());
            _rift = Register(new RiftPanel());
            _confirm = Register(new ConfirmDialog());
            _phone = Register(new PhoneScreen());
            _calendar = Register(new CalendarScreen());
            _bed = Register(new BedScreen());
            _roomMenu = Register(new RoomMenuScreen());
            _evidenceBoard = Register(new EvidenceBoardScreen());
            _tutorial = Register(new TutorialScreen());
        }

        private T Register<T>(T screen) where T : UiScreen
        {
            _router.Register(screen, _screenRoot);
            return screen;
        }

        private void CacheSceneObjects()
        {
            _interactor = FindFirstObjectByType<PlayerInteractor>();
            _controller = FindFirstObjectByType<FirstPersonController>();
            _playerCamera = FindFirstObjectByType<Camera>();

            if (_interactor != null && _interactor.promptText != null)
            {
                // Prompt lama memakai font Arial; HUD baru memakai TMP (DESIGN 9.1).
                _interactor.promptText.gameObject.SetActive(false);
            }

            GameObject legacy = GameObject.Find("Interaction HUD");
            if (legacy != null)
            {
                legacy.SetActive(false);
            }
        }

        private void HookState()
        {
            SelaState.Changed += OnStateChanged;
            UiSettings.Changed += ApplySettings;
        }

        private void OnStateChanged()
        {
            _router.RefreshOpen();
            if (_hud != null && _hud.IsOpen)
            {
                _hud.Refresh();
            }
        }

        /// <summary>
        /// Selama panel terbuka, gerakan dan penoleh kamera berhenti supaya klik pada tombol
        /// UI tidak dipakai ulang untuk mengunci cursor (DESIGN 9.4).
        /// </summary>
        private void OnBlockedChanged(bool blocked)
        {
            if (_controller != null)
            {
                _controller.enabled = !blocked;
            }
        }

        private void Start()
        {
            if (SelaState.Current.SeenContentInfo)
            {
                ShowMainMenu();
            }
            else
            {
                SelaState.Current.SeenContentInfo = true;
                _router.Open(_contentInfo);
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (_gameplayActive && !_router.AnyModalOpen)
            {
                if (keyboard.tabKey.wasPressedThisFrame)
                {
                    _router.Open(_notes);
                }

                // Jalan cepat ke isi kamar, dipakai sementara trigger benda 3D belum siap.
                if (keyboard.mKey.wasPressedThisFrame)
                {
                    _router.Open(_roomMenu);
                }
            }

            if (_hud != null && _hud.IsOpen)
            {
                _hud.SetPrompt(CurrentPrompt());
            }
        }

        /// <summary>Nomor retakan yang sedang berlangsung, dipakai panel Jeda (DESIGN 9.14).</summary>
        public int ActiveRift { get; private set; } = -1;

        private static readonly FieldInfo InteractorTargetField = typeof(PlayerInteractor)
            .GetField("target", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo InteractablePromptField = typeof(Interactable)
            .GetField("prompt", BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Prompt memakai nama aksi dari Interactable (misal "Baca catatan"), bukan kalimat
        /// umum, supaya pemain tahu apa yang akan terjadi (DESIGN 9.4).
        /// </summary>
        private string CurrentPrompt()
        {
            if (_interactor == null)
            {
                return string.Empty;
            }

            if (SeatInteraction.Current != null)
            {
                return "Q - Berdiri";
            }

            if (InteractorTargetField != null)
            {
                var target = InteractorTargetField.GetValue(_interactor) as Interactable;
                if (target != null)
                {
                    string prompt = InteractablePromptField != null
                        ? InteractablePromptField.GetValue(target) as string
                        : null;
                    if (!string.IsNullOrEmpty(prompt))
                    {
                        return SelaContent.Ui.PromptKey + " · " + prompt;
                    }
                    return SelaContent.Ui.PromptKey + " - Interaksi";
                }
            }

            return string.Empty;
        }

        private void ApplySettings()
        {
            if (_playerCamera != null)
            {
                _playerCamera.fieldOfView = UiSettings.FieldOfView;
            }
            ApplySensitivity();
        }

        /// <summary>
        /// sensitivitas dan invert Y dibaca lewat field privat FirstPersonController, supaya file milik
        /// anggota Gameplay tidak perlu diubah. Kalau field-nya berubah nama, pengaturan ini dilewati diam-diam.
        /// </summary>
        private void ApplySensitivity()
        {
            if (_controller == null)
            {
                return;
            }

            FieldInfo field = typeof(FirstPersonController).GetField("mouseSensitivity",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                // Pengaturan memakai pengali 0,2 sampai 3; nilai dasar controller adalah 0,1.
                field.SetValue(_controller, BaseSensitivity * UiSettings.Sensitivity);
            }

            FieldInfo invert = typeof(FirstPersonController).GetField("invertY",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (invert != null)
            {
                invert.SetValue(_controller, UiSettings.InvertY);
            }
        }

        private const float BaseSensitivity = 0.1f;

        // -- Pembuka layar dari luar -------------------------------------------------

        public void ShowMainMenu()
        {
            _gameplayActive = false;
            _router.CloseAll();
            _router.Open(_mainMenu);
        }

        public void OnNewGameStarted()
        {
            _gameplayActive = true;
            _router.Open(_hud);
            _hud.Refresh();

            // Pengenalan pertama muncul sekali, urutan langkahnya sesuai DESIGN 9.15.
            if (!SelaState.Current.TutorialSeen)
            {
                OpenTutorial();
            }
        }

        public void OnGameResumed()
        {
            _gameplayActive = true;
            _router.Open(_hud);
            _hud.Refresh();
        }

        public void ContinueAfterInfo()
        {
            _router.Close(_contentInfo);
            ShowMainMenu();
        }

        public void OpenSettingsFromMenu()
        {
            _router.Open(_settings);
        }

        public void OpenSettingsFromPause()
        {
            _router.Open(_settings);
        }

        public void OpenSettingsFromInfo()
        {
            _router.Open(_settings);
        }

        public void OpenCreditsFromMenu()
        {
            _router.Open(_credits);
        }

        public void OpenHelp()
        {
            _router.Open(_help);
        }

        public void OpenCv()
        {
            _router.Open(_cv);
        }

        public void OpenApplicationSnapshot(OfferId offer)
        {
            _snapshot.SetOffer(offer);
            _router.Open(_snapshot);
        }

        public void OpenVerification(OfferId offer)
        {
            _verification.SetOffer(offer);
            _router.Open(_verification);
        }

        public void OpenInterview(OfferId offer)
        {
            _interview.SetOffer(offer);
            _router.Open(_interview);
        }

        public void OpenActivity(SelaActivity activity)
        {
            _activity.SetActivity(activity);
            _router.Open(_activity);
        }

        public void OpenBreak()
        {
            _router.Open(_break);
        }

        public void OpenRift(int rift)
        {
            ActiveRift = rift;
            _rift.SetRift(rift);
            _router.Open(_rift);
        }

        /// <summary>
        /// Satu dialog konfirmasi untuk semua peringatan: mulai baru, ulang hari, keluar,
        /// dan menyelesaikan adegan (DESIGN 9.14).
        /// </summary>
        public void AskConfirm(string title, string body, System.Action onConfirm)
        {
            _confirm.Ask(title, body, onConfirm);
            _router.Open(_confirm);
        }

        /// <summary>
        /// Adegan retakan diselesaikan tanpa memakai sesi. Hasil ceritanya sama (DESIGN 9.12).
        /// </summary>
        public void CompleteRiftScene()
        {
            SelaState state = SelaState.Current;
            if (ActiveRift >= 1 && ActiveRift <= state.RiftSolved.Length)
            {
                state.RiftSolved[ActiveRift - 1] = true;
            }
            ActiveRift = -1;
            if (_rift.IsOpen)
            {
                _router.Close(_rift);
            }
            AdvanceSessionAfterActivity();
        }

        /// <summary>
        /// Sesi berpindah setelah aktivitas selesai dan peristiwanya sudah ditangani.
        /// Malam tetap Malam, dan checkpoint langsung disimpan (GDD 3.2).
        /// </summary>
        public void AdvanceSessionAfterActivity()
        {
            SelaState state = SelaState.Current;
            SelaSessionFlow.AdvanceSession(state);
            state.NotifyChanged();
            SelaSaveStore.Write(state);
        }

        /// <summary>Menampilkan kembali panel retakan dari panel Jeda.</summary>
        public void ShowRiftHint()
        {
            if (ActiveRift < 0)
            {
                return;
            }
            if (_pause.IsOpen)
            {
                _router.Close(_pause);
            }
            _router.Open(_rift);
        }

        public void OpenDecision()
        {
            _router.Open(_decision);
        }

        public void OpenEpilog()
        {
            _router.Close(_decision);
            _router.Open(_epilog);
        }

        public void OpenLaptop()
        {
            _router.Open(_laptop);
        }

        /// <summary>Membuka laptop pada tab pesan, dipakai dari panel ponsel.</summary>
        public void OpenLaptopMessages()
        {
            _laptop.ShowMessages();
            _router.Open(_laptop);
        }

        public void OpenPhone()
        {
            _router.Open(_phone);
        }

        public void OpenCalendar()
        {
            _router.Open(_calendar);
        }

        public void OpenBed()
        {
            _router.Open(_bed);
        }

        public void OpenNotes()
        {
            _router.Open(_notes);
        }

        public void OpenRoomMenu()
        {
            _router.Open(_roomMenu);
        }

        public void OpenEvidenceBoard()
        {
            _router.Open(_evidenceBoard);
        }

        public void OpenTutorial()
        {
            _router.Open(_tutorial);
        }

        public void TogglePause()
        {
            if (_router.IsOpen(_pause))
            {
                _router.Close(_pause);
            }
            else
            {
                _router.Open(_pause);
            }
        }

        public void RestartDay()
        {
            SelaState state = SelaState.Current;
            int day = state.Day;

            AskConfirm(SelaContent.Ui.PauseItems[2],
                SelaContent.Ui.RestartDayWarning + " (Hari " + day + ")",
                () =>
                {
                    state.Session = SelaSession.Pagi;
                    state.SessionResolved = false;
                    state.TidiedToday = false;
                    state.BreakUsedToday = false;
                    state.NotifyChanged();
                    SelaSaveStore.Write(state);
                    _router.Close(_pause);
                });
        }

        public void ReturnToMenu()
        {
            AskConfirm(SelaContent.Ui.PauseItems[3], SelaContent.Ui.ExitConfirm, () =>
            {
                SelaSaveStore.Write(SelaState.Current);
                ShowMainMenu();
            });
        }
    }
}
