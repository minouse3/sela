# Visual Concept Gallery - SELA

Koleksi visualisasi konsep game **SELA** yang digenerate berdasarkan [GDD.md](../GDD.md) dan [DESIGN.MD](../DESIGN.MD).

![SELA Key Art](00-key-art.jpg)

---

### Daftar Aset Visual

| No | File | Deskripsi Visual | Konsep Terkait |
|---|---|---|---|
| 00 | `00-key-art.jpg` | **Key Art / Poster Utama SELA**: Memperlihatkan tipografi judul `S E L A` dan tagline di atas ruangan kamar kos yang terbelah dua—sisi kiri tenang dengan cahaya hangat, sisi kanan mulai terdistorsi oleh atmosfer dingin, retakan realitas, dan lembar penolakan melayang. | Cover & Identitas Game (GDD Header / Tagline) |
| 01 | `01-kamar-kos-firstperson.webp` | Tampilan ruang utama Kamar 07 dari sudut pandang orang pertama (First-Person). Kasur single di kiri, meja kerja hijau kelabu dengan laptop di tengah, gantungan kunci 07, jendela dengan tirai tanah liat di kanan. Gaya seni *Cat Poster Kos*. | Kamar 07 Normal (GDD 5.1 / DESIGN 6) |
| 02 | `02-pencahayaan-pagi-siang-malam.webp` | Tiga potongan horizontal memperlihatkan transisi pencahayaan: Pagi (cahaya matahari hangat dari jendela), Siang (pencahayaan netral merata), dan Malam (suasana redup sunyi). | Sistem Waktu & Siklus Hari (GDD 3.2 / DESIGN 5) |
| 03 | `03-hud-gameplay-free-roam.webp` | Antarmuka layar saat eksplorasi: Indikator waktu di kiri atas (`Hari 3 - Pagi`), indikator status di kiri bawah (`TENAGA` 4 bar kotak, `TEKANAN` 6 garis), dan prompt interaksi di tengah bawah (`[E] - Baca catatan`). | HUD & UI Hirarki (GDD 3.1 / DESIGN 9.1) |
| 04 | `04-antarmuka-laptop-verifikasi.webp` | Tampilan layar laptop dengan 3 tab (`Lamaran`, `Pesan`, `Catatan`). Memperlihatkan rincian lowongan Studio Lajur, kompensasi Rp4.200.000/bulan, jam kerja 40 jam/minggu, dan klausul perlindungan dokumen asli. | Layar Laptop & Verifikasi (GDD 3.6 / DESIGN 9.2) |
| 05 | `05-papan-bukti-closeup.webp` | Tampilan inspeksi dekat Meja Bukti di dinding utara. Tiga kartu bukti: `E0 Proyek Kuliah`, `E1 Antarmuka Formulir` (highlight kuning "Dipilih"), dan `E2 Catatan Pengujian`. | Meja Bukti & Portofolio (GDD 3.5 / DESIGN 9.2) |
| 06 | `06-karakter-potret-interview.webp` | Layar "SELA · Portofolio & Rekrutmen" dengan 3 kartu potret karakter 2D gaya anime: Nara (Perekrut Studio Lajur), Bagas (Koordinator Koperasi Pijar), dan Dita (Teman Kuliah Raka). | Desain Karakter (GDD 5.3 / DESIGN 8) |
| 07 | `07-retakan-ruang-1-meja-menjauh.webp` | Retakan Ruang Hari ke-2: Kamar memanjang secara surealis hingga 7 meter, kasur berderet, meja kerja berada di ujung jauh, dan lembaran kertas penolakan putih melayang diam di udara. | Retakan Ruang 1 (GDD 3.8 / DESIGN 7.1) |
| 08 | `08-retakan-ruang-2-pintu-bernama.webp` | Retakan Ruang Hari ke-4: Lorong kos memanjang dengan tiga pintu: pintu pertama bertuliskan `DITERIMA`, pintu kedua `TERLAMBAT`, dan pintu ketiga kamar asli `07`. | Retakan Ruang 2 (GDD 3.8 / DESIGN 7.2) |
| 09 | `09-retakan-ruang-3-kursi-kosong.webp` | Retakan Ruang Hari ke-6: Kamar kos dipenuhi kursi-kursi kayu kosong tambahan yang mewakili tuntutan sosial. Di kursi dekat jendela terdapat kartu pesan ibu bertuliskan "Tiketnya tetap boleh dipakai". | Retakan Ruang 3 (GDD 3.8 / DESIGN 7.3) |
| 10 | `10-map-kontrak-keputusan-final.webp` | Meja keputusan malam ke-7 dengan map hijau terbuka berisi draf kontrak kerja: Akses Prima dengan stempel merah tebal `[Berisiko]`, Studio Lajur bertanda `[JELAS]`, dan Koperasi Pijar bertanda `[JELAS]`. | Keputusan Kontrak (GDD 2.4 & 3.6 / DESIGN 9.2) |
| 11 | `11-tiga-ending-sela.webp` | Tiga panel komparasi ending: Panel kiri *Mulai dengan Batas* (surat kontrak di meja), panel tengah *Pulang, Bukan Selesai* (tas besar di kasur dan tiket di meja), panel kanan *Kontrak yang Mengunci* (malam kelabu, map berisiko). | Tiga Ending SELA (GDD 3.12 / DESIGN 9.13) |

