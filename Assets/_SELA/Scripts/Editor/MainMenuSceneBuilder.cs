using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SELA.UI;

namespace SELA.EditorTools
{
    public static class MainMenuSceneBuilder
    {
        public const string MainMenuScenePath = "Assets/_SELA/Scenes/MainMenu/MainMenu.unity";
        const string OfficialGameplayScenePath = "Assets/_SELA/Scenes/Gameplay/Kos_Main.unity";
        const string DefaultGameplayScenePath = "Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity";
        const string ProgressScenePath = "Assets/progress.unity";
        const string PostProcessProfilePath = "Assets/_SELA/Settings/SELA_PostProcessProfile.asset";

        static readonly Color ColPaper = Hex("#F2E7CF");
        static readonly Color ColPaperFold = Hex("#D8C8A8");
        static readonly Color ColBtnNormal = Hex("#FAF3E3");
        static readonly Color ColBtnHover = Hex("#E6D5B4");
        static readonly Color ColBtnPressed = Hex("#D4BE95");
        static readonly Color ColBtnQuit = Hex("#EFE0D6");
        static readonly Color ColInk = Hex("#2D3A34");
        static readonly Color ColMutedInk = Hex("#5A6660");
        static readonly Color ColClay = Hex("#AE7462");
        static readonly Color ColInkBlue = Hex("#596C80");
        static readonly Color ColShadow = new Color(0.176f, 0.227f, 0.204f, 0.34f);

