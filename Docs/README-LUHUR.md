# README untuk Luhur — bagian yang saya kerjakan di SELA

> Berkas ini buat saya baca sendiri saat rekaman video penjelasan ke dosen.
> Durasi maksimal 2 menit. Yang penting: jelas **apa** yang dibuat, **di mana** filenya,
> dan **bagaimana** cara saya tunjukkan di layar.

---

## 1. Identitas

| | |
|---|---|
| Nama | Luhur Pangaribowo Handaruaji |
| NIM | 2404130093 |
| Bagian tugas | **Narrative & UI** — naskah, data konten, semua UI (HUD, laptop, CV, Catatan, menu), tutorial, aksesibilitas |
| Rekan | Naila Rifda Rahmah (3D & Audio), Steven Bong (Gameplay & System) |

Aturan pembagian yang dipakai: **GDD** menang soal aturan permainan, **DESIGN** menang soal
tampilan, dan **naskah** menang soal isi cerita. Kalau dokumennya berbeda, yang dipakai adalah
ketiga aturan itu secara terpisah, bukan digabung seenak fouet.

---

## 2. Kalimat pembuka untuk video (baca ini, sekitar 15 detik)

> "Di bagian saya ada naskah cerita dan seluruh antarmuka. Semua teks cerita tidak ditulis
> langsung di dalam tombol, tapi dikumpulkan di satu tempat yang namanya `SelaContent`, lalu
> dipakai bersama oleh laptop, Catatan, panel CV, dan halaman penutup. Dari situ saya bangun
> HUD, menu, panel aktivitas, panel lamaran, panel interview, Meja Bukti, kalender, pengaturan,
> dan halaman bantuan. Saya juga yang bikin alur pengenalan enam langkah supaya pemain baru
> paham aturan mainnya tanpa perlu penjelasan panjang."

---

## 3. Cara menjalankan game

1. Buka proyek di **Unity 6.3 (6000.3.27f2)**. Editor-nya masih memakai lock file proyek.
2. Buka scene **`Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity`** — ini scene yang sudah
   terdaftar di *Build Settings* dan sudah berisi `SelaBootstrap` sebagai pembuat UI.
3. Tekan **Play**.
4. Kalau teks di layar tidak terlihat atau kotak-kotak kosong, itu masalah font TMP. Sudah
   ada penanganannya otomatis: `UiKit` memuat font dari `Resources/UI/Fonts`, kalau glifnya
   kosong font dibuat ulang dari file TTF saat runtime, dan kalau itu gagal dipakai font
   bawaan TextMeshPro. Detailnya di bagian 6.

### Kontrol yang dipakai saat demo

| Tombol | Fungsi |
|---|---|
| `W A S D` | Jalan |
| `Mouse` | Melihat |
| `E` | Interaksi dengan benda (menunggu trigger 3D dari Bong) |
| `Tab` | Buka Catatan |
| `M` | Buka **Menu Ruang** (jalan pintas sementara, lihat bagian 8) |
| `Esc` | Tutup panel teratas / Jeda |

---

## 4. Peta file: bagian saya

Semua file UI ada di `Assets/_SELA/Scripts/UI/`, semua teks cerita di
`Assets/_SELA/Scripts/Narrative/`.

### 4.1 Tulang punggung UI (bagian inti yang paling saya banggakan)

| File | Isi |
|---|---|
| `UI/UiKit.cs` | Pabrik komponen: teks, tombol, panel kertas, scroll, divider, kotak Tenaga, garisTekanan, kartu bukti, kartu terpilih, modal layer. Semua layar dibangun dari sini, jadi jarak dan warnanya seragam. |
| `UI/UiTheme.cs` | Palet warna (KertasBeras, HijauMeja, TanahTirai, BiruTinta, ArangHijau, KuningKusam, MerahPeringatan) + skala ukuran teks. |
| `UI/UiRouter.cs` | Tumpukan layar: satu layar aktif di atas, `Esc` menutup yang teratas, dan hanya layar **modal** yang memblokir dunia. |
| `UI/UiScreen.cs` | Kelas dasar layar: `IsModal`, `Build()`, `Refresh()`. |
| `UI/Screens/UiSheet.cs` | Lembar panel: judul, area isi yang bisa digulir, dan **tombol Kembali yang selalu terlihat** di luar area scroll. |
| `UI/SelaBootstrap.cs` | Otak UI: membuat canvas, mendaftarkan semua layar, menyambungkan HUD, kamera, dan seluruh tombol. |
| `UI/UiSettings.cs` | Menyimpan pengaturan yang dipilih pengguna (volume, sensitivitas, invert Y, FOV, ukuran teks, reduce motion, intensitas atmosfer). |
| `UI/UiFocusRing.cs` | Garis fokus tebal untuk navigasi keyboard. |

