# Serah-Terima Sesi Luhur — SELA

Status per **26 November 2026**. Branch lokal `UI,dll`, belum di-push (menunggu `Luuhhuurr`
diberi hak **Write** di repo `minouse3/sela`).

---

## 1. Peran dan batas kerja

| Anggota | Bagian |
|---|---|
| Naila Rifda Rahmah | 3D & Audio |
| Steven Bong | Gameplay & System |
| **Luhur Pangaribowo Handaruaji (2404130093)** | **Narrative & UI** — naskah, content data, seluruh UI (HUD, laptop, CV, Catatan, menu), tutorial, accessibility |

Urutan otoritas dokumen: `Docs/GDD.MD` → `Docs/DESIGN.MD` → naskah.
Kalau implementasi berbeda dari dokumen, **dokumen yang menang** dan UI harus diselaraskan.

Aturan main:
- **Unity Editor milik user — jangan dibantai.** Jangan recompile lewat asset pipeline.
  Selalu pakai `unity command script recompile`.
- Semua teks game tumbuh dari `SelaContent` (`Assets/_SELA/Scripts/Narrative/`, 14 file partial).
  Tidak boleh ada string UI yang ditulis langsung di kode layar.
- Komentar kode hanya kalau menjelaskan *why*-nya (keterbatasan tersembunyi, invarian).
- Tidak boleh ada karakter CJK/garbled di `Docs/**` dan `Assets/_SELA/Scripts/**`.

---

## 2. Arsitektur UI (ringkas tapi penting)

```
Canvas (Overlay, CanvasScaler 1280x720, match 0.5)
└── Screens                       <- semua UiScreen adalah anak dari sini
    ├── HUD                       (IsModal = false, dunia tetap bisa dipakai)
    └── <Panel modal>             (IsModal = true, memblokir dunia)
UiRouter  -> stack layar, fokus, cursor, blokir input
UiScreen  -> Root + Build() + Refresh()
UiSheet   -> panel kertas: Header / Content / Footer
UiKit     -> pabrik komponen
UiTheme   -> palet + tipografi
```

| Berkas | Isi |
|---|---|
| `Assets/_SELA/Scripts/UI/UiKit.cs` | pabrik komponen: `Panel`, `TextIn`, `Paragraph`, `Button`, `ListButton`, `Scroll`, `Column`, `VerticalDivider`, `Spacer`, `ActionRow`, `Divider`, `SectionTitle`, `Height` |
| `Assets/_SELA/Scripts/UI/UiTheme.cs` | palet + ukuran font (`BodyFontSize` 23, `SmallFontSize` 19, `DocumentFontSize` 19, `TitleFontSize` 36, `SectionFontSize` 26) |
| `Assets/_SELA/Scripts/UI/UiSheet.cs` | panel kertas + `EnsureFooter` |
| `Assets/_SELA/Scripts/UI/TmpAutoHeight.cs` | menjembatani TMP (tidak implementasi `ILayoutElement`) supaya tinggi paragraf ikut isi |
| `Assets/_SELA/Scripts/UI/ScrollBarDriver.cs` | scrollbar buatan sendiri: muncul hanya kalau isi > viewport, bisa di-drag |
| `Assets/_SELA/Scripts/UI/Screens/*.cs` | seluruh layar |
| `Assets/_SELA/Scripts/Narrative/SelaContent*.cs` | seluruh teks game (sumber kebenaran tunggal) |
| `Assets/_SELA/Scripts/Narrative/SelaState.cs` | state runtime + simpanan |
| `Assets/_SELA/Scripts/Gameplay/SelaSessionFlow.cs` | satu sumber aturan sesi/hari (bukan di folder UI) |

### Jebakan uGUI yang sudah pernah dikena

1. **`offsetMax` diukur dari anchor, bukan dari tepi.** `UiKit.Box(parent, name, (0,0),(1,1), (0,0), (0.3f,0))`
   akan melebar sampai 100% karena `anchorMax = (1,1)`. **Selalu pakai `UiKit.Column` untuk split layout.**
2. **`PaperPanel` tidak meregangkan dirinya.** `UiKit.PaperPanel` membuat RectTransform dengan
   anchor/size bawaan Unity (0.5/0.5, 100x100) — **tidak** seperti `UiKit.Box`.
   Tombol yang dibuat di dalamnya butuh layout group di parent-nya, kalau tidak ukurannya jadi kotak 100x100.
