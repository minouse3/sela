# Prompt siap pakai untuk AI berikutnya

Salin isi bagian "Isi prompt" di bawah ke AI baru (DeepSeek/GPT/Claude), lalu kirim.
Prompt ini mandiri: tidak perlu membaca ulang riwayat percakapan sebelumnya.

---

## Isi prompt

```text
Kamu adalah anggota tim yang mengerjakan project game Unity "SELA" untuk tugas mata kuliah
Grafika Komputer. Kamu menggantikan sesi AI sebelumnya. Kamu WAJIB membaca file
`Docs/HANDOFF-SESI-BERIKUTNYA.md` di project SEBELUM melakukan apa pun — file itu berisi
konteks lengkap, daftar file penting, jebakan uGUI yang sudah pernah dikena, cara memverifikasi
di Unity Editor, dan daftar pekerjaan yang belum selesai.

Lokasi project: `B:\SELA\sela` (Windows, PowerShell, path bash `/b/SELA/sela`).

=== IDENTITAS DAN SCOPE ===

Aku: Luhur Pangaribowo Handaruaji (NIM 2404130093).
Peran aku di tim: **Narrative & UI**.
- Naila Rifda Rahmah: 3D & Audio
- Steven Bong: Gameplay & System
- Aku: naskah (script), content data, SELURUH UI (HUD, laptop, CV, Catatan, menu),
  tutorial, accessibility.

Semua teks game dan UI hanya boleh ditulis di `Assets/_SELA/Scripts/Narrative/SelaContent*.cs`
(14 file partial static class `SelaContent`). Jangan pernah menyalin string UI ke file layar.

Project ini akan ikut lomba game tingkat universitas, jadi hasil akhir harus rapi dan
sempurna, bukan sekadar berfungsi.

=== ATURAN KERJA YANG WAJIB DIPATUHI ===

1. Mode kerja: **Auto Mode / jalan sendiri**. Jangan berhenti bertanya hal detail.
   Ambil keputusan sendiri, kerjakan sampai tuntas, baru laporkan hasil di akhir.
2. **Jangan kill / restart Unity Editor milik aku.** Editor sering sedang terbuka dan memegang
   lock proyek.
3. **Selalu kompilasi offline dulu**, baru sentuh Editor:
   `python -I Docs/tools/check_compile.py`  → harus `errors: 0 warnings: 0`
   (peringatan apa pun = gagal, harness pakai `-warnaserror+`).
4. **Verifikasi selalu dengan screenshot**, bukan dengan membaca kode saja. Dua bug besar
   sesi lalu (lebar kolom akibat `offsetMax`, tombol yang tidak pernah muncul) sama sekali
   tidak kelihatan dari kode.
5. Urutan otoritas dokumen: **GDD menang soal aturan main, DESIGN.MD menang soal tampilan,
   naskah menang soal isi cerita.** Kalau ada yang bertentangan, dokumen yang menang dan
   implementasi harus diselaraskan. Jangan ubah GDD/DESIGN.
6. Bahasa: **jawab aku dalam bahasa Indonesia**. Teks game juga bahasa Indonesia.
   Jangan pernah sisipkan huruf CJK.
7. Komentar kode hanya kalau menjelaskan *why*-nya. Tanpa komentar fluff.
8. Simpan screenshot ke `Screenshots/`.
9. Jangan pernah `console clear` — baca console dari cursor yang disimpan sebelum operasi pertama.
10. Git: branch lokal `UI,dll` sudah ada dan sudah commit (2a39ed1). **Jangan push** —
    akun `Luuhhuurr` hanya punya akses baca ke `minouse3/sela`. Commit lokal saja kalau
    perlu. Jangan pernah `git add -A` tanpa cek `git status` dulu, dan jangan commit
    file rahasia.

=== CARA VERIFIKASI (WAJIB BACA) ===

Editor Unity **sudah terbuka** di project ini dan bisa dikendalikan lewat CLI `unity`
(skill `unity-cli`, package `com.unity.pipeline`):

```
unity status --project-path "B:\SELA\sela" --until-ready --format json
unity command script recompile --project-path "B:\SELA\sela" --format json
unity command editor.playmode set_run_in_background --project-path "B:\SELA\sela" --format json
unity command editor set_auto_tick --project-path "B:\SELA\sela" --format json
unity command capture game_view -- --source screen --output-path "B:\SELA\sela\Screenshots\nama.png"
unity command console get --project-path "B:\SELA\sela" -- --since <cursor> --level error
```

WAJIB diulang `set_run_in_background` + `set_auto_tick` **setiap** recompile, kalau tidak
game membeku di frame 1 dan screenshot hanya bingkai beku.

Alat uji UI ada di `AgentScripts/UiTest.cs` (di luar `Assets/` supaya tidak di-import Unity):

```
unity command script run --project-path "B:\SELA\sela" \
  -- --file AgentScripts/UiTest.cs --entry UiTest.Open  --args '["OpenLaptop"]' --format json
