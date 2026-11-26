# Naskah Video Progress — Luhur (maks 2 menit)

> File ini untuk aku baca sendiri saat rekaman. Tiap segmen ada blok **[BACA]** yang berisi
> kalimat yang harus diucapkan. Di bawahnya ada **TAMPILKAN** (apa yang muncul di layar) dan
> **SOROT KODE** (file dan baris yang ditunjuk kursor mouse).
>
> Aturan paling penting: **jangan membaca nama file dan nomor baris**. Sebutkan nama file saja
> kalau memang perlu. Yang diucapkan hanya isi blok [BACA].

---

## Sebelum rekaman (10 menit)

1. `python -I Docs/tools/check_compile.py` → harus keluar `errors: 0 warnings: 0`.
2. Buka di Unity: `Assets/_SELA/Scripts/Narrative/SelaContent.cs`,
   `SelaContent.Offers.cs`, `Assets/_SELA/Scripts/UI/UiKit.cs`, `UiTheme.cs`,
   `UI/Screens/LaptopScreen.cs`, `UI/Screens/InterviewScreen.cs`.
   Taruh di tab terpisah supaya tidak perlu mencari saat rekaman.
3. Buka game: scene `Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity` → **Play**.
   Kalau muncul menu utama, klik **Mulai baru** → lewati tutorial.
4. Pastikan teks di layar terbaca, bukan kotak kosong. Kalau kotak kosong, hentikan rekaman
   dan perbaiki dulu.
5. Siapkan `Screenshots/` sebagai cadangan kalau game tiba-tiba macet.
6. Rekam 1080p. Satu segmen satu take; kalau salah, potong dari awal segmen itu.

---

## Segmen 1 — Pembuka (0:00 – 0:12)

**TAMPILKAN:** gameplay di kamar kos, kamera pelan-pelan berputar.

> **[BACA]**
> Selamat pagi. Saya Luhur Pangaribowo Handaruaji. Di tim saya menangani naskah cerita dan
> seluruh antarmuka game SELA. Ceritanya tentang Raka, lulusan yang punya tujuh hari untuk
> memilih menerima tawaran kerja atau pulang ke rumah.

---

## Segmen 2 — Satu sumber teks (0:12 – 0:30)

**TAMPILKAN:** editor, file `Assets/_SELA/Scripts/Narrative/SelaContent.cs`.

**SOROT KODE:** `SelaContent.cs` baris **56** — `public static partial class SelaContent`.
cursor berhenti di situ beberapa detik, lalu turun ke file `SelaContent.Ui.cs` sebentar.

> **[BACA]**
> Semua teks cerita dan label tombol saya kumpulkan di satu tempat, namanya `SelaContent`.
> Jadi kalimat yang muncul di laptop, di Catatan, dan di halaman penutup dijamin identik,
> karena semuanya dibaca dari sumber yang sama.

---

## Segmen 3 — Pabrik UI (0:30 – 0:48)

**TAMPILKAN:** editor, file `Assets/_SELA/Scripts/UI/UiKit.cs`, lalu `UiTheme.cs`.

**SOROT KODE:**
- `UiKit.cs` baris **207** — `public static Button Button(...)`
- `UiKit.cs` baris **138** — `public static RectTransform Column(...)`
- `UiTheme.cs` baris **10–23** — daftar warna `KertasBeras`, `HijauMeja`, `BiruTinta`, dan seterusnya

> **[BACA]**
> Antarmukanya tidak saya buat dari prefab. Semuanya dibangun saat game berjalan lewat satu
> pabrik komponen, namanya `UiKit`. Warna dan ukuran huruf semuanya ada di `UiTheme`.
> Karena begitu, mengubah tema cukup dilakukan di satu tempat saja.

---

## Segmen 4 — Laptop dan alur lamaran (0:48 – 1:20)

Ini segmen inti. Urutan layar dan kode:

**4a. TAMPILKAN:** `Screenshots/lap-l0-01-applications.png` (bisa juga dibuka langsung di game).
**SOROT KODE:** `SelaContent.Offers.cs` baris **150** (`OpeningDay`), **164** (`ClosingDay`),
lalu **175** (`StatusLabel`).