### 4.2 Layar yang saya buat

| File | Layar |
|---|---|
| `UI/Screens/MainMenuScreen.cs` | Menu utama (judul kiri, daftar menu di bawahnya) |
| `UI/Screens/SettingsScreens.cs` | Pengaturan, Credits dan Sumber, Refleksi dan Bantuan, Informasi konten |
| `UI/Screens/PauseScreen.cs` | Panel Jeda, status simpan, Ulangi dari checkpoint, dan Petunjuk/Selesaikan adegan saat retakan berlangsung |
| `UI/Screens/HudScreen.cs` | HUD: Hari/Sesi, kotak Tenaga, garisTekanan, prompt interaksi |
| `UI/Screens/NotesScreen.cs` | Catatan (Tab): bukti, jadwal, fakta ruang, riwayat, cara main |
| `UI/Screens/LaptopScreen.cs` | Laptop tiga tab: Lamaran, Pesan, Catatan |
| `UI/Screens/CvScreen.cs` | Draf CV dan Snapshot pengiriman lamaran |
| `UI/Screens/VerificationScreen.cs` | Verifikasi perusahaan (empat aspek syarat) |
| `UI/Screens/InterviewScreen.cs` | Panel interview dan hasil interview |
| `UI/Screens/ActivityPanel.cs` | Panel aktivitas satu sesi + panel Jeda (break) |
| `UI/Screens/EvidenceBoardScreen.cs` | **Meja Bukti**: pilih kartu, pasang ke penjepit |
| `UI/Screens/RoomScreens.cs` | Ponsel, Kalender, Kasur, Menu Ruang |
| `UI/Screens/DecisionScreen.cs` | Keputusan akhir (pilih tawaran atau pulang) + epilog |
| `UI/Screens/TutorialScreen.cs` | **Pengenalan 6 langkah** saat pertama kali main |
| `UI/Screens/ConfirmDialog.cs` | Dialog konfirmasi tunggal yang dipakai semua peringatan |
| `UI/Screens/RiftPanel.cs` | Panel Retakan (isi retakan sebagian Naila) |

### 4.3 Isi cerita dan data konten

| File | Isi |
|---|---|
| `Narrative/SelaContent.cs` | Judul, tagline, enum dasar |
| `Narrative/SelaContent.Ui.cs` | Semua label tombol dan judul layar |
| `Narrative/SelaContent.Offers.cs` | Tiga tawaran: dokumen lowongan, syarat, aspek verifikasi |
| `Narrative/SelaContent.Messages.cs` | Pesan hari 1-7 dari Ibu, Dita, dan perusahaan |
| `Narrative/SelaContent.Dita.cs` | Percakapan Dita |
| `Narrative/SelaContent.Evidence.cs` | Isi kartu E0/E1/E2, pertanyaan dan umpan balik aktivitas |
| `Narrative/SelaContent.Cv.cs` | Dua klaim CV, klaim berlebihan, dukungan bukti |
| `Narrative/SelaContent.Interviews.cs` | Pertanyaan, jawaban, dan alasan penolakan |
| `Narrative/SelaContent.Rifts.cs` | Teks tiga Retakan |
| `Narrative/SelaContent.Recovery.cs` | Teks pemulihan dan jeda |
| `Narrative/SelaContent.Endings.cs` | Empat epilog, halaman refleksi, credits |
| `Narrative/SelaContent.Tutorial.cs` | Enam langkah pengenalan |
| `Narrative/SelaState.cs` | Sumber data yang dibaca semua UI + penyimpanan JSON |

> Prinsipnya: **satu pemilik teks**. Tidak ada kalimat cerita yang ditulis dua kali di dua
> tempat, jadi versi yang tampil di laptop, Catatan, dan panel penutup selalu sama.

---

## 5. Daftar fitur yang bisa ditunjukkan di video (urutan aman)

Kalau hanya ada 2 menit, demonstrasi ini cukup dan semuanya sudah ada di kode.