unity command script run --project-path "B:\SELA\sela" \
  -- --file AgentScripts/UiTest.cs --entry UiTest.Click --args '["Periksa perusahaan"]' --format json
```

Entry point yang tersedia: `Click`, `Label`, `Screens`, `Dump`, `State`, `GiveEvidence`, `Open`.
**`UiTest.Open` hanya bisa memanggil metode `SelaBootstrap` yang TANPA parameter.**
Kalau butuh memanggil yang punya parameter (mis. `OpenVerification(OfferId)`), tambahkan
entry point baru, misalnya `OpenOffer(string method, string offer)`.

Pembungkus siap pakai: `sh /tmp/sela/ui.sh click|label|screens|dump|state|open|shot`
(buat ulang kalau hilang).

Kalau pengukuran piksel dibutuhkan, pakai decoder PNG buatan sendiri (zlib + struct) —
Pillow tidak ada di mesin ini.

=== PEKERJAAN YANG BELUM SELESAI (urutan prioritas) ===

**TODO-1 (PALING PENTING — keluhan utama aku): alur lengkap lamaran belum pernah
dibuktikan jalan di layar.**

Keluhan asliku: *"UI nya masih tidak karuan dan tidak bisa buka laptop untuk proses lamar
kerja"*. Akar masalahnya sudah ditemukan dan diperbaiki (status lowongan sekarang berbasis
hari: Studio Lajur hari 1–3, Koperasi Pijar hari 3–5, Akses Prima hari 4–7; field
`SelaState.ApplicationOpened` yang dulu hanya di-set setelah lamaran terkirim sudah dihapus).
Tapi **jalannya belum pernah diuji dari awal sampai tombol "Kirim" ditekan.**

Rangkaian yang harus diuji, dengan screenshot tiap langkah:
1. `OpenLaptop` → tab Lamaran → cek tombol "Buka CV" / "Periksa perusahaan" / "Kirim lamaran"
2. Buka CV (`CvScreen`) → pilih klaim → pasang bukti E0/E1/E2 → cek teks peringatan
3. `OpenApplicationSnapshot(offer)` → cek ringkasan klaim, bukti, jadwal, biaya → klik "Kirim"
4. `OpenVerification(offer)` → periksa 4 aspek
5. `OpenInterview(offer)` → interview

Yang harus dicek khusus:
- "Periksa perusahaan" **muncul di hari 1** (lowongan A terbit hari 1)
- "Koperasi Pijar" baru menampilkan "Terbuka sampai malam hari 5" setelah hari 3
- Akses Prima (C) menampilkan "Tawaran langsung tersedia" dan **tidak** punya tombol Kirim
- Setelah lamaran terkirim, status di daftar berubah jadi "Interview terjadwal" / "Ditolak"
- Warning bukti yang belum lengkap tampil dan bisa dibaca

Untuk langkah 2–5 kamu perlu `OfferId` sebagai argumen → **tambahkan entry point
`OpenOffer` di `AgentScripts/UiTest.cs`**.

**TODO-2: panel CV, verifikasi, dan snapshot kirim belum pernah difoto sama sekali.**
Cek: tinggi tombol cukup (teks panjang tidak menimpa), baris tombol tidak menempel tepi,
peringatan bukti terbaca, header dan isi tidak tumpang tindih.

**TODO-3: panel lain yang belum difoto** — `ActivityPanel`, `EvidenceBoardScreen` (Meja Bukti),
`NotesScreen`, `RiftPanel`, `DecisionScreen`, `Epilog`, `SettingsScreens` (menu pengaturan,
credits, bantuan), `RoomScreens` (Menu Ruangan, ponsel, kalender, kasur), `MainMenuScreen`,
`ContentInfo`, `ConfirmDialog`, `TutorialScreen`.
Pola yang perlu diwaspadai: teks dengan `UiKit.Height(...)` tinggi tetap bisa meluap kalau
isinya lebih panjang.

**TODO-4: dokumentasi.**
- `Docs/README-LUHUR.md` masih versi lama (bagian "belum saya uji visual di dalam Editor" sudah
  tidak benar). Perbarui dengan: alur demo terbaru, daftar tombol, peta berkas, daftar screenshot.
  File ini dipakai untuk penjelasan video ≤2 menit ke dosen.
- Script penjelasan video 2 menit untuk dosen.
- Update `Docs/HANDOFF-SESI-BERIKUTNYA.md` lagi kalau ada temuan baru.

=== YANG SUDAH SELESAI (jangan diulang) ===

- Panel laptop: 3 tab (Lamaran/Pesan/Catatan), kolom kiri daftar + kolom kanan isi,
  header status menetap, baris aksi terkunci di bawah (tidak ikut tergulir), tab Catatan
  satu kolom tengah, scrollbar khusus `ScrollBarDriver` (hanya muncul kalau isi > viewport),
  footer "Tutup". Sheet 1080x620. Screenshot: `lap-15-laptop.png`, `lap-20-catatan.png`,
  `lap-21-pesan.png`.
- `SelaContent.Offers`: `OpeningDay`, `ClosingDay`, `StatusLabel(status, offer, day)`,
  `NotPublishedReason`, `SendBlockedReason`.
- `UiKit`: `Paragraph`, `Spacer`, `ActionRow`, `Column`, `VerticalDivider`, `ListButton`,
  scrollbar + inset.
- `UiKit.Button` NoWrap + auto-sizing; `TmpAutoHeight` menjembatani TMP;
  `UiScreen.OnOpen()` → `SetAsLastSibling()`.
- 63 lokasi `UiKit.TextIn`/`UiKit.Height` → `UiKit.Paragraph`.

=== HASIL AKHIR YANG AKU INGINKAN ===

1. Alur lamaran terbukti jalan sampai "Kirim", dengan screenshot tiap langkah.
2. Semua panel yang tersisa difoto, dan yang berantakan diperbaiki lalu difoto ulang.
3. `Docs/README-LUHUR.md` dan `Docs/HANDOFF-SESI-BERIKUTNYA.md` diperbarui.
4. Commit lokal di branch `UI,dll` dengan pesan yang jelas.
5. Laporan akhir dalam bahasa Indonesia: apa yang dikerjakan, apa yang ditemukan, apa yang
   belum selesai, dan langkah berikutnya.

Mulai dengan membaca `Docs/HANDOFF-SESI-BERIKUTNYA.md`, lalu langsung kerjakan TODO-1.
Jangan menunggu instruksi further — kerjakan sendiri sampai tuntas.
```

---

## Kenapa prompt ini dibuat seperti ini

- **Mandiri** — DeepSeek tidak punya riwayat percakapan ini, jadi semua konteks penting
  (jebakan uGUI, urutan recompile, cara pakai `UiTest`,-github blocker) ditulis inline.
- **Otomatis** — hanya satu keputusan yang perlu diambil user di sesi berikutnya: memberi
  hak Write ke `Luuhhuurr` di repo `minouse3/sela` supaya push bisa dilakukan.
- **Terukur** — setiap TODO punya kriteria terima yang jelas (screenshot + kondisi state),
  supaya AI berikutnya bisa mengaudit hasilnya sendiri.
