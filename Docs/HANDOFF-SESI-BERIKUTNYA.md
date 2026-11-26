# Serah-terima untuk sesi berikutnya — UI SELA

> Dokumen ini untuk sesi Claude Code berikutnya. Baca dulu, lalu lanjut dari bagian "TODO utama".
> Semua yang sudah selesai sudah ditandai [OK] beserta buktinya (screenshot).

---

## 1. Konteks proyek

- Proyek: `B:\SELA\sela` (Unity 6.3 / 6000.3.27f2, URP, lilToon, TextMeshPro, Input System, uGUI).
- Tim: **Luhur** (Narrative & UI — file ini), Naila Rifda Rahmah (3D & Audio), Steven Bong (Gameplay & System).
- Scene demo: `Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity`.
- Aturan otoritas dokumen: **GDD** menang soal aturan main, **DESIGN.MD** menang soal tampilan,
  **naskah** menang soal isi cerita. Kalau dokumennya berbeda domain, pilih satu aturan saja.
- Lokasi teks: `Assets/_SELA/Scripts/Narrative/SelaContent.*.cs` (satu pemilik teks).
- Lokasi UI: `Assets/_SELA/Scripts/UI/` (UiKit, UiTheme, UiRouter, UiScreen, Screens/).

## 2. Cara terhubung ke Editor yang sedang terbuka

Editor Unity **sudah terbuka** di proyek ini dan bisa dikendalikan lewat CLI `unity`
(skill `unity-cli`). Urutan yang terbukti bekerja:

```bash
unity status --project-path "B:\SELA\sela" --until-ready --format json
unity command script recompile --project-path "B:\SELA\sela" --format json
unity command editor.playmode start --project-path "B:\SELA\sela" --format json
unity command editor.playmode set_run_in_background --project-path "B:\SELA\sela" --format json
unity command editor set_auto_tick --project-path "B:\SELA\sela" --format json
unity command capture game_view --project-path "B:\SELA\sela" -- --source screen --output-path "<path>.png"
unity command console get --project-path "B:\SELA\sela" -- --since <cursor> --level error
```

** wajib diulang setelah tiap recompile/domain reload:**
`editor.playmode set_run_in_background` + `editor set_auto_tick`. Tanpa itu game membeku di frame 1
karena Editor tidak difokus. Bukti bahwa game benar-benar berjalan: `unity status` menunjukkan
`playerLoopTicking: true` dan `frameCount` bertambah.

### Membuka panel dari dalam game (script eval)

`script eval` menerima `--file <path>.cs` (WAJIB ekstensi `.cs`) atau `--code <string>`.
Skrip dikompilasi sebagai Roslyn script, jadi `using UnityEngine.UI;` dan `using TMPro;`
harus ditulis eksplisit di dalam skrip.

Template klik tombol (letakkan di file `.cs`, lalu ganti TARGET):

```csharp
using UnityEngine.UI;
using TMPro;
public static class Runner {
    public static string Go() {
        string target = "Mulai baru";
        foreach (var b in UnityEngine.Object.FindObjectsByType<Button>(UnityEngine.FindObjectsSortMode.None)) {
            var t = b.GetComponentInChildren<TextMeshProUGUI>();
            if (t != null && t.text == target && b.gameObject.activeInHierarchy) { b.onClick.Invoke(); return "clicked " + target; }
        }
        return "not found: " + target;
    }
}
return Runner.Go();
```

Pemanggilan:
`unity command script eval --project-path "B:\SELA\sela" -- --file "C:/Users/M S I/AppData/Local/Temp/sela/nama.cs" --format json`

## 3. Verifikasi kompilasi offline

Editor memegang lock proyek, jadi kompilasi dicek offline lebih dulu:

```bash
python -I Docs/tools/check_compile.py
```

Harus keluar `errors: 0 warnings: 0` untuk dua pass (runtime + editor).
Harness ini memakai Roslyn bawaan Unity dengan `-warnaserror+`, jadi **peringatan apa pun = gagal**.

## 4. [OK] Yang sudah diperbaiki dan sudah dicek di layar

| Perbaikan | File | Bukti |
|---|---|---|
| Latar menu utama sekarang menutup layar penuh (sebelumnya kotak 115x115 di tengah) | `UI/UiKit.cs` (`Stretch`), `UI/Screens/MainMenuScreen.cs` | `Screenshots/sela-11-mainmenu.png` |
| Susunan menu utama tidak keluar layar (sebelumnya "Autosave terakhir" keluar bawah) | `UI/Screens/MainMenuScreen.cs` | `Screenshots/sela-11-mainmenu.png` |
| Info simpanan memakai isi berkas simpanan yang sebenarnya, bukan state yang sedang jalan | `UI/Screens/MainMenuScreen.cs`, `Narrative/SelaContent.Ui.cs` (`ResumeReason`) | `Screenshots/sela-11-mainmenu.png` |
| Label tombol tidak lagi membungkus ke dua baris; ukuran huruf mengecil sendiri | `UI/UiKit.cs` (`Button`: `textWrappingMode = NoWrap` + `enableAutoSizing`, `fontSizeMin 15`) | `Screenshots/sela-10-start.png` |
| Label Hari/Sesi HUD tidak terpotong, kotak prompt & caption kosong disembunyikan | `UI/Screens/HudScreen.cs` | `Screenshots/sela-13-tutorial.png` |
| Dialog konfirmasi sudah rapi | (tanpa perubahan, verifikasi visual) | `Screenshots/sela-12-confirm.png` |
| Kotak kalender tidak lagi sempit; isi jadi nomor hari + keterangan di bawah | `UI/Screens/RoomScreens.cs` (`CalendarScreen`) | belum difoto ulang |
| Menu ruang menutup diri sebelum membuka panel lain, jadi tidak bertumpuk | `UI/Screens/RoomScreens.cs` (`RoomMenuScreen.AddButton`) | belum difoto ulang |