## Mechanic Sheets

Lembar 12-25 merupakan ilustrasi mekanik dalam format JPG. Setiap panel memperlihatkan state alternatif atau urutan layar, bukan seluruh panel muncul bersamaan. Gambar menjadi sketsa suasana dan susunan UI; aturan, angka, dialog, dan pilihan akhir mengikuti GDD serta DESIGN.MD.

| No | Ilustrasi JPG | Cakupan mekanik | Spesifikasi |
| --- | --- | --- | --- |
| 12 | [HUD and Focus](12-hud-focus-states.jpg) | HUD bernilai, target sah/tidak ada target, panel baca gratis, dan perpindahan kursor | DESIGN 9.4; GDD 3.1 dan 3.9 |
| 13 | [Activity States](13-activity-states.jpg) | Konfirmasi satu sesi, Tenaga kurang, hasil aktivitas, dan alasan aksi tidak tersedia | DESIGN 9.5; GDD 3.3 |
| 14 | [Calendar and Sleep](14-calendar-sleep.jpg) | Tujuh hari, tiga sesi, tidur, bangun, hari terakhir, dan peringatan interview terlewat | DESIGN 9.6; GDD 3.2 |
| 15 | [Pressure and Jeda](15-pressure-jeda.jpg) | Band Tekanan, Jeda otomatis pertama, dua pilihan pemulihan, dan nilai 6 berulang | DESIGN 9.7; GDD 3.4 |
| 16 | [CV and Snapshot](16-cv-snapshot.jpg) | Klaim berlebihan, bukti belum tersedia, snapshot terkirim, dan koreksi klaim | DESIGN 9.8; GDD 3.5 |
| 17 | [Recruitment and Verification](17-recruitment-verification.jpg) | Empat aspek kontrak, penilaian pemain, temuan klarifikasi, jendela lamaran, dan benturan jadwal | DESIGN 9.9; GDD 3.6 |
| 18 | [Interview and Results](18-interview-results.jpg) | Satu perekrut, pertanyaan/respons, catatan terkirim, tawaran, penolakan, dan undangan terlewat | DESIGN 9.10; GDD 3.7 dan 3.11 |
| 19 | [Messages and Notes](19-messages-notes.jpg) | Inbox, pesan baru, baca gratis, aktivitas Hubungi Dita, dan overlay Catatan | DESIGN 9.11; GDD 3.3, 3.6, dan 3.9 |
| 20 | [Puzzle Interaction and Help](20-puzzle-help.jpg) | Ambil/bawa/kembalikan benda, tujuan salah, caption, petunjuk, dan Selesaikan adegan | DESIGN 9.12; GDD 3.8-3.10 |
| 21 | [Contract Decision States](21-contract-decision.jpg) | Pilihan sementara, tarik pilihan, risiko C, konfirmasi akhir, dan refleksi tanpa skor | DESIGN 9.13; GDD 3.11-3.12 |
| 22 | [Menu and Save States](22-menu-save-states.jpg) | Main Menu, Pause, checkpoint, konfirmasi keluar, dan kegagalan save/load | DESIGN 9.14; GDD 3.13 |
| 23 | [Settings and Reading Screens](23-settings-reading.jpg) | Informasi konten, pengaturan audio/kamera/teks, Credits, sumber, dan bantuan | DESIGN 9.15; GDD 3.13-3.14 |
| 24 | [Room Activity Presentation](24-room-activities.jpg) | Beristirahat, Hubungi Dita, Duduk tanpa layar, serta Rapikan meja dan perbedaan biayanya | DESIGN 9.5; GDD 3.3 |
| 25 | [Portfolio Work Presentation](25-portfolio-work.jpg) | Formulir sebelum/sesudah E1 serta tiga kasus uji dan batas pembuktian E2 | DESIGN 9.5; GDD 3.3 dan 3.5 |

Lembar 12-25 memakai ruang, warna, material kertas, serta gaya potret pada ilustrasi awal sebagai acuan. File PNG yang masih tersedia merupakan salinan pendamping; galeri menggunakan JPG. Ilustrasi tetap dapat digunakan meskipun tulisan atau jumlah pilihan belum mengikuti revisi terbaru. Isi implementasi dibuat sebagai teks dan kontrol terpisah yang dapat diperbesar.

## Implementation Notes

Teks serta aturan implementasi mengikuti [GDD](../GDD.md) dan [DESIGN bagian 9 dan 12](../DESIGN.MD), bukan tulisan dekoratif di dalam ilustrasi. Contohnya, Tekanan memiliki enam segmen, E2 memuat tiga kasus uji yang ditentukan, dan surat resmi hanya aktif setelah diperoleh. Catatan penyesuaian ilustrasi 00-11 tersedia di DESIGN 12.2.

Semua gambar adalah referensi desain, bukan screenshot produk jadi. Lembar mekanik tidak menambahkan fitur game, skor klinis, aplikasi laptop, atau slot save baru.
