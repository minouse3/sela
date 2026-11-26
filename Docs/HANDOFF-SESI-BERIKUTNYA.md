# Serah-Terima Sesi Luhur — SELA

Status per **27 November 2026**. Branch lokal `UI,dll`, belum di-push (menunggu `Luuhhuurr`
diberi hak **Write** di repo `minouse3/sela`).

Rantai inti Minggu 2 **sudah lulus uji otomatis + screenshot** (lihat 4.4–4.5). Sisa pekerjaan
ada di bagian 5: jalur interview/ending B, layar yang belum difoto, dan dokumen/demo.

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
| `UiTest.Texts("<nama>")` | teks **penuh** tiap label di layar (untuk memeriksa naskah; `Dump` memotong 30 karakter) |
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

Pembungkus siap pakai: `python "C:\Users\M S I\AppData\Local\Temp\opencode\ui.py" click|open|openarg|closetop|scroll|state|screens|buttons|dump|texts|setday|setsession|savereload|give|shot`.

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
`lap-21-pesan.png` (tab Pesan final). Bukti batch L2: `chain-01..22` (lihat 4.5).

### 4.5 Batch perbaikan L2 (27 November, verifikasi ulang sukses)

**Alur sesi & Retakan**
- `SelaBootstrap.FinishActivityEvents()` menjadi satu urutan tetap untuk semua aktivitas
  (Aktivitas, interview, kirim lamaran, verifikasi, jeda): Jeda → Retakan terjadwal → sesi pindah.
  Sebelumnya aktivitas selain `ActivityPanel` tidak memicu Retakan.
- Retakan dicek **sebelum** sesi berpindah, jadi Retakan hari 2/4/6 muncul pada aktivitas siang
  dan tidak dobel saat adegan selesai.
- `RestartDay` menutup Retakan/Jeda yang masih terbuka; tanpa ini panel lama menutupi state
  checkpoint (teruji: pause di Retakan 1 → "Ulangi dari checkpoint" → HUD bersih, Hari 1 Pagi).

**Keputusan akhir**
- `DecisionScreen.RequestDecision()` memakai `SelaContent.Endings.DecisionConfirmLabel(choice)`;
  jalur C menambahkan `DecisionRiskWarning` + `DecisionConfirmBody`. Label tombol jadi
  "Konfirmasi pulang" / "Konfirmasi Studio Lajur" / "Konfirmasi Akses Prima".
- "Tarik pilihan" sekarang memakai `ConfirmDialog` (`DecisionWithdrawBody`).
- Pilihan A/B/C dan Pulang ditulis ke save saat dipilih (sebelumnya hilang bila reload).
- **Bug GDD 3.6 diperbaiki:** tawaran C tidak pernah bisa dipilih karena `obtained` hanya
  menerima status `Ditawari`, sedangkan C tanpa interview. Sekarang C terbuka sejak hari 4
  sampai malam hari 7; teruji sampai epilog C.

**Interview ditolak**
- Uji ulang jalur gagal: lamaran A terkirim dengan E1, jawaban tengah (1+1) → status
  `Ditolak`, `ApplicationInterviewScore = -2`, Tekanan naik dari 0 → 2 (biaya interview +1,
  penolakan +1). Alasan utama tampil ("penjelasan jawaban belum cukup"), catatan alasan lain
  tersimpan. Hasil disimpan satu kali di `ApplyResult`.

**Teks naskah**
- `chega` → "biar tidak kelelahan di hari pertama" (pesan Dita).
- `Qualifications` → "kualifikasi" (pesan Admin Akses Prima).
- `Kerjamanya` → "Jam kerjanya" (klarifikasi tawaran C).
- `Ticket` → "Tiket" (Catatan).
  Semua dicek lewat `UiTest.Texts` (teks penuh), bukan `Dump` yang memotong 30 karakter.

**Laptop**
- Kolom daftar kiri (Lamaran/Pesan) sekarang area gulir sendiri. Sebelumnya daftar pesan
  memanjang ke luar panel tanpa scroll, sehingga pesan lama (mis. Admin Akses Prima,
  hari 4) mustahil dibuka. Screenshot: `chain-20-pesan-list-scroll.png`.

**Pause**
- Panel Pause memakai satu area gulir; info checkpoint "Autosave terakhir" + "Awal hari"
  dan "Tersimpan" selalu bisa dilihat tanpa menimpa tombol (`chain-16-pause-info.png`).

---

## 5. Yang BELUM selesai (urutan prioritas)

### TODO-1 — Kasus tepi tersisa
Jalur A (sukses), B (sukses), C (berisiko), Pulang, dan penolakan interview sudah diuji
sampai hasil (`chain-14`, `chain-17`, `chain-22`, `chain-23`). Yang belum:
- Undangan interview terlewat (tidak dipakai di sesi siang) → Ditolak + Tekanan +1,
  dan tidak dobel saat hari berganti berkali-kali.
- Interview B gagal (bukan hanya B sukses) dan verifikasi B.
- Hari 7 tanpa tawaran sama sekali → hanya Pulang (pernah teruji tanpa tawaran,
  tetapi belum dengan lamaran yang ditolak).

Jalur uji cepat: `setday` + `setsession` + `openarg OpenActivity DudukTanpaLayar`,
lalu `open OpenLaptop`; `setday`/`setsession` hanya untuk uji, bukan fitur game.

### TODO-2 — Layar yang belum difoto
`EvidenceBoardScreen` (Meja Bukti), `NotesScreen`, `SettingsScreen`, `CreditsScreen`,
`HelpScreen`, `ContentInfoScreen`, `TutorialScreen`, `RoomScreens` (menu ruangan,
ponsel, kalender, kasur), `BreakScreen`, `OfferLetterScreen` (Baca tawaran),
`EndingReflection` (tombol "Refleksi dan Bantuan" di epilog).

Pola yang diwaspadai: `UiKit.Height(...)` tetap bisa memotong isi, dan daftar tanpa
`UiKit.Scroll` tidak bisa dijangkau (bug yang baru diperbaiki di laptop).

### TODO-3 — Aset & audio
Potret Nara/Bagas/Dita (3 ekspresi), transisi Retakan 2/3, tekstur/bayangan, dan audio
masih milik Naila; UI hanya menyediakan tempatnya. Retakan 2 dan 3 sendiri sudah lolos
uji state (`RiftSolved` true) dan punya Petunjuk + konfirmasi "Selesaikan adegan".

### TODO-4 — Dokumentasi & video
- `Docs/README-LUHUR.md` belum diperbarui dengan alur demo terbaru dan peta berkas.
- Skrip video 1 menit: menu → E1 → CV → lamaran A → snapshot → interview A → Retakan 1 →
  tidur → hari 2 interview/hasil → keputusan hari 7 → epilog. Rekam dari state baru
  supaya tidak ada sisa `setday`.
- Update `Docs/TARGET-PROGRESS-MINGGUAN.md` setiap Jumat (sudah dicentang untuk Luhur).

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