        [MenuItem("SELA/Build Main Menu Scene")]
        public static void BuildMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            BuildBackdrop3D();
            BuildEventSystem();
            BuildMenuCanvas();

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(MainMenuScenePath));
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
            RegisterBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[SELA] MainMenu scene rebuilt in Cat Poster paper style: " + MainMenuScenePath);
        }

        static void BuildBackdrop3D()
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = Hex("#242E2A");
            cam.fieldOfView = 50f;
            camGo.AddComponent<AudioListener>();
            camGo.transform.position = new Vector3(2.95f, 1.44f, -1.02f);
            camGo.transform.rotation = Quaternion.Euler(6f, 136f, 0f);

            var urpCam = camGo.AddComponent<UniversalAdditionalCameraData>();
            urpCam.renderPostProcessing = true;
            urpCam.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            urpCam.antialiasingQuality = AntialiasingQuality.High;

            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(PostProcessProfilePath);
            if (profile != null)
            {
                var volGo = new GameObject("Global Volume");
                var vol = volGo.AddComponent<Volume>();
                vol.isGlobal = true;
                vol.sharedProfile = profile;
            }

            var skyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SELA/Art/Materials/M_SELA_Sky.mat");
            if (skyMat != null)
                RenderSettings.skybox = skyMat;

            var lightGo = new GameObject("Directional Light");
            var dirLight = lightGo.AddComponent<Light>();
            dirLight.type = LightType.Directional;
            dirLight.color = new Color(1.0f, 0.89f, 0.72f);
            dirLight.intensity = 1.75f;
            dirLight.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(22f, 258f, 0f);
            RenderSettings.sun = dirLight;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.48f, 0.52f, 0.57f);
            RenderSettings.ambientEquatorColor = new Color(0.46f, 0.41f, 0.35f);
            RenderSettings.ambientGroundColor = new Color(0.25f, 0.21f, 0.18f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.89f, 0.83f, 0.73f);
            RenderSettings.fogDensity = 0.0055f;

            var backdropRoot = new GameObject("MenuBackdrop_Kos");

            // Warm ceiling light + desk lamp light matching Kos_Main Kamar 07
            AddPointLight(backdropRoot.transform, "RoomCeilingLight", new Vector3(4.32f, 2.55f, -1.93f), new Color(1.0f, 0.88f, 0.72f), 1.15f, 6.5f);
            AddPointLight(backdropRoot.transform, "DeskLampLight", new Vector3(5.46f, 1.04f, -3.40f), new Color(1.0f, 0.84f, 0.62f), 0.75f, 3.0f);

            // Actual Kamar 07 room shell (SM_Kamar_Kiri) at its exact Kos_Main transform
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Environments/Kos/SM_Kamar_Kiri.fbx", new Vector3(-0.09f, 0f, 0.58f), 0f, preservePrefabRotation: true);

            // Room furniture & props (zero overlaps, aligned to SM_Kamar_Kiri walls & floor)
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Furniture/SM_Meja.fbx", new Vector3(4.90f, 0f, -3.44f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Electronics/SM_Laptop.fbx", new Vector3(4.92f, 0.752f, -3.37f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Electronics/SM_Ponsel.fbx", new Vector3(4.56f, 0.752f, -3.33f), 15f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Electronics/SM_Lampu_Meja.fbx", new Vector3(5.46f, 0.76f, -3.40f), -18f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Furniture/SM_Papan_Bukti.fbx", new Vector3(4.91f, 1.29f, -3.72f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Furniture/SM_Kursi_Jendela.fbx", new Vector3(5.04f, 0f, -2.72f), 172f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Environments/Kos/SM_Jendela.fbx", new Vector3(5.66f, 0.97f, -1.92f), -90f);

            var curtainL = PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Decorations/SM_Tirai.fbx", new Vector3(5.64f, 0.96f, -2.48f), -90f);
            if (curtainL != null) curtainL.transform.localScale = new Vector3(0.26f, 0.90f, 1f);
            var curtainR = PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Decorations/SM_Tirai.fbx", new Vector3(5.64f, 0.96f, -1.36f), -90f);
            if (curtainR != null) { curtainR.name = "SM_Tirai_R"; curtainR.transform.localScale = new Vector3(0.26f, 0.90f, 1f); }

            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Furniture/SM_Rak.fbx", new Vector3(3.79f, 0f, -3.73f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Documents/SM_Buku_Berdiri.fbx", new Vector3(3.67f, 0.795f, -3.69f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Documents/SM_Buku_Berdiri.fbx", new Vector3(3.87f, 0.795f, -3.69f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Documents/SM_Buku_Berdiri.fbx", new Vector3(3.69f, 0.41f, -3.69f), 0f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Documents/SM_Kalender.fbx", new Vector3(5.60f, 1.41f, -0.69f), -90f);
            PlaceFbx(backdropRoot.transform, "Assets/_SELA/Art/Props/Electronics/SM_Kipas.fbx", new Vector3(5.27f, 0f, -1.41f), -88f);

            // Clean courtyard backdrop visible through the open morning window
            var wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SELA/Art/Materials/M_CatPoster_RoomFinish.mat");
            var capMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SELA/Art/Materials/M_SELA_ExtRoof.mat");
            var yardWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yardWall.name = "MenuExterior_CourtyardWall";
            yardWall.transform.SetParent(backdropRoot.transform, false);
            yardWall.transform.position = new Vector3(11.5f, 1.0f, -2.0f);
            yardWall.transform.localScale = new Vector3(0.3f, 2.2f, 14f);
            if (wallMat != null) yardWall.GetComponent<MeshRenderer>().sharedMaterial = wallMat;

            var yardCap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yardCap.name = "MenuExterior_WallCap";
            yardCap.transform.SetParent(backdropRoot.transform, false);
            yardCap.transform.position = new Vector3(11.5f, 2.15f, -2.0f);
            yardCap.transform.localScale = new Vector3(0.4f, 0.14f, 14.1f);
            if (capMat != null) yardCap.GetComponent<MeshRenderer>().sharedMaterial = capMat;
        }

        static void AddPointLight(Transform parent, string name, Vector3 pos, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.None;
        }

        static GameObject PlaceFbx(Transform parent, string assetPath, Vector3 pos, float rotY, bool preservePrefabRotation = false)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null) return null;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            if (inst == null) return null;
            inst.transform.position = pos;
            if (!preservePrefabRotation)
                inst.transform.rotation = Quaternion.Euler(0f, rotY, 0f);
            return inst;
        }

        static void BuildEventSystem()
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        static void BuildMenuCanvas()
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

            var canvasGo = new GameObject("MainMenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            var controller = canvasGo.AddComponent<MainMenuController>();
            var so = new SerializedObject(controller);

            var stackGo = new GameObject("MenuColumn", typeof(RectTransform));
            stackGo.transform.SetParent(canvasGo.transform, false);
            var stackRt = stackGo.GetComponent<RectTransform>();
            stackRt.anchorMin = new Vector2(0f, 0.5f);
            stackRt.anchorMax = new Vector2(0f, 0.5f);
            stackRt.pivot = new Vector2(0f, 0.5f);
            stackRt.anchoredPosition = new Vector2(56f, 0f);
            stackRt.sizeDelta = new Vector2(360f, 580f);

            var vlg = stackGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10f;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var titleCard = CreatePaperCard("TitlePlaque", stackGo.transform, ColPaper, 134f, withDogEar: true);
            var titlePad = new GameObject("Inner", typeof(RectTransform));
            titlePad.transform.SetParent(titleCard.transform, false);
            var tpRt = titlePad.GetComponent<RectTransform>();
            tpRt.anchorMin = Vector2.zero;
            tpRt.anchorMax = Vector2.one;
            tpRt.offsetMin = new Vector2(22f, 14f);
            tpRt.offsetMax = new Vector2(-28f, -14f);

            var titleVlg = titlePad.AddComponent<VerticalLayoutGroup>();
            titleVlg.spacing = 5f;
            titleVlg.childAlignment = TextAnchor.MiddleLeft;
            titleVlg.childControlWidth = true;
            titleVlg.childControlHeight = false;
            titleVlg.childForceExpandWidth = true;

            var titleText = CreateText("TitleText", titlePad.transform, "S E  L A", font, 52f, FontStyles.Bold, ColInk, TextAlignmentOptions.Left);
            SetHeight(titleText.gameObject, 60f);

            var rule = CreatePanel("TitleRule", titlePad.transform, ColClay, false);
            SetHeight(rule, 3f);

            var tagline = CreateText("TaglineText", titlePad.transform, "Tujuh hari di Kamar 07.", font, 17f, FontStyles.Normal, ColMutedInk, TextAlignmentOptions.Left);
            SetHeight(tagline.gameObject, 26f);

            AddSpacer("TitleGap", stackGo.transform, 10f);

            // Clean stacked cream paper buttons (no AI subtitles!)
            var btnNew = CreatePaperMenuButton("NewGameButton", stackGo.transform, "Mulai Baru", null, font, ColBtnNormal, out _);
            var btnContinue = CreatePaperMenuButton("ContinueButton", stackGo.transform, "Lanjutkan", "...", font, ColBtnNormal, out var continueSub);
            var btnLoad = CreatePaperMenuButton("LoadGameButton", stackGo.transform, "Muat Simpanan", null, font, ColBtnNormal, out _);
            var btnSettings = CreatePaperMenuButton("SettingsButton", stackGo.transform, "Pengaturan", null, font, ColBtnNormal, out _);
            var btnCredits = CreatePaperMenuButton("CreditsButton", stackGo.transform, "Kredit", null, font, ColBtnNormal, out _);
            var btnQuit = CreatePaperMenuButton("QuitGameButton", stackGo.transform, "Keluar", null, font, ColBtnQuit, out _);

            AddSpacer("ToastGap", stackGo.transform, 10f);
            var toast = CreateText("StatusToast", stackGo.transform, "", font, 18f, FontStyles.Bold, ColPaper, TextAlignmentOptions.Left);
            var toastShadow = toast.gameObject.AddComponent<Shadow>();
            toastShadow.effectColor = new Color(0.10f, 0.14f, 0.12f, 0.85f);
            toastShadow.effectDistance = new Vector2(1.5f, -1.5f);
            SetHeight(toast.gameObject, 28f);

            // Sub-Panels (Cat Poster paper sheets)
            var loadPanel = BuildLoadPanel(canvasGo.transform, font,
                out var autosaveInfo, out var loadAutoBtn, out var dayStartInfo, out var loadDayBtn, out var closeLoadBtn);

            var confirmPanel = BuildConfirmNewGamePanel(canvasGo.transform, font,
                out var confirmYesBtn, out var confirmCancelBtn);

            var settingsPanel = BuildSettingsPanel(canvasGo.transform, font,
                out var masterSlider, out var masterVal,
                out var ambSlider, out var ambVal,
                out var sfxSlider, out var sfxVal,
                out var sensSlider, out var sensVal,
                out var fovSlider, out var fovVal,
                out var invertToggle, out var reduceToggle, out var lowAtmosToggle,
                out var textScaleBtn, out var textScaleVal,
                out var closeSettingsBtn);

            var creditsPanel = BuildCreditsPanel(canvasGo.transform, font, out var closeCreditsBtn);

            // Fullscreen Fade Overlay
            var fadeGo = CreatePanel("FadeOverlay", canvasGo.transform, new Color(0.08f, 0.11f, 0.10f, 1f), false);
            StretchFull(fadeGo.GetComponent<RectTransform>());
            var fadeGroup = fadeGo.AddComponent<CanvasGroup>();
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.interactable = false;

            so.FindProperty("gameplaySceneName").stringValue = "Kos_Main";
            so.FindProperty("newGameButton").objectReferenceValue = btnNew;
            so.FindProperty("continueButton").objectReferenceValue = btnContinue;
            so.FindProperty("continueSubtitleText").objectReferenceValue = continueSub;
            so.FindProperty("loadGameButton").objectReferenceValue = btnLoad;
            so.FindProperty("settingsButton").objectReferenceValue = btnSettings;
            so.FindProperty("creditsButton").objectReferenceValue = btnCredits;
            so.FindProperty("quitButton").objectReferenceValue = btnQuit;

            so.FindProperty("loadPanel").objectReferenceValue = loadPanel;
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("creditsPanel").objectReferenceValue = creditsPanel;
            so.FindProperty("confirmNewGamePanel").objectReferenceValue = confirmPanel;
            so.FindProperty("statusToastText").objectReferenceValue = toast;

            so.FindProperty("autosaveInfoText").objectReferenceValue = autosaveInfo;
            so.FindProperty("loadAutosaveButton").objectReferenceValue = loadAutoBtn;
            so.FindProperty("dayStartInfoText").objectReferenceValue = dayStartInfo;
            so.FindProperty("loadDayStartButton").objectReferenceValue = loadDayBtn;
            so.FindProperty("closeLoadButton").objectReferenceValue = closeLoadBtn;

            so.FindProperty("confirmNewGameYesButton").objectReferenceValue = confirmYesBtn;
            so.FindProperty("confirmNewGameCancelButton").objectReferenceValue = confirmCancelBtn;

            so.FindProperty("masterVolumeSlider").objectReferenceValue = masterSlider;
            so.FindProperty("masterVolumeValue").objectReferenceValue = masterVal;
            so.FindProperty("ambienceVolumeSlider").objectReferenceValue = ambSlider;
            so.FindProperty("ambienceVolumeValue").objectReferenceValue = ambVal;
            so.FindProperty("effectsVolumeSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("effectsVolumeValue").objectReferenceValue = sfxVal;
            so.FindProperty("sensitivitySlider").objectReferenceValue = sensSlider;
            so.FindProperty("sensitivityValue").objectReferenceValue = sensVal;
            so.FindProperty("fovSlider").objectReferenceValue = fovSlider;
            so.FindProperty("fovValue").objectReferenceValue = fovVal;
            so.FindProperty("invertYToggle").objectReferenceValue = invertToggle;
            so.FindProperty("reduceMotionToggle").objectReferenceValue = reduceToggle;
            so.FindProperty("lowAtmosphereToggle").objectReferenceValue = lowAtmosToggle;
            so.FindProperty("textScaleButton").objectReferenceValue = textScaleBtn;
            so.FindProperty("textScaleValue").objectReferenceValue = textScaleVal;
            so.FindProperty("closeSettingsButton").objectReferenceValue = closeSettingsBtn;

            so.FindProperty("closeCreditsButton").objectReferenceValue = closeCreditsBtn;
            so.FindProperty("fadeOverlay").objectReferenceValue = fadeGroup;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static GameObject BuildLoadPanel(
            Transform parent, TMP_FontAsset font,
            out TMP_Text autosaveInfo, out Button loadAutosaveBtn,
            out TMP_Text dayStartInfo, out Button loadDayStartBtn,
            out Button closeBtn)
        {
            var panel = CreateModalSheet("LoadGamePanel", parent, "Muat Simpanan", font, 580f, 390f, out var body);

            var h1 = CreateText("AutosaveHeading", body, "Autosave Terakhir", font, 19f, FontStyles.Bold, ColInk, TextAlignmentOptions.Left);
            SetHeight(h1.gameObject, 24f);
            autosaveInfo = CreateText("AutosaveInfo", body, "Belum ada autosave.", font, 16f, FontStyles.Normal, ColMutedInk, TextAlignmentOptions.Left);
            SetHeight(autosaveInfo.gameObject, 22f);
            loadAutosaveBtn = CreatePaperActionButton("LoadAutosaveButton", body, "Muat Autosave", font, ColBtnNormal);

            AddSpacer("LoadGap", body, 6f);

            var h2 = CreateText("DayStartHeading", body, "Awal Hari", font, 19f, FontStyles.Bold, ColInk, TextAlignmentOptions.Left);
            SetHeight(h2.gameObject, 24f);
            dayStartInfo = CreateText("DayStartInfo", body, "Belum ada simpanan awal hari.", font, 16f, FontStyles.Normal, ColMutedInk, TextAlignmentOptions.Left);
            SetHeight(dayStartInfo.gameObject, 22f);
            loadDayStartBtn = CreatePaperActionButton("LoadDayStartButton", body, "Ulangi Awal Hari (08:00)", font, ColBtnNormal);

            AddSpacer("LoadFooterGap", body, 8f);
            closeBtn = CreatePaperActionButton("CloseLoadButton", body, "Kembali", font, ColPaperFold);

            panel.SetActive(false);
            return panel;
        }

        static GameObject BuildConfirmNewGamePanel(
            Transform parent, TMP_FontAsset font,
            out Button yesBtn, out Button cancelBtn)
        {
            var panel = CreateModalSheet("ConfirmNewGamePanel", parent, "Mulai Baru?", font, 500f, 280f, out var body);

            var desc = CreateText("ConfirmDesc", body,
                "Simpanan lama akan diganti ke Hari 1 · 08:00.\nPengaturan tetap aman.",
                font, 17f, FontStyles.Normal, ColInk, TextAlignmentOptions.Left);
            SetHeight(desc.gameObject, 52f);

            AddSpacer("ConfirmGap", body, 6f);
            yesBtn = CreatePaperActionButton("ConfirmNewGameYesButton", body, "Ya, Mulai Baru", font, ColBtnQuit);
            cancelBtn = CreatePaperActionButton("ConfirmNewGameCancelButton", body, "Batal", font, ColBtnNormal);

            panel.SetActive(false);
            return panel;
        }

        static GameObject BuildSettingsPanel(
            Transform parent, TMP_FontAsset font,
            out Slider masterSlider, out TMP_Text masterVal,
            out Slider ambSlider, out TMP_Text ambVal,
            out Slider sfxSlider, out TMP_Text sfxVal,
            out Slider sensSlider, out TMP_Text sensVal,
            out Slider fovSlider, out TMP_Text fovVal,
            out Toggle invertToggle,
            out Toggle reduceToggle,
            out Toggle lowAtmosToggle,
            out Button textScaleBtn, out TMP_Text textScaleVal,
            out Button closeBtn)
        {
            var panel = CreateModalSheet("SettingsPanel", parent, "Pengaturan", font, 640f, 570f, out var body);

            CreateSliderRow(body, "Volume Utama", font, 0f, 1f, 0.85f, out masterSlider, out masterVal);
            CreateSliderRow(body, "Suasana", font, 0f, 1f, 0.80f, out ambSlider, out ambVal);
            CreateSliderRow(body, "Efek Suara", font, 0f, 1f, 0.90f, out sfxSlider, out sfxVal);
            CreateSliderRow(body, "Sensitivitas Mouse", font, 0.03f, 0.35f, 0.10f, out sensSlider, out sensVal);
            CreateSliderRow(body, "Sudut Pandang (FOV)", font, 60f, 90f, 70f, out fovSlider, out fovVal);

            invertToggle = CreateToggleRow(body, "Balik Sumbu Y", font);
            reduceToggle = CreateToggleRow(body, "Kurangi Gerak Kamera", font);
            lowAtmosToggle = CreateToggleRow(body, "Atmosfer Lebih Tenang", font);

            var row = CreateHorizontalRow("TextScaleRow", body, 36f);
            var lbl = CreateText("Label", row.transform, "Ukuran Teks", font, 17f, FontStyles.Normal, ColInk, TextAlignmentOptions.Left);
            SetFlexWidth(lbl.gameObject, 1f);
            textScaleBtn = CreateCompactPaperButton("TextScaleButton", row.transform, "100%", font, 120f, out textScaleVal);

            AddSpacer("SettingsFooterGap", body, 4f);
            closeBtn = CreatePaperActionButton("CloseSettingsButton", body, "Simpan & Kembali", font, ColBtnNormal);

            panel.SetActive(false);
            return panel;
        }

        static GameObject BuildCreditsPanel(Transform parent, TMP_FontAsset font, out Button closeBtn)
        {
            var panel = CreateModalSheet("CreditsPanel", parent, "Kredit", font, 600f, 430f, out var body);

            string creditsBody =
                "<b>Kelompok VI — Grafika Komputer</b>\n" +
                "• Steven Bong — Gameplay & Sistem\n" +
                "• Luhur Pangaribowo Handaruaji — Narasi & UI\n" +
                "• Naila Rifda Rahmah — 3D & Audio\n\n" +
                "<b>Rujukan</b>\n" +
                "UN SDG 3 (Kesehatan Mental) · UN SDG 8 (Pekerjaan Layak) · WHO Mental Health at Work\n\n" +
                "<i>SELA adalah karya fiktif dan bukan layanan konseling atau nasihat hukum.</i>";

            var txt = CreateText("CreditsBodyText", body, creditsBody, font, 16f, FontStyles.Normal, ColInk, TextAlignmentOptions.TopLeft);
            txt.textWrappingMode = TextWrappingModes.Normal;
            SetHeight(txt.gameObject, 235f);

            closeBtn = CreatePaperActionButton("CloseCreditsButton", body, "Kembali", font, ColPaperFold);

            panel.SetActive(false);
            return panel;
        }

        static GameObject CreatePaperCard(string name, Transform parent, Color fill, float height, bool withDogEar)
        {
            var go = CreatePanel(name, parent, fill, true);
            SetHeight(go, height);

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ColInk;
            outline.effectDistance = new Vector2(2f, -2f);

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = ColShadow;
            shadow.effectDistance = new Vector2(6f, -6f);

            if (withDogEar)
                AddDogEar(go.transform, 24f);

            return go;
        }

        static void AddDogEar(Transform parent, float size)
        {
            var ear = CreatePanel("DogEar", parent, ColPaperFold, false);
            var rt = ear.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(size, size);

            var foldLine = ear.AddComponent<Outline>();
            foldLine.effectColor = ColInk;
            foldLine.effectDistance = new Vector2(-1.5f, -1.5f);
        }

        static GameObject CreateModalSheet(string name, Transform parent, string heading, TMP_FontAsset font, float width, float height, out Transform bodyTransform)
        {
            var overlay = CreatePanel(name, parent, new Color(0.11f, 0.14f, 0.13f, 0.45f), true);
            StretchFull(overlay.GetComponent<RectTransform>());

            var card = CreatePanel("Sheet", overlay.transform, ColPaper, true);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.anchoredPosition = new Vector2(150f, 0f);
            cardRt.sizeDelta = new Vector2(width, height);

            var outline = card.AddComponent<Outline>();
            outline.effectColor = ColInk;
            outline.effectDistance = new Vector2(2f, -2f);

            var shadow = card.AddComponent<Shadow>();
            shadow.effectColor = ColShadow;
            shadow.effectDistance = new Vector2(6f, -6f);

            AddDogEar(card.transform, 22f);

            var bodyGo = new GameObject("Body", typeof(RectTransform));
            bodyGo.transform.SetParent(card.transform, false);
            var bodyRt = bodyGo.GetComponent<RectTransform>();
            bodyRt.anchorMin = Vector2.zero;
            bodyRt.anchorMax = Vector2.one;
            bodyRt.offsetMin = new Vector2(28f, 22f);
            bodyRt.offsetMax = new Vector2(-28f, -24f);

            var vlg = bodyGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 9f;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var title = CreateText("Heading", bodyGo.transform, heading, font, 26f, FontStyles.Bold, ColInk, TextAlignmentOptions.Left);
            SetHeight(title.gameObject, 34f);

            var div = CreatePanel("Divider", bodyGo.transform, new Color(0.176f, 0.227f, 0.204f, 0.25f), false);
            SetHeight(div, 2f);

            bodyTransform = bodyGo.transform;
            return overlay;
        }

        static Button CreatePaperMenuButton(
            string name, Transform parent, string label, string rightTag,
            TMP_FontAsset font, Color normalColor, out TMP_Text rightTagComp)
        {
            var go = CreatePanel(name, parent, Color.white, true);
            SetHeight(go, 48f);

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ColInk;
            outline.effectDistance = new Vector2(2f, -2f);

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = ColShadow;
            shadow.effectDistance = new Vector2(3f, -3f);

            var img = go.GetComponent<Image>();
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.colors = MakeColorBlock(normalColor, ColBtnHover, ColBtnPressed);

            var mainTxt = CreateText("Label", go.transform, label, font, 19f, FontStyles.Bold, ColInk, TextAlignmentOptions.Left);
            var mainRt = mainTxt.rectTransform;
            mainRt.anchorMin = Vector2.zero;
            mainRt.anchorMax = Vector2.one;
            mainRt.offsetMin = new Vector2(18f, 0f);
            mainRt.offsetMax = new Vector2(rightTag != null ? -145f : -18f, 0f);

            rightTagComp = null;
            if (rightTag != null)
            {
                rightTagComp = CreateText("SaveTag", go.transform, rightTag, font, 14f, FontStyles.Normal, ColMutedInk, TextAlignmentOptions.Right);
                var subRt = rightTagComp.rectTransform;
                subRt.anchorMin = Vector2.zero;
                subRt.anchorMax = Vector2.one;
                subRt.offsetMin = new Vector2(18f, 0f);
                subRt.offsetMax = new Vector2(-16f, 0f);
            }

            return btn;
        }

        static Button CreatePaperActionButton(string name, Transform parent, string label, TMP_FontAsset font, Color normalColor)
        {
            var go = CreatePanel(name, parent, Color.white, true);
            SetHeight(go, 42f);

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ColInk;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var img = go.GetComponent<Image>();
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.colors = MakeColorBlock(normalColor, ColBtnHover, ColBtnPressed);

            var txt = CreateText("Label", go.transform, label, font, 17f, FontStyles.Bold, ColInk, TextAlignmentOptions.Center);
            StretchFull(txt.rectTransform);
            return btn;
        }

        static Button CreateCompactPaperButton(string name, Transform parent, string label, TMP_FontAsset font, float width, out TMP_Text textComp)
        {
            var go = CreatePanel(name, parent, Color.white, true);
            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = width;
            le.minWidth = width;
            le.preferredHeight = 34f;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = ColInk;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var img = go.GetComponent<Image>();
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.colors = MakeColorBlock(ColBtnNormal, ColBtnHover, ColBtnPressed);

            textComp = CreateText("Label", go.transform, label, font, 16f, FontStyles.Bold, ColInk, TextAlignmentOptions.Center);
            StretchFull(textComp.rectTransform);
            return btn;
        }

        static void CreateSliderRow(
            Transform parent, string labelText, TMP_FontAsset font,
            float min, float max, float initial,
            out Slider slider, out TMP_Text valueText)
        {
            var row = CreateHorizontalRow(labelText.Replace(" ", "") + "Row", parent, 36f);

            var lbl = CreateText("Label", row.transform, labelText, font, 17f, FontStyles.Normal, ColInk, TextAlignmentOptions.Left);
            var lblLe = lbl.gameObject.AddComponent<LayoutElement>();
            lblLe.preferredWidth = 195f;

            var sliderGo = new GameObject("Slider", typeof(RectTransform));
            sliderGo.transform.SetParent(row.transform, false);
            var sliderLe = sliderGo.AddComponent<LayoutElement>();
            sliderLe.flexibleWidth = 1f;
            sliderLe.preferredHeight = 22f;

            var bg = CreatePanel("Background", sliderGo.transform, new Color(0.176f, 0.227f, 0.204f, 0.22f), false);
            var bgRt = bg.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0f, 0.36f);
            bgRt.anchorMax = new Vector2(1f, 0.64f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGo.transform, false);
            var faRt = fillArea.GetComponent<RectTransform>();
            faRt.anchorMin = new Vector2(0f, 0.36f);
            faRt.anchorMax = new Vector2(1f, 0.64f);
            faRt.offsetMin = Vector2.zero;
            faRt.offsetMax = Vector2.zero;

            var fill = CreatePanel("Fill", fillArea.transform, ColClay, false);
            StretchFull(fill.GetComponent<RectTransform>());

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGo.transform, false);
            StretchFull(handleArea.GetComponent<RectTransform>());

            var handle = CreatePanel("Handle", handleArea.transform, ColInk, true);
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(15f, 20f);

            slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handleRt;
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = initial;

            valueText = CreateText("Value", row.transform, "", font, 16f, FontStyles.Bold, ColInk, TextAlignmentOptions.Right);
            var valLe = valueText.gameObject.AddComponent<LayoutElement>();
            valLe.preferredWidth = 66f;
        }

        static Toggle CreateToggleRow(Transform parent, string labelText, TMP_FontAsset font)
        {
            var row = CreateHorizontalRow(labelText.Replace(" ", "") + "Row", parent, 34f);

            var lbl = CreateText("Label", row.transform, labelText, font, 17f, FontStyles.Normal, ColInk, TextAlignmentOptions.Left);
            SetFlexWidth(lbl.gameObject, 1f);

            var toggleGo = new GameObject("Toggle", typeof(RectTransform));
            toggleGo.transform.SetParent(row.transform, false);
            var tLe = toggleGo.AddComponent<LayoutElement>();
            tLe.preferredWidth = 26f;
            tLe.preferredHeight = 26f;

            var bg = CreatePanel("Background", toggleGo.transform, ColBtnNormal, true);
            StretchFull(bg.GetComponent<RectTransform>());
            var bgOut = bg.AddComponent<Outline>();
            bgOut.effectColor = ColInk;
            bgOut.effectDistance = new Vector2(1.5f, -1.5f);

            var check = CreatePanel("Checkmark", bg.transform, ColClay, false);
            var checkRt = check.GetComponent<RectTransform>();
            checkRt.anchorMin = Vector2.zero;
            checkRt.anchorMax = Vector2.one;
            checkRt.offsetMin = new Vector2(5f, 5f);
            checkRt.offsetMax = new Vector2(-5f, -5f);

            var toggle = toggleGo.AddComponent<Toggle>();
            toggle.targetGraphic = bg.GetComponent<Image>();
            toggle.graphic = check.GetComponent<Image>();
            toggle.isOn = false;
            return toggle;
        }

        static GameObject CreateHorizontalRow(string name, Transform parent, float height)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            SetHeight(go, height);

            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 16f;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            return go;
        }

        static GameObject CreatePanel(string name, Transform parent, Color color, bool raycastTarget)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = raycastTarget;
            return go;
        }

        static TMP_Text CreateText(string name, Transform parent, string text, TMP_FontAsset font, float size, FontStyles style, Color color, TextAlignmentOptions align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (font != null) tmp.font = font;
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            return tmp;
        }

        static void AddSpacer(string name, Transform parent, float height)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            SetHeight(go, height);
        }

        static void SetHeight(GameObject go, float height)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.minHeight = height;
        }

        static void SetFlexWidth(GameObject go, float flex)
        {
            var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            le.flexibleWidth = flex;
        }

        static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        static ColorBlock MakeColorBlock(Color normal, Color highlighted, Color pressed)
        {
            var cb = ColorBlock.defaultColorBlock;
            cb.normalColor = normal;
            cb.highlightedColor = highlighted;
            cb.selectedColor = highlighted;
            cb.pressedColor = pressed;
            cb.disabledColor = new Color(normal.r * 0.88f, normal.g * 0.88f, normal.b * 0.88f, 0.55f);
            cb.colorMultiplier = 1f;
            cb.fadeDuration = 0.1f;
            return cb;
        }

        static void RegisterBuildSettings()
        {
            var list = new List<EditorBuildSettingsScene>();
            AddSceneIfExists(list, MainMenuScenePath);
            AddSceneIfExists(list, OfficialGameplayScenePath);
            AddSceneIfExists(list, DefaultGameplayScenePath);
            AddSceneIfExists(list, ProgressScenePath);
            EditorBuildSettings.scenes = list.ToArray();
        }

        static void AddSceneIfExists(List<EditorBuildSettingsScene> list, string path)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
                list.Add(new EditorBuildSettingsScene(path, true));
        }

        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
