# Naskah Video Progress — Luhur (maks 2 menit)

> File ini buat aku baca sendiri pas rekaman. Yang di **[BACA]** itu yang aku ucapkan.
> Di bawahnya ada **[TAMPILKAN]** (apa yang muncul di layar) dan **[SOROT]** (file + baris
> yang ditunjuk kursor).
>
> Catatan: **jangan sebut nomor baris.** Sebut nama file aja kalau perlu. Yang keluar dari
> mulut hanya isi blok [BACA].

---

## Sebelum rekaman

1. `python -I Docs/tools/check_compile.py` → harus `errors: 0 warnings: 0`.
2. Buka dulu di Unity: `SelaContent.cs`, `SelaContent.Offers.cs`, `UiKit.cs`, `UiTheme.cs`,
   `LaptopScreen.cs`, `InterviewScreen.cs`. Taruh di tab terpisah biar pas rekaman ga perlu
   nyari file.
3. Buka scene `Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity` → **Play**. Kalau muncul menu
   utama, klik **Mulai baru** → lewati tutorial.
4. Cek teks di layar kelihatan, bukan kotak kosong. Kalau kotak kosong, stop rekaman dulu.
5. `Screenshots/` udah siap jadi cadangan kalau game nyangkut.
6. Rekam 1080p. Satu segmen satu take; kalau ada salah, potong dari awal segmen itu.

---

## Segmen 1 — Pembuka (0:00 – 0:12)

**TAMPILKAN:** gameplay di kamar kos, kamera pelan berputar.

> **[BACA]**
> Selamat pagi. Saya Luhur Pangaribowo Handaruaji. Di tim saya menangani naskah cerita dan
> seluruh antarmuka SELA. Ceritanya soal Raka, lulusan yang punya tujuh hari memilih:
> kerja atau pulang.

---

## Segmen 2 — Satu sumber teks (0:12 – 0:26)

**TAMPILKAN:** editor, `Assets/_SELA/Scripts/Narrative/SelaContent.cs`.

**SOROT:** `SelaContent.cs` baris **56** — `public static partial class SelaContent`.
Diam di situ beberapa detik, lalu geser ke `SelaContent.Ui.cs` sebentar.

> **[BACA]**
> Semua teks cerita dan label tombol saya kumpulkan di satu tempat, namanya SelaContent.
> Jadi kalimat di laptop, di Catatan, sama di penutup itu sama persis, semuanya dari satu
> sumber.

---

## Segmen 3 — Pabrik UI (0:26 – 0:39)

**TAMPILKAN:** editor, `Assets/_SELA/Scripts/UI/UiKit.cs`, lalu `UiTheme.cs`.

**SOROT:**
- `UiKit.cs` baris **207** — `public static Button Button(...)`
- `UiKit.cs` baris **138** — `public static RectTransform Column(...)`
- `UiTheme.cs` baris **10–23** — daftar warnanya

> **[BACA]**
> UI-nya tidak saya buat dari prefab. Semuanya dibangun pas game jalan lewat satu pabrik
> komponen, namanya UiKit. Warna dan ukuran huruf semua di UiTheme. Jadi ganti tema cuma di
> satu tempat.

---

## Segmen 4 — Laptop dan alur lamaran (0:39 – 1:19)

Ini segmen inti.

**4a. TAMPILKAN (0:39 – 0:59):** `Screenshots/lap-l0-01-applications.png`, atau langsung buka di game.
**SOROT:** `SelaContent.Offers.cs` baris **150** (`OpeningDay`), **164** (`ClosingDay`),
terus **175** (`StatusLabel`).

> **[BACA]**
> Ini inti dari saya. Laptop Raka ada tiga tab: Lamaran, Pesan, Catatan. Status tiap
> perusahaan dihitung dari hari lowongannya terbit. Studio Lajur terbuka sampai malam hari
> tiga, Koperasi Pijar belum terbit. Jadi tombol Kirim lamaran cuma muncul kalau lowongannya
> sudah ada.