**Catatan:** kotak biru besar di kiri bawah menu utama **bukan bug UI** — itu objek 3D di kamar
(poster/papan biru). Sudah dicek: tidak ada `Graphic` biru di hierarki UI.

## 5. TODO utama (urutan pakai)

### TODO-1 — Teks Inggris di pratinjau tutorial (SUDAH SELESAI)
`Assets/_SELA/Scripts/UI/Screens/TutorialScreen.cs` baris 89-90 sudah diganti ke bahasa Indonesia:
`"Tiket bus berangkat pagi hari kedelapan."` dan `"Lowongan untuk wisuda baru."`
Hasil recompile offline: 0 error, 0 warning.

### TODO-2 — Alur laptop → lamaran (keluhan utama user, BELUM diverifikasi)
Keluhan utama user: *"UI nya masih tidak karuan dan tidak bisa membuka laptop untuk proses lamar kerja"*.
Kode jalannya sudah ada, tapi belum pernah dibuktikan jalan di layar:

`M` → Menu Ruang → `Laptop - lamaran, pesan, catatan`
→ tab **Lamaran** → pilih perusahaan (Studio Lajur / Koperasi Pijar / Akses Prima)
→ **Buka CV** (`CvScreen`) → klaim + dukungan bukti
→ **Verifikasi perusahaan** (`VerificationScreen`, 4 aspek syarat)
→ **Kirim lamaran** (`ApplicationSnapshotScreen`) → `Send()`.

Langkah verifikasi yang harus dilakukan:
1. `script eval` klik `"Mulai baru"` → `"Konfirmasi"` → tutorial → `"Lewati"`.
2. `script eval` klik `"Laptop - lamaran, pesan, catatan"`.
3. Screenshot tiap tab (Lamaran / Pesan / Catatan), lalu CV, verifikasi, snapshot.
4. Perbaiki yang berantakan, lalu ulangi.

### TODO-3 — Screenshot tiap panel yang belum dicek ulang
`NotesScreen`, `LaptopScreen` (3 tab), `CvScreen`, `VerificationScreen`,
`ApplicationSnapshotScreen`, `ActivityPanel`, `EvidenceBoardScreen`, `DecisionScreen`,
`EpilogScreen`, `PauseScreen`, `SettingsScreen`, `CreditsScreen`, `HelpScreen`,
`RoomMenuScreen`, `PhoneScreen`, `CalendarScreen`, `BedScreen`.

Pola yang perlu diwaspadai: teks yang memakai `UiKit.Height(...)` dengan tinggi tetap bisa
meluap keluar kotaknya kalau isinya lebih panjang. Kalau itu terjadi, tambah tinggi atau
perbaiki teksnya.

### TODO-4 — Rapikan `Docs/README-LUHUR.md`
Bagian 9 (yang belum selesai) dan bagian 10 (checklist rekaman) masih menyebut
"belum saya uji visual di dalam Editor" — itu sudah tidak benar setelah sesi ini.
Perbarui juga daftar file (`SelaContent.Tutorial.cs`, `TutorialScreen.cs`,
`EvidenceBoardScreen.cs` sudah ada di sana) dan catatan alur demo.

## 6. Aturan yang harus dijaga

- Semua teks cerita/UI ditulis di `SelaContent.*.cs`, **tidak** disalin ke file UI.
- Tombol yang tidak bisa dipakai harus tetap punya alasan tertulis; jangan pakai ikon gembok.
- Warna dan ukuran teks hanya dari `UiTheme.cs`.
- Panel modal memblokir dunia; HUD `IsModal => false`.
- Jangan pernah `console clear`; baca dari cursor yang disimpan sebelum operasi pertama.
- Kalau mau kompilasi cek offline dulu sebelum menyentuh Editor.
- Simpan screenshot ke `Screenshots/`.
- Bahasa: teks game dan dokumen **bahasa Indonesia**; jangan pernah sisipkan huruf CJK.

## 7. Snapshot status terakhir

- Editor: PID 12572, port 7801, state ready.
- Play mode: aktif, `playerLoopTicking: true`.
- State game: sudah "Mulai baru" → tutorial langkah 1 terbuka (screenshot `sela-13-tutorial.png`).
- Console: bersih sejak cursor 72 (dicek ulang setelah TODO-1).
- Kompilasi offline terakhir: 46 file runtime + 47 file editor, 0 error, 0 warning.
