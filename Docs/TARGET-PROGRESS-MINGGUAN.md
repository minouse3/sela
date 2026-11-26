# Target Progress Mingguan — Proyek Akhir SELA

Rencana ini memakai backbone mingguan dari [GDD 3.15](GDD.md#315-production-plan-and-acceptance-criteria), lalu
dipecah per anggota mengikuti pembagian tugas di PPT *Kelompok SELA*. Semua angka waktu bisa dikecilkan kalau
jam kerja tiap anggota terbatas; yang tidak boleh dipotong adalah urutan: minggu 1 fondasi, minggu 2 satu
rantai utuh, minggu 3 kelengkapan, minggu 4 playable.

Status per 27 November 2026: ✅ selesai · 🟡 sebagian · ⬜ belum.

---

## 0. Aturan main (semua anggota)

| Aturan | Isi |
| --- | --- |
| Definisi "bisa dimainkan" | Orang lain bisa masuk scene, menyelesaikan aksi, dan melihat hasilnya tanpa penjelasan tambahan |
| Demo mingguan | Jumat, 10 menit per anggota, rekam 1 menit per anggota untuk video penjelasan akhir |
| Merge |_SELA_ branch + commit tiap Kamis malam, wajib build lolos kompilasi |
| Jangan tambah sistem | Cakupan tetap: 1 kamar, 1 lorong, 3 Retakan, 2 persiapan bukti, 2 interview, 3 tawaran, 3 ending (GDD 3.15) |
| Sumber kebenaran | GDD + DESIGN.md. Kalau beda, GDD menang; DESIGN governs tampilan |
| Konten | Semua teks cerita dan UI dalam bahasa Indonesia, satu versi, disimpan di `Scripts/Narrative/SelaContent*.cs` |

---

## Minggu 1 — Fondasi yang bisa dimainkan

**Target akhir minggu:** dari Main Menu, pemain masuk kamar, melihat HUD hari/sesi/tenaga/tekanan, bisa
berjalan, menekan E pada pintu, dan tidur sampai ganti hari. Naskah disusun sampai blok 3 hari pertama.

| Anggota | Target | Done when |
| --- | --- | --- |
| Naila | Aset FBX masuk scene + material palet (`SELA > Setup Palette Material`), 1 set animasi karakter (duduk/duduk-berdiri) untuk prototipe, daftar kebutuhan aset | Aset bedroom tampil, material satu warna bisa diganti lewat atlas, animasi bisa dipreview |
| Bong | Gerak first-person, raycast + tombol E, siklus Pagi/Siang/Malam, 1 aktivitas dengan biaya tenaga, checkpoint dasar | Pemain bisa jalan, buka pintu, ganti sesi, tidur → hari berikutnya |
| Luhur | Kanvas UI runtime (`SelaBootstrap`, `UiKit`, `UiTheme`), HUD hari/sesi/tenaga/tekanan + prompt interaksi, Main Menu, layar Informasi Konten, kerangka 17 layar lain (bisa dibuka, isinya menyusul) | Tombol "Mulai" masuk gameplay, HUD tampil, tiap layar terbuka tanpa error |

**Bukti playable:** 1 video 30 detik: menu → masuk kamar → HUD bergerak → tidur → hari 2.

---

## Minggu 2 — Satu rangkaian utuh (dulu "prepare E1 → lamaran → interview → Retakan 1 → save/load")

**Target akhir minggu:** satu jalur lengkap bisa dimainkan orang luar tim: siapkan bukti E1, kirim lamaran A,
snapshot, interview A, Retakan 1, lalu keluar dan lanjutkan dari checkpoint.

| Anggota | Target | Done when |
| --- | --- | --- |
| Naila | Penangkas/retakan pertama (kamar memanjang) + aset kunci 07 dan pintu, pose pertama untuk potret 3 tokoh | Retakan 1 bisa diselesaikan, transisi tidak gagap |
| Bong | Logika biaya aktivitas (Tenaga/Tekanan), Jeda otomatis saat Tekanan 6, trigger Retakan 1, save/load JSON | Keluar di tengah interview → ulang dari checkpoint yang disebut, tanpa hasil dobel |
| Luhur | Layar Laptop (tab Lamaran/Pesan/Catatan), susun CV (E0/E1/E2), panel verifikasi 4 aspek, layar interview, panel Catatan, Pause + Lanjutkan/Simpan | Pemain bisa menyusun CV, mengirim lamaran, underwent interview, dan melanjutkan game |

**Bukti playable:** 1 orang luar tim:main 10 menit, reporter bug di grup.

**Status Luhur (27 Nov): ✅** Rantai E1 → CV → lamaran A → snapshot → interview A (skor 4) →
Retakan 1 → tidur → interview/hasil → keputusan hari 7 → epilog sudah lulus uji otomatis
dan screenshot `Screenshots/chain-01..17`. Checkpoint awal hari (`sela-day-start.json`)
teruji lewat "Ulangi dari checkpoint".

---

## Minggu 3 — Kelengkapan konten dan seluruh hari

**Target akhir minggu:** tujuh hari bisa diselesaikan dari menu sampai keputusan, dengan tiga ending.

| Anggota | Target | Done when |
| --- | --- | --- |
| Naila | Retakan 2 (lorong tiga pintu) dan Retakan 3 (kursi kosong), potret Nara/Bagas/Dita, poles tekstur & bayangan | Dua Retakan selesai tanpa audio, teks petunjuk terbaca |
| Bong | 9 aktivitas harian (kerja, istirahat, hubungi Dita, merapikan, tidur), interview B, verifikasi 3 perusahaan, aturan ending | Semua hari 1–7 punya urutan aksi yang jelas, Tekanan 6/Tenaga 0 tidak membuat buntu |
| Luhur | Naskah lengkap sesuai inventaris GDD (pesan harian, 7 percakapan Dita, 3 lowongan + verifikasi, 2 interview, 3 Retakan, 4 epilog), layar: aktivitas, Jeda, keputusan kontrak, epilog, Credits/Sumber/Bantuan | Ending mengikuti pilihan kontrak, bukan stat; epilog purchases Burden berubah sesuai catatan pemain |

**Bukti playable:** 2 orang luar tim menyelesaikan game penuh dengan jalur berbeda (kerja / pulang / berisiko).

**Status Luhur (27 Nov): 🟡** Retakan 1–3, keputusan kontrak (A/B/Pulang/C), konfirmasi akhir,
epilog A/B/C, penolakan interview, dan tiga bug teks sudah lulus uji + screenshot
(`chain-18..23`). Masih kurang: interview terlewat, interview B gagal, dan foto layar
Settings/Credits/Help/Meja Bukti.

---

## Minggu 4 — Uji, perbaikan, build

**Target akhir minggu:** build Windows yang bisa dipertahankan dan diuji.

| Anggota | Target | Done when |
| --- | --- | --- |
| Naila | Audit lisensi aset (pastikan semua open source), sprite/portrait final, cek performa model | Tidak ada aset berlisensi tidak jelas, frame time tercatat |
| Bong | Bugar: state buntu, salah load, Tekanan/Tenaga, urutan Retakan; build + logging minimal | Tidak ada bug kritis dari 3 jalur uji utama |
| Luhur | Uji keterbacaan (semua menu & dialog bisa dipilih keyboard, fokus terlihat di 150%), panel settings (audio/kamera/teks/atmosfer), layar credits/bantuan, cek semua teks against GDD | Semua layar punya fokus keyboard yang terlihat; teks tidak terpotong di 1280×720 |

**Bukti playable:** build Windows + 1 lembar hasil playtest (durasi per hari, aktivitas yang dipilih, pemahaman snapshot).

---

## Minggu 5–6 (kalau waktu ada) — Polish

Tidak menambah sistem atau cabang cerita. Hanya: aset final (tekstur, audio, potret), perbaikan hasil playtest,
penyempurnaan ritme, dan Universities presentasi (slide + video 2 menit per anggota).

---

## Ritme mingguan (supaya tidak menumpuk di akhir)

| Hari | Kegiatan |
| --- | --- |
| Senin | Deal target minggu, breakdown tugas, bagi acceptance criteria |
| Selasa–Rabu | Eksekusi + daily check 15 menit (apa yang jalan, apa yang tersendat) |
| Kamis | Merge + build check + video 1 menit per anggota (potongan untuk video akhir) |
| Jumat | Demo 10 menit + review + geser target minggu depan |

---

## Checklist tiap akhir minggu

- [ ] Build jalan dari menu sampaiending, tanpa error Console
- [ ] Target minggu ini tercentang di tabel di atas
- [ ] Semua teks baru masuk ke satu file konten, bukan disalin ke banyak tempat
- [ ] Bug baru masuk daftar terpusat (bukan chat)
- [ ] Video 1 menit untuk anggota ini siap