| # | Yang ditunjukkan | Cara | Yang terlihat |
|---|---|---|---|
| 1 | Menu utama | Play, lalu `Mulai baru` → konfirmasi | Judul di kiri, daftar menu, tidak ada skor |
| 2 | Pengenalan 6 langkah | Otomatis muncul setelah Mulai baru | Tiap langkah memakai komponen yang dipakai (panel aktivitas, kartu, kalender, CV, snapshot) |
| 3 | Menu Ruang | Tekan `M` | Daftar benda, 9 aktivitas, dan Kemajuan cerita; aktivitas yang terkunci diberi alasan tertulis |
| 4 | Panel aktivitas | Pilih "Rapikan contoh antarmuka" → `Gunakan satu sesi` | Biaya sebelum tombol: `Tenaga: 3 → 2`, `Tekanan: 1 → 2` |
| 5 | Pilihan kartu (Meja Bukti) | Pilih kartu, kartu terpilih dapat garis tebal + label **Dipilih**, lalu `Pakai pilihan ini` | Pilihan salah → alasan, boleh diulang tanpa biaya |
| 6 | Laptop | `M` → Laptop | Tiga tab: Lamaran, Pesan, Catatan; status tiap perusahaan |
| 7 | Draf CV | Dari tab Lamaran → `Buka CV` | Dua klaim + bukti; klaim tanpa dukungan ditulis **Bukti belum mendukung kalimat ini** |
| 8 | Meja Bukti | Dari menu ruang atau tab Catatan → `Buka Meja Bukti` | Kartu dipilih → dipasang ke penjepit; hubungan klaim dan bukti tertulis |
| 9 | Catatan | `Tab` | Bukti, jadwal, fakta ruang, riwayat, cara main |
| 10 | Keputusan dan epilog | `M` → Kemajuan cerita → Keputusan akhir, lalu Penutupan cerita | Peringatan risiko, konfirmasi, epilog sesuai pilihan |
| 11 | Pengaturan | `Esc` → Pengaturan | Ukuran teks 150% → seluruh panel ikut membesar, tombol Kembali tetap terlihat |
| 12 | Credits dan Refleksi | Menu utama → Credits / Bantuan | Hanya aset yang benar-benar dipakai; tanpa nomor bantuan rekaan |

---

## 6. Detail teknis yang bisa saya jelaskan kalau ditanya

1. **Font TMP kosong (masalah nyata yang sudah dibenerin).**
   Aset `Noto Sans Regular SDF` dan `Noto Sans Bold SDF` di `Assets/_SELA/Resources/UI/Fonts/`
  impannya kosong, jadi kalau tidak ditangani semua teks di layar tidak terlihat.
   `UiKit.LoadFont` sekarang: cek tabel glif → kalau kosong, buat ulang `TMP_FontAsset` dari
   file `.ttf` (mode dynamic, SDFAA) saat runtime → kalau gagal, pakai `TMP_Settings.defaultFontAsset`.
   File TTF-nya ada di `Assets/_SELA/UI/Fonts/` (Noto Sans Regular/Bold) dan lisensinya OFL.

2. **Modal vs non-modal.**
   HUD duduk di tumpukan router tapi `IsModal = false`, jadi tidak pernah memblokir dunia.
   `UiRouter.AnyModalOpen` yang dipakai untuk mengunci input, bukan `AnyOpen`.
   Efeknya: kursor tetap terkunci, `Tab` tetap membuka Catatan, dan `Esc` tidak pernah
   menutup HUD secara tidak sengaja.

3. **Aturan sesi dipakai satu sumber.**
   Biaya aktivitas, urutan `validasi → konfirmasi → biaya satu kali → cek Jeda → cek Retakan →
   geser sesi → simpan checkpoint` semuanya di `Scripts/Gameplay/SelaSessionFlow.cs`.
   UI memanggil fungsi itu, tidak menyalin aturannya sendiri.

4. **Aksesibilitas.**
   Ukuran teks 150% menjadi pengali ukuran komponen, jadi bukan cuma judul yang membesar;
   tombol Kembali memang ditaruh di luar area scroll supaya selalu terlihat;
   semua panel punya fokus keyboard; tidak ada ikon gembok untuk hal yang sebenarnya bisa dicoba;
   tidak ada nomor bantuan rekaan, dan halaman refleksi menyebutkan orang tepercaya, konselor kampus, layanan kesehatan setempat, serta kanal resmi ketenagakerjaan.

5. **Penyimpanan.**
   `SelaSaveStore` menulis JSON dengan pola tulis ke berkas sementara lalu rename, supaya file
   simpanan tidak pernah setengah jadi. Array dari simpanan lama dinormalisasi supaya
   perubahan jumlah tawaran tidak bikin error.

---

## 7. Serah terima ke Steven Bong (Gameplay & System)

Bong tinggal membuat trigger interaksi 3D. Semua layar saya sudah punya pintu masuk publik di
`SelaBootstrap`, tinggal dipanggil dari `Interactable.onInteract`:

```csharp
SelaBootstrap.Instance.OpenLaptop();          // laptop meja
SelaBootstrap.Instance.OpenLaptopMessages();  // isi pesan dari ponsel
SelaBootstrap.Instance.OpenPhone();           // ponsel
SelaBootstrap.Instance.OpenCalendar();        // kalender dinding
SelaBootstrap.Instance.OpenBed();             // kasur
SelaBootstrap.Instance.OpenEvidenceBoard();   // papan bukti
SelaBootstrap.Instance.OpenNotes();           // catatan harian
SelaBootstrap.Instance.OpenActivity(SelaActivity.RapikanContohAntarmuka);
```

Tambahan yang perlu tahu:

- `Scripts/Gameplay/SelaSessionFlow.cs` saya buat karena aturannya dipakai UI.
  Kalau Bong mau aturan ini jadi miliknya, tinggal pindahkan file itu dan ubah namespace.
  Yang penting jangan ada dua tempat yang menghitung sesi.
- Di `Scripts/Player/FirstPersonController.cs` saya menambah **satu field**,
  `invertY` (default `false`), supaya setting invert Y bisa dipakai. Sisanya tidak saya ubah.
- Pengaturan sensitivitas dikalikan `BaseSensitivity (0,1) × UiSettings.Sensitivity`,
  karena angka di pengaturan adalah pengali 0,2-3,0, bukan nilai absolut.

---

## 8. Serah terima ke Naila (3D & Audio)

- Font dan warna UI memakai palet yang sama dengan aset: `UiTheme.cs`.
  Kalau palet PNG (`T_SELA_Palette.png`) berubah, warna UI bisa ikut disesuaikan di satu tempat.
- Objek yang saya perkirakan perlu trigger 3D:
  `SM_Laptop`, `SM_Ponsel`, `SM_Kalender`, `SM_Kasur`, `SM_Papan_Bukti`, `SM_Meja`, `SM_Kursi_Jendela`.
- Teks naratif 3D (label pintu 07, kalender, tiket, kartu pesan Ibu) belum saya buat sebagai
  mesh label. Kalau Naila butuh, teksnya sudah ada di `SelaContent` dan tinggal saya beri stringnya.

---

## 9. Yang belum selesai / perlu diketahui

1. **Menu Ruang (tombol `M`) adalah jalan pintas sementara.**
   Saya buat supaya semua panel bisa didemokan sekarang, sebelum trigger benda 3D ada.
   Setelah trigger 3D siap, tombol `M` di `SelaBootstrap.Update()` bisa dihapus.
2. **Meja Bukti sudah ada sebagai panel UI**, belum terikat ke objek `SM_Papan_Bukti`.
3. **Audio dan caption** belum saya kerjakan (itu bagian Naila); panel Jeda sudah punya
   tempat untuk subtitle, tinggal diisi data audio.
4. **Simpan/load final** masih menyimpan ke `Application.persistentDataPath` dengan JSON.
   Kalau formatnya sudah pasti, boleh diubah ke slot save per pemain.
5. **Kompilasi sudah bersih dan sudah diuji visual di Editor yang sedang membuka proyek.**
   Editor itu saya kendalikan lewat `unity-cli`, jadi tiap panel bisa dibuka, difoto, dan
   diperbaiki langsung di game yang sedang berjalan. Verifikasi kompilasi offline tetap dipakai
   sebagai gerbang pertama, karena Editor memegang lock proyek:

   ```
   python -I Docs/tools/check_compile.py
   ```

   Hasil terakhir: 47 script, 0 error, 0 warning. Screenshot tiap panel ada di `Screenshots/`.

---

## 10. Checklist sebelum rekaman

- [ ] Buka `KosV2_PlayerTest.unity`, tekan Play, pastikan teks terlihat (bukan kotak kosong).
- [ ] Coba `M` → semua panel terbuka tanpa error di Console.
- [ ] Coba `Tab` → Catatan, `Esc` → Jeda, `Esc` lagi → lanjut main.
- [ ] Coba `M` → Pengaturan → ukuran teks 150%, pastikan tombol Kembali masih terlihat.
- [ ] Uji `Mulai baru` → pengenalan 6 langkah muncul dan bisa dilewati.
- [ ] Pastikan Console bersih (tidak ada error kompilasi).

---

## 11. Kalau hanya boleh satu kalimat

> "Seluruh antarmuka SELA dan seluruh teks ceritanya saya kerjakan, dengan satu sumber teks
> di `SelaContent` supaya tidak ada versi yang berbeda antar panel, dan UI-nya dibangun
> dari nol saat runtime lewat `UiKit` dan `UiRouter` supaya tema, fokus keyboard, dan
> ukuran teks bisa diatur dari satu tempat."