3. **TMP tidak implementasi `ILayoutElement`** → tanpa `TmpAutoHeight`, teks panjang menimpa isi di bawahnya.
   Semua teks panjang harus pakai `UiKit.Paragraph`.
4. **`Object.Destroy` tertunda sampai akhir frame** → `childCount` masih berisi objek lama.
   Jangan menghitung anak lewat `childCount` untuk memutuskan tampilan; pakai penghitung eksplisit.
5. **`AddComponent` memanggil `Awake` seketika** → field yang di-set *setelah* `AddComponent`
   masih `null` di dalam `Awake`. Ambil komponennya secara lazy di `LateUpdate`.
6. **Urut sibling menentukan layering.** `UiScreen.OnOpen()` sudah memanggil `Root.SetAsLastSibling()`
   supaya panel modal menutupi HUD.
7. **TMP alignment vertikal tengah** = `TextAlignmentOptions.MidlineLeft` / `MidlineRight`
   (bukan `MiddleLeft` — itu tidak ada).

---

## 3. Perkakas (wajib dipakai, jangan asal ubah)

### Kompilasi offline (cepat, tanpa menyentuh Editor)
```bash
cd /b/SELA/sela
python -I Docs/tools/check_compile.py
```
Output ideal: `sela-check.dll sources: 48 errors: 0 warnings: 0`. Kalau ada error, **perbaiki dulu** sebelum lanjut.
Harness ini memakai Roslyn bawaan Unity dengan `-warnaserror+`, jadi **peringatan apa pun = gagal**.

### Editor yang sedang berjalan
```bash
unity status --format json                       # pastikan state "ready"
unity command script recompile --project-path "B:\SELA\sela" --format json
sleep 12                                           # tunggu import selesai
unity command editor.playmode set_run_in_background --project-path "B:\SELA\sela" --format json
unity command editor set_auto_tick --project-path "B:\SELA\sela" --format json
```
> Setelah **setiap** recompile, ulangi `set_run_in_background` + `set_auto_tick`, kalau tidak
> game membeku di frame 1 dan screenshot yang diambil hanya bingkai beku.
> Bukti game benar-benar jalan: `unity status` → `playerLoopTicking: true` dan `frameCount` bertambah.

### Alat uji UI
Harus di dalam `AgentScripts/`, **di luar `Assets/`** supaya tidak di-import Unity.
`AgentScripts/UiTest.cs` — entry point:

| Perintah | Fungsi |
|---|---|
| `UiTest.Click("<label>")` | menekan tombol aktif yang labelnya mengandung teks |
| `UiTest.Label()` | daftar label tombol aktif |
| `UiTest.Screens()` | daftar layar yang terbuka |
| `UiTest.Dump("<nama>")` | posisi/warna/label tiap elemen di layar |
| `UiTest.State()` | ringkasan state: hari, sesi, tenaga, tekanan, bukti, status lamaran, CV, verifikasi |
| `UiTest.GiveEvidence(["E1","E2"])` | memberi bukti untuk menguji panel yang butuh bukti |
| `UiTest.Open("<metodeSelaBootstrap>")` | membuka layar lewat refleksi (**hanya metode tanpa parameter**) |

Contoh pemanggilan:
```bash
unity command script run --project-path "B:\SELA\sela" \
  -- --file AgentScripts/UiTest.cs --entry UiTest.Open --args '["OpenLaptop"]' --format json
unity command script run --project-path "B:\SELA\sela" \
  -- --file AgentScripts/UiTest.cs --entry UiTest.Click --args '["Periksa perusahaan"]' --format json
```

> **Catatan penting:** `script run --file ... --entry` mengompilasi berkas proyek **dengan akses
> assembly proyek** (beda dengan `script eval` yang mode Roslyn script tanpa referensi proyek).
> Untuk memanggil kode game, pakai `script run --entry`.
> `script eval` hanya untuk hal sederhana tanpa referensi proyek.

Pembungkus siap pakai: `sh /tmp/sela/ui.sh click|label|screens|dump|state|open|shot`
(jika file hilang, buat ulang; isinya memanggil perintah di atas).

Screenshot:
```bash
unity command capture game_view -- --source screen --output-path "B:\SELA\sela\Screenshots\nama.png"
```