**4b. TAMPILKAN (0:59 – 1:09):** `chain-09-cv.png` (Draf CV) → `chain-10-snapshot.png` (Kirim lamaran).

> **[BACA]**
> Pemain menyusun CV dan memasang bukti. Sebelum dikirim ada panel ringkasan: kalimat apa
> yang terkirim, bukti apa yang ikut, biayanya berapa.

**4c. TAMPILKAN (1:09 – 1:19):** `chain-13-interview-result.png` (surat tawaran).

> **[BACA]**
> Setelah dikirim, interview jalan. Kalau syaratnya terpenuhi, Raka dapat surat tawaran
> berisi gaji dan jam kerjanya. Jadi alurnya utuh, dari CV sampai tawaran.

---

## Segmen 5 — Dua bug yang ketahuan (1:19 – 1:37)

**TAMPILKAN:** editor, `Assets/_SELA/Scripts/UI/Screens/LaptopScreen.cs`, lalu
`InterviewScreen.cs`.

**SOROT:**
- `LaptopScreen.cs` baris **161** — `AddAction`
- `InterviewScreen.cs` baris **240** — `ApplyResult()`, lalu **266** — `BuildResult()`

> **[BACA]**
> Pas saya uji alur ini, dua bug ketahuan. Tombol di panel laptop memanggil dirinya sendiri,
> jadi tidak pernah muncul. Panel interview menghitung hasil di dalam fungsi menggambar,
> layarnya berulang terus. Keduanya sudah dipisah: satu buat ngitung, satu buat nampilin.

---

## Segmen 6 — Aksesibilitas dan penutup (1:37 – 1:53)

**TAMPILKAN:** panel Draf CV atau Pengaturan di game, lalu balik ke editor `UiTheme.cs`.

**SOROT:** `UiTheme.cs` baris **55** — `public static float Scaled(float size)`.

> **[BACA]**
> Untuk aksesibilitas, ukuran teks bisa 150 persen dan seluruh panel ikut membesar, tombol
> Kembali tetap kelihatan di luar area gulir. Jadi naskah dan UI seluruhnya saya, dengan satu
> sumber teks, dan alur lamaran yang sudah jalan. Terima kasih.

---

## Kalau game lagi macet pas rekaman

Pakai screenshot dengan urutan yang sama: `lap-l0-01-applications.png` → `chain-09-cv.png` →
`chain-10-snapshot.png` → `chain-13-interview-result.png`. Blok [BACA] tetap dibaca sama,
cuma bagian TAMPILKAN diganti gambar diam. Kursor mouse digerakin pelan di region yang lagi
dibahas biar kelihatan hidup.

## Kalau ditanya dosen

| Pertanyaan | Jawaban |
|---|---|
| "Kenapa ga pakai prefab?" | Supaya semua panel konsisten dan bisa ikut ukuran teks pilihan pemain, tanpa bikin ribuan prefab. |
| "Kenapa teksnya di satu file?" | Biar tidak ada versi berbeda antara laptop, Catatan, dan penutup, dan nambah konten cuma di satu tempat. |
| "Bagaimana tahu UI-nya ga rusak?" | Tiap panel dijalankan di game yang lagi hidup, lalu dipotret dan dibaca satu-satu. Bug masuk daftar, bukan ke chat. |
| "Sudah playable?" | Alur lamaran sudah utuh: susun CV, kirim, interview, surat tawaran. Sisanya masih dirapikan. |
| "Kenapa ga pakai prefab yang ada di scene?" | Prefab di scene ga bisa menyesuaikan ukuran teks pilihan pemain. Dibangun runtime, satu aturan buat semua panel. |
| "Kenapa status lowongan ikut hari?" | Supaya pemain tahu lamaran yang masih bisa dikirim dan yang sudah lewat batas, bukan cuma tombol mati. |