> **[BACA]**
> Ini bagian paling penting dari saya. Laptop Raka punya tiga tab: Lamaran, Pesan, dan Catatan.
> Di tab Lamaran, status tiap perusahaan dihitung dari hari lowongannya terbit. Studio Lajur
> terbuka sampai malam hari tiga, Koperasi Pijar belum terbit. Tombol Periksa perusahaan dan
> Kirim lamaran muncul kalau lowongannya benar-benar sudah ada.

**4b. TAMPILKAN:** `chain-09-cv.png` (Draf CV) → `chain-10-snapshot.png` (Kirim lamaran).

> **[BACA]**
> Dari sini pemain menyusun CV, memasang bukti yang relevan, lalu sebelum dikirim ada panel
> ringkasan: kalimat apa yang terkirim, bukti apa yang ikut, dan berapa biayanya.

**4c. TAMPILKAN:** `chain-13-interview-result.png` (surat tawaran).

> **[BACA]**
> Setelah dikirim, interview berjalan, dan kalau syaratnya terpenuhi, Raka menerima surat
> tawaran berisi gaji dan jam kerjanya. Jadi alurnya sudah utuh dari CV sampai surat tawaran.

---

## Segmen 5 — Dua bug yang ketahuan (1:20 – 1:42)

**TAMPILKAN:** editor, file `Assets/_SELA/Scripts/UI/Screens/LaptopScreen.cs`, lalu
`Assets/_SELA/Scripts/UI/Screens/InterviewScreen.cs`.

**SOROT KODE:**
- `LaptopScreen.cs` baris **161** — `AddAction`, tempat tombolnya harus memanggil `UiKit.Button`
- `InterviewScreen.cs` baris **240** — `ApplyResult()` (perhitungan), lalu **266** —
  `BuildResult()` (cuma menggambar)

> **[BACA]**
> Dua bug baru ketahuan waktu saya menguji alur ini. Yang pertama, tombol di panel laptop
> memanggil dirinya sendiri, jadi tombolnya tidak pernah muncul. Yang kedua, panel interview
> menghitung hasil di dalam fungsi menggambar, sehingga layarnya berulang tanpa henti.
> Keduanya sudah diperbaiki dengan memisahkan perhitungan dari penggambaran.

---

## Segmen 6 — Aksesibilitas dan penutup (1:42 – 2:00)

**TAMPILKAN:** panel Draf CV atau Pengaturan di game, lalu kembali ke layar editor
`UiTheme.cs`.

**SOROT KODE:** `UiTheme.cs` baris **55** — `public static float Scaled(float size)`.

> **[BACA]**
> Untuk aksesibilitas, ukuran teks bisa diatur 150 persen dan seluruh panel ikut membesar,
> dan tombol kembali selalu terlihat di luar area gulir. Jadi ringkasannya: seluruh naskah
> dan seluruh UI saya kerjakan, dengan satu sumber teks, satu pabrik komponen, dan alur
> lamaran yang sudah terbukti jalan. Terima kasih.

---

## Cadangan kalau game tidak bisa direkam live

Pakai screenshot dengan urutan yang sama: `lap-l0-01-applications.png` → `chain-09-cv.png` →
`chain-10-snapshot.png` → `chain-13-interview-result.png`. Blok [BACA] tetap dibaca sama persis,
hanya bagian "TAMPILKAN" yang diganti gambar diam. Gerakkan kursor mouse dengan pelan di
region yang sedang dijelaskan supaya tetap terlihat hidup.

## Kalau ditanya dosen

| Pertanyaan | Jawaban singkat |
|---|---|
| "Kenapa UI-nya tidak pakai prefab?" | Supaya semua panel konsisten dan bisa dibangun runtime sesuai ukuran teks pilihan pemain, tanpa harus membuat ribuan prefab. |
| "Kenapa teksnya di satu file?" | Supaya tidak ada versi berbeda antara laptop, Catatan, dan penutup, dan supaya menambah konten cuma di satu tempat. |
| "Bagaimana cara tahu UI-nya tidak rusak?" | Setiap panel dijalankan di game yang sedang hidup, lalu dipotret dan dibaca satu per satu. Bug baru masuk daftar, bukan ke grup chat. |
| "Sudah playable?" | Alur lamaran sudah utuh: susun CV, kirim, interview, surat tawaran. Sisanya masih dirapikan. |
| "Kenapa tidak pakai prefab yang di scene?" | Prefab di scene tidak bisa menyesuaikan ukuran teks pilihan pemain. Dengan membangun runtime, satu aturan untuk semua panel. |