### Mengukur ukuran sebenarnya
Ukur piksel PNG langsung dengan Python (zlib + struct, tanpa Pillow) untuk memastikan
letak garis/tombol benar. Pola yang dipakai sesi ini: menarik baris dengan warna tertentu
(mis. hijau `0x71,0x85,0x79`) pada kolom tertentu, dan memotong + memperbesar region
tertentu ke PNG baru supaya bisa dibaca.

---

## 4. Yang sudah selesai

### 4.1 Panel Laptop (sudah dicek lewat screenshot)

- **Tiga tab** Lamaran / Pesan / Catatan, tab aktif berwarna `BiruTinta`.
- **Kolom kiri = daftar, kolom kanan = isi**, dipisah `UiKit.VerticalDivider`.
- **Header tetap** di kolom kanan: judul perusahaan + lencana status (kanan).
  Judul dan status selalu terlihat walau dokumen sedang digulir.
- **Baris aksi terkunci** di bawah kolom kanan (tinggi 62) — tombol tidak pernah ikut tergulir.
  Tab Pesan tidak punya tombol → baris aksi disembunyikan dan area isi diperluas.
- **Dokumen 3 paragraf**, font `DocumentFontSize` (19), muat tanpa menggulir.
  Lowongan A/B/C + `Akses Prima` sebagai tawaran langsung.
- **Tab Catatan memakai satu kolom di tengah** (0.16–0.84) supaya baris teksnya tidak terlalu panjang.
- **Scrollbar** dibuat sendiri (`ScrollBarDriver`), muncul hanya kalau isi > viewport.
- **Footer "Tutup"** di bawah panel supaya bagian bawah tidak kosong.
- Sheet laptop `1080 x 620`.

### 4.2 Perbaikan alur lamaran (akar keluhan "tidak bisa buka laptop untuk lamar kerja")

Sebelumnya `ApplicationOpened` **hanya** di-set setelah lamaran terkirim, sehingga
tombol "Periksa perusahaan" tidak pernah muncul dan status selalu "Belum dibuka".

Sekarang status berbasis hari (GDD 3.4), API baru di `SelaContent.Offers`:

```csharp
public static int OpeningDay(OfferId offer)   // A=1, B=3, C=4
public static int ClosingDay(OfferId offer)    // A=3, B=5, C=7
public static string StatusLabel(ApplicationStatus status, OfferId offer, int day)
//   belum terbit      -> "Belum dibuka"
//   terbit, belum kirim, masih dalam jendela -> "Terbuka sampai malam hari N"
//   sudah lewat       -> "Jendela lamaran berakhir"
//   terkirim          -> "Interview terjadwal" / "Ditolak" / "Tawaran tersedia"
//   C                 -> "Tawaran langsung tersedia"
public static string NotPublishedReason(OfferId offer)   // menyebut tanggal, bukan gembok
public static string SendBlockedReason(ApplicationStatus status, OfferId offer, int day, bool scheduleTaken)
```

Field `SelaState.ApplicationOpened` **dihapus** (sudah tidak dipakai).
Pemanggil `StatusLabel` yang diupdate: `LaptopScreen`, `NotesScreen`.
`CvScreen.Send()` tidak lagi meng-set `ApplicationOpened`.

### 4.3 Perbaikan umum

- `UiKit.Scroll` menambah scrollbar + inset kiri 14 / kanan 10.
- `UiKit.Button` pakai `textWrappingMode = NoWrap` + `enableAutoSizing` (label tombol tidak pernah dua baris).
- `UiKit.ListButton` — item daftar dua baris (judul + subjudul), tinggi 56.
- `UiKit.Spacer`, `UiKit.Column`, `UiKit.VerticalDivider`, `UiKit.ActionRow` (baris tombol yang tidak meregang).
- `UiKit.Paragraph` = teks panjang dengan `TmpAutoHeight`.
- `SelaBootstrap.OnEnable()` membangun ulang UI setelah domain reload; `_gameplayActive` jadi `[SerializeField]`.
- `UiScreen.OnOpen()` → `Root.SetAsLastSibling()`.
- Semua teks bahasa Inggris di `TutorialScreen` diganti bahasa Indonesia.
- 63 lokasi `UiKit.TextIn` + `UiKit.Height` dikonversi jadi `UiKit.Paragraph`.

### 4.4 Screenshot bukti (sudah ada di `Screenshots/`)
`lap-15-laptop.png` (tab Lamaran final), `lap-20-catatan.png` (tab Catatan final),
`lap-21-pesan.png` (tab Pesan final).

---

## 5. Yang BELUM selesai (urutan prioritas)

### TODO-1 — Alur lengkap lamaran (PALING PENTING, keluhan utama user)
Belum pernah diuji jalan sampai kirim. Rangkaiannya:

1. `OpenLaptop` → tab Lamaran → klik "Buka CV" → `CvScreen`
2. `CvScreen` → pilih klaim + pasang bukti → lanjut ke snapshot
3. `OpenApplicationSnapshot` → "Kirim"
4. `OpenVerification(offer)` → periksa 4 aspek
5. `OpenInterview(offer)` → interview

**Syarat:** `UiTest.Open` hanya bisa memanggil metode tanpa parameter.
Tambahkan entry point baru di `AgentScripts/UiTest.cs`, misalnya
`OpenOffer(string method, string offer)` yang mem-parse `OfferId` dari argumen.

Periksa juga:
- apakah "Periksa perusahaan" muncul di hari 1 (lowongan A terbit hari 1)?
- apakah "Koperasi Pijar" baru tampil "Terbuka sampai malam hari 5" setelah hari 3?
- apakah Akses Prima (C) menampilkan "Tawaran langsung tersedia" dan **tidak** punya tombol Kirim?

### TODO-2 — Panel CV, verifikasi, snapshot kirim
Belum pernah difoto sama sekali. Cek:
- tinggi tombol, teks panjang tidak menimpa
- baris tombol tidak menempel tepi
- peringatan bukti yang belum ada terbaca
- header dan isi tidak tumpang tindih

### TODO-3 — Panel lain yang belum difoto
`ActivityPanel`, `EvidenceBoardScreen` (Meja Bukti), `NotesScreen`, `RiftPanel`,
`DecisionScreen`, `Epilog`, `SettingsScreens` (menu pengaturan, credits, bantuan),
`RoomScreens` (Menu Ruangan, ponsel, kalender, kasur), `MainMenuScreen`, `ContentInfo`,
`ConfirmDialog`, `TutorialScreen`.

Pola yang perlu diwaspadai: teks yang memakai `UiKit.Height(...)` dengan tinggi tetap bisa
meluap keluar kotaknya kalau isinya lebih panjang.

### TODO-4 — Dokumentasi
- `Docs/README-LUHUR.md` — belum diupdate dengan alur demo terbaru, daftar tombol, peta berkas.
  Ini dipakai untuk penjelasan video ≤2 menit ke dosen.
- Script penjelasan video 2 menit untuk dosen (masih versi lama).

---

## 6. Git

- Branch lokal: `UI,dll`. `origin` = `https://github.com/minouse3/sela.git`.
- **Belum bisa push:** akun `Luuhhuurr` hanya punya akses baca ke `minouse3/sela`
  (`gh api repos/minouse3/sela` → `"push": false`).
- Solusi: pemilik repo `minouse3` menambahkan `Luuhhuurr` sebagai collaborator dengan hak **Write**.
- Setelah itu: `git push -u origin "UI,dll"`.
- `AgentScripts/` **tidak boleh** ikut ter-push ke `Assets/` (alat uji saja, bukan aset game).
  Kalau tidak ingin ikut repo, tambahkan `AgentScripts/` ke `.gitignore` sebelum push.

---

## 7. Peringatan khusus untuk AI berikutnya

1. **Jangan berhenti bertanya.** Gunakan Auto Mode: lanjutkan sendiri, ambil keputusan untuk
   hal detail, laporkan hasilnya di akhir.
2. **Jangan kill Unity Editor** dan jangan buka tab baru yang bentrok dengan Editor yang jalan.
3. **Selalu kompilasi offline dulu** (`check_compile.py`), baru `unity command script recompile`.
4. **Verifikasi dengan screenshot**, bukan dengan membaca kode saja. Dua bug besar sesi ini
   (lebar kolom akibat `offsetMax`, tombol yang tidak pernah muncul) **tidak terlihat dari kode**.
5. **Jangan ubah GDD/DESIGN.** Kalau ada yang bertentangan, laporkan ke user.
6. Kalau perlu mengukur, pakai pengukuran piksel PNG (tidak ada Pillow di mesin ini).
7. Bahasa: user menulis bahasa Indonesia — **jawab dalam bahasa Indonesia**.
