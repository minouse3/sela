# SELA
## Game Design Document

*Sebentar lagi harus menjawab. Belum tentu harus mengiyakan.*

![SELA - Key Art](visuals/00-key-art.jpg)

| Project Identity | Design Target |
| --- | --- |
| Platform | PC Windows, dimainkan secara luring |
| Engine | Unity, lingkungan utama 3D |
| Perspective | First-person, single-player |
| Genre | Psychological narrative, eksplorasi ruang, pengelolaan aktivitas ringan |
| Playtime | Target 45-60 menit untuk satu permainan pertama |
| Structure | Tujuh hari cerita, tiga sesi aktivitas per hari, tiga ending |
| Setting | Kamar kos dan lorong pendek di kota fiktif Indonesia |
| SDG Focus | SDG 3: Good Health and Well-Being; SDG 8: Decent Work and Economic Growth |
| Production Target | Tiga anggota, empat hingga enam minggu |

Angka durasi, performa, dan keseimbangan di dokumen ini merupakan sasaran desain yang perlu diuji melalui playtest, bukan hasil pengukuran produk jadi.

## 1. Game Overview

### 1.1 High Concept

Raka, lulusan Informatika yang sudah delapan bulan mencari kerja, memiliki tujuh hari sebelum masa sewa kosnya berakhir. Bekal kebutuhan dasar dan tiket bus untuk pagi hari kedelapan sudah tersedia. Ia ingin pulang membawa kabar baik, tetapi semakin sulit membedakan keinginannya sendiri dari tuntutan yang ia bayangkan datang dari orang lain.

Pemain menjalani hari dari sudut pandang Raka: memperbaiki contoh proyek, menyusun lamaran di laptop, memeriksa syarat kerja, dan sesekali meninggalkan meja untuk berbicara dengan seseorang. Ketika kalimat penolakan mulai memenuhi pikirannya, benda-benda di kamar berubah tempat. Meja terasa semakin jauh. Pintu memiliki nama yang tidak pernah dicat siapa pun. Pemain mengembalikan ruang dengan mencocokkan benda nyata dengan informasi yang sudah ia ketahui.

Pertanyaan utamanya adalah pekerjaan seperti apa yang patut diterima, serta batas apa yang masih ingin dipertahankan Raka. Mendapatkan pekerjaan merupakan salah satu hasil yang baik. Pulang sambil menyusun rencana berikutnya juga merupakan penutup yang sah.

### 1.2 Purpose of the Game

Pencarian kerja sering disederhanakan menjadi persoalan rajin melamar. Dalam cerita ini, Raka juga menghadapi informasi lowongan yang tidak jelas, tuntutan pengalaman untuk posisi pemula, dan rasa bersalah karena masih menerima bantuan keluarga. WHO menjelaskan bahwa ketidakamanan kerja, beban berlebihan, kontrol kerja yang rendah, serta pengangguran dapat berkaitan dengan risiko kesehatan mental [R3]. Game memakai konteks tersebut tanpa membuat klaim tentang prevalensinya pada seluruh lulusan Indonesia.

Tujuan sosial dan pembelajaran SELA adalah agar pemain dapat:

1. Membedakan bukti kemampuan dari klaim yang belum dapat dipertanggungjawabkan.
2. Menemukan tanda risiko dalam tawaran kerja, seperti penahanan dokumen asli, jam kerja tanpa batas yang jelas, dan desakan untuk segera menyetujui syarat.
3. Menjelaskan bahwa pekerjaan layak mencakup kondisi kerja dan hak pekerja, bukan hanya nominal penghasilan.
4. Mengenali kebutuhan untuk berhenti sejenak dan menghubungi orang lain ketika tekanan meningkat.

| SDG Alignment | Penerapan dalam permainan | Bukti yang dapat dibahas setelah bermain |
| --- | --- | --- |
| SDG 3, target 3.4: kesehatan mental dan kesejahteraan [R1] | Aktivitas pemulihan tetap tersedia; tekanan tinggi tidak mengakhiri permainan; dukungan sosial hadir dalam cerita | Pemain dapat menyebutkan keputusan yang membantu Raka mengambil jeda |
| SDG 8, target 8.5 dan 8.8: pekerjaan layak dan perlindungan hak pekerja [R2] | Pemain memeriksa kompensasi, jam kerja, ruang lingkup tugas, serta perlindungan dokumen sebelum memilih kontrak | Pemain dapat membandingkan dua tawaran berdasarkan syaratnya |

Game hanya menggunakan dua SDG tersebut. Keterkaitannya ditanamkan pada keputusan dan konsekuensi, bukan berupa kuis hafalan nomor SDG.

Tekanan merupakan abstraksi untuk kebutuhan permainan. Nilainya bukan diagnosis, ukuran kewarasan, atau prediksi kondisi seseorang. Merapikan kamar, tidur, dan percakapan singkat juga tidak digambarkan sebagai pengganti perawatan profesional.

### 1.3 Intended Use

SELA merupakan game naratif mandiri. Pemain dapat menyelesaikannya tanpa pengajar, akun daring, atau pengetahuan pemrograman. Semua informasi yang diperlukan untuk mengambil keputusan tersedia di dalam game.

Dalam kegiatan pembelajaran, game dapat menjadi pelengkap diskusi tentang transisi kuliah ke dunia kerja. Skenario penggunaan yang disarankan adalah pengantar singkat, satu sesi bermain, lalu diskusi sekitar 10-15 menit. Pengajar dapat membahas alasan pemain menerima atau menolak tawaran, bukti yang mendasari keputusan, serta pilihan yang tersedia selain terus menambah lamaran.

Contoh pertanyaan diskusi:

- Syarat apa yang membuat tawaran terlihat aman atau justru berisiko?
- Apakah Raka masih memiliki pilihan ketika sebuah lamaran ditolak?
- Bagian mana dari tekanan Raka berasal dari keputusan pribadi, dan bagian mana dari kondisi rekrutmen?

Game tidak ditujukan untuk terapi, diagnosis, rehabilitasi, atau pelatihan hukum ketenagakerjaan. Kontrak dan nama perusahaan di dalamnya bersifat fiktif. Pemain yang tidak nyaman dapat menurunkan intensitas atmosfer atau keluar tanpa kehilangan checkpoint terakhir.

### 1.4 Justification for Use

Wouters, van Nimwegen, van Oostendorp, dan van der Spek menemukan keunggulan rata-rata serious games pada pembelajaran dan retensi dalam meta-analisis mereka, tetapi tidak menemukan keunggulan motivasi yang signifikan dibandingkan pengajaran konvensional. Hasil pembelajaran juga lebih baik ketika game dilengkapi metode pengajaran lain [R4]. Temuan tersebut mendukung penggunaan keputusan interaktif dan diskusi pendamping, bukan menjamin efektivitas SELA.

Hipotesis desainnya: pemain lebih mudah membahas batas kerja ketika mereka harus membaca syarat, mempertimbangkan waktu, dan menyampaikan keputusan sebagai tokoh yang memiliki alasan personal. Efektivitas game ini belum dibuktikan. Evaluasi awal memakai pertanyaan sebelum dan sesudah bermain tentang tanda risiko lowongan dan unsur pekerjaan layak. Setelah bermain, peserta juga membaca satu contoh tawaran fiktif di luar cerita, lalu menyebutkan bagian yang perlu ditanyakan serta alasannya. Contoh ini menjadi bahan evaluasi terpisah dan tidak menambah lowongan dalam game. Catatan playtest tidak boleh disebut sebagai bukti klinis.

Identitas SELA terletak pada hubungan antara meja lamaran dan ruang fisik. Bukti yang pemain pelajari di meja juga menjadi pegangan ketika kamar berubah. Tidak ada mekanisme dari judul referensi yang dipindahkan sebagai paket permainan.

### 1.5 Target Audience

Sasaran utama adalah pemain berusia 18-25 tahun, terutama mahasiswa tingkat akhir dan pencari kerja awal karier. Pemain yang menyukai cerita pendek, eksplorasi, dan ketegangan tanpa pertarungan menjadi sasaran sekunder.

Bahasa utama adalah bahasa Indonesia. Istilah CV, portofolio, interview, dan kontrak tetap digunakan sesuai konteks. Materi sensitif mencakup penolakan kerja, tekanan finansial, perbandingan sosial, dan rekrutmen eksploitatif. Tidak ada visual melukai diri, kekerasan grafis, atau adegan bunuh diri.

### 1.6 Genre and Design Pillars

Genre utamanya adalah psychological narrative dengan eksplorasi first-person 3D. Horor muncul dari perubahan ruang, jeda percakapan, dan ketidaksesuaian informasi. Pemain tidak bertarung, bersembunyi dari musuh, atau menguji refleks.

Desain berpegang pada keputusan yang terbaca: konsekuensi aktivitas terlihat sebelum dipilih, syarat tawaran dapat diperiksa, dan ending dapat ditelusuri dari keputusan terakhir. Rutinitas dibuat pendek agar waktu bermain lebih banyak dipakai untuk memperhatikan perubahan dan menimbang pilihan.

## 2. Gameplay

### 2.1 Objectives

Tujuan utama adalah menentukan langkah Raka pada malam hari ketujuh. Pemain dapat menerima salah satu tawaran kerja yang memenuhi kebutuhannya atau pulang dengan rencana lanjutan. Pemain juga dapat menyetujui kontrak yang membatasi kebebasannya setelah melewati peringatan yang jelas.

Tujuan selama permainan adalah menyiapkan bukti kemampuan, menjaga ruang untuk pemulihan, mengikuti interview, dan mencari informasi sebelum menyetujui tawaran. Tidak semua aktivitas harus dilakukan untuk mencapai penutup yang utuh. Menghabiskan tujuh hari tanpa melamar tetap menghasilkan ending dengan konsekuensi naratif yang sesuai.

### 2.2 Core Loop

Pemain bangun di kamar kos, membaca pesan atau perubahan pada papan hari, lalu memilih satu aktivitas untuk sesi berjalan. Eksplorasi, membaca, dan memeriksa benda tidak memajukan sesi. Aktivitas yang menggunakan waktu selalu memiliki panel konfirmasi.

Pengenalan dilakukan bertahap: membaca E0 secara gratis, membuka lalu membatalkan panel aktivitas, menyelesaikan satu aktivitas pilihan, melihat pergantian sesi, menyusun CV, lalu meninjau panel pengiriman. Pemain boleh menunda lamaran; urutan pengenalan tidak memaksa pengiriman atau menghabiskan sesi tambahan. Penjelasan snapshot muncul sebelum pengiriman pertama dan dapat dibaca kembali di Catatan.

Setelah aktivitas selesai, pemain melihat perubahan kondisi dan hasilnya. Jadwal berpindah dari pagi ke siang, kemudian malam. Setelah sesi malam, pemain menutup hari melalui kasur. Tidur tidak menghabiskan sesi tambahan. Pada hari tertentu, perubahan ruang terjadi sebelum sesi berikutnya terbuka.

```mermaid
flowchart TD
    A[Awal hari dan pesan baru] --> B[Eksplorasi kamar atau laptop]
    B --> C[Pilih aktivitas dan baca biayanya]
    C --> D[Konfirmasi satu sesi]
    D --> E[Hasil aktivitas dan pemeriksaan tekanan]
    E --> F{Peristiwa ruang terjadwal?}
    F -->|Ya| G[Selesaikan Retakan Ruang]
    F -->|Tidak| H{Sesi malam selesai?}
    G --> H
    H -->|Belum| B
    H -->|Sudah, hari 1 sampai 6| I[Tidur dan mulai hari berikutnya]
    I --> A
    H -->|Sudah, hari 7| J[Keputusan akhir dan epilog]
```

Target pembagian durasi adalah sekitar lima menit pengenalan, 25-35 menit rutinitas serta keputusan, 6-9 menit untuk tiga Retakan Ruang, dan 5-8 menit penutup. Sebagian pemain dapat selesai lebih cepat karena tidak ada waktu tunggu wajib pada teks.

### 2.3 Game Progression and Play Flow

| Day | Perkembangan utama | Pilihan yang ditekankan |
| --- | --- | --- |
| 1 | Raka membaca pesan ibu, mengenali bekal, dan membuka lowongan Studio Lajur | Menyiapkan bukti atau langsung mengirim klaim yang belum siap |
| 2 | Penolakan lama muncul kembali; Retakan Ruang pertama berlangsung | Memisahkan riwayat penolakan dari fakta tentang proyek yang pernah selesai |
| 3 | Revisi kedua portofolio tersedia; Koperasi Pijar membuka lowongan | Memilih waktu belajar, pemulihan, dan lamaran berikutnya |
| 4 | Pesan Akses Prima datang; Retakan Ruang kedua berlangsung | Memeriksa tawaran yang terdengar paling mudah |
| 5 | Batas lamaran Koperasi Pijar berakhir; Catatan mengajak meninjau hal yang belum jelas pada tawaran | Memeriksa pendampingan, lembur, atau kelanjutan kontrak; jika belum ada tawaran, meninjau bukti dan kesempatan yang tersisa |
| 6 | Interview terakhir jika terjadwal; pesan ibu tentang tiket hadir sejak pagi; Retakan ketiga; perbandingan pilihan di Catatan | Menghubungkan syarat kerja dengan kebutuhan Raka dan membedakan pesan nyata dari tuntutan yang dibayangkan |
| 7 | Ibu menanyakan jadwal keberangkatan atau rencana tinggal; Raka menyiapkan jawaban terakhir | Memilih satu batas kerja untuk A/B atau satu langkah setelah pulang, lalu mengunci keputusan pada malam hari |

Urutan informasi utama tetap. Dialog, isi map lamaran, dan tampilan kamar menyesuaikan riwayat aktivitas. Tidak ada rotasi lowongan acak atau cabang yang memerlukan level baru.

Pesan hari 5-7 memakai tiga konteks: dua tawaran resmi, satu tawaran resmi, atau belum ada tawaran resmi. Semua tampil melalui Pesan dan Catatan yang sudah ada. Ajakan meninjau syarat tidak menahan informasi penting sampai hari tersebut; hasil verifikasi tetap dapat diperoleh lebih awal. Pembacaan dan perbandingan gratis sesi. Sesi yang tersisa tetap dapat dipakai untuk pemulihan, dengan teks singkat pada pengulangan agar pemain tidak mengulang percakapan panjang.

### 2.4 Mission, Challenge, and Puzzle Structure

Tantangan pengelolaan waktu menggunakan 21 sesi. Menyelesaikan dua bukti, dua verifikasi, dua lamaran, dan dua interview memerlukan delapan sesi; memeriksa Akses Prima menambah satu sesi. Pemain dapat memperoleh kedua tawaran resmi sejak hari 4. Hari 5-7 berfokus pada memahami syarat, kebutuhan pribadi, dan jawaban akhir. Playtest mencatat apakah sesi setelah persiapan selesai masih terasa berguna. Persiapan tidak menuntut penguasaan bahasa pemrograman secara nyata.

Tantangan lamaran menghubungkan satu klaim CV dengan hasil kerja yang dapat ditunjukkan. Pemain boleh memilih kalimat berlebihan, tetapi game memperlihatkan ketidaksesuaian tersebut dan menyediakan kesempatan untuk meluruskannya saat interview. Tidak ada hukuman blacklist lintas perusahaan.

Tantangan kontrak meminta pemain menelusuri empat aspek: tugas, kompensasi, jam kerja, dan dokumen pribadi. Tawaran resmi juga harus dibaca; label perusahaan bukan jaminan otomatis.

Tiga puzzle Retakan Ruang menggunakan benda yang telah diperkenalkan. Pemain membawa satu benda ke titik jangkar yang sesuai. Tidak ada kode acak, musuh, batas waktu, atau kematian. Kegagalan menempatkan benda hanya menghasilkan petunjuk. Ketegangan berasal dari ruang yang tidak masuk akal, bukan ancaman kehilangan progres.

### 2.5 Example Play Session

Pada pagi hari ketiga, Raka memiliki Tenaga 3 dan Tekanan 2. Pemain menyelesaikan bukti kedua: Tenaga menjadi 2, Tekanan menjadi 3, dan sesi berganti ke siang. Pemain kemudian memeriksa Koperasi Pijar: Tenaga menjadi 1, Tekanan tetap 3. Pada malam hari, pemain mengirim lamaran dengan dua bukti: Tenaga menjadi 0 dan Tekanan menjadi 4. Interview dicatat untuk siang hari keempat.

Setelah tidur, Raka memulai hari keempat dengan Tenaga 2 dan Tekanan 3. Pemain masih mempunyai sesi pagi untuk beristirahat sebelum interview. Tidak ada peluang acak yang menentukan diterima atau ditolaknya lamaran.

## 3. Mechanics

### 3.1 Rules and State Variables

| State | Rentang atau bentuk | Nilai awal | Fungsi |
| --- | --- | --- | --- |
| `day` | 1-7 | 1 | Mengatur peristiwa dan batas lamaran |
| `session` | Pagi, Siang, Malam | Pagi | Menentukan aktivitas yang sedang tersedia |
| `sessionResolved` | Benar atau salah | Salah | Menandai bahwa aktivitas sesi berjalan sudah selesai; malam yang selesai hanya dapat ditutup melalui kasur |
| `energy` / Tenaga | Bilangan bulat 0-4 | 3 | Biaya aktivitas yang memerlukan konsentrasi |
| `pressure` / Tekanan | Bilangan bulat 0-6 | 1 | Mengatur intensitas atmosfer dan kebutuhan jeda |
| `evidence` | Himpunan kartu E0, E1, E2 | E0 | Menyimpan seluruh bukti yang dimiliki; E1/E2 ditambahkan tanpa menghapus E0 |
| `cvDraft` | Dua klaim dan daftar bukti terpasang | Klaim sesuai peran awal; bukti lamaran kosong | Draf yang dapat diedit tanpa mengubah snapshot terkirim |
| `application[A,B]` | Belum dikirim, terjadwal, ditolak, ditawari | Belum dikirim | Status dua lamaran resmi; menarik pilihan tidak menghapus tawaran |
| `verification[A,B,C]` | Belum diperiksa atau diperiksa | Belum diperiksa | Kelengkapan catatan tawaran |
| `offerChoice` | Tidak ada, A, B, C | Tidak ada | Pilihan sementara sebelum keputusan final |
| `decisionNote` | Belum dipilih atau satu catatan sesuai pilihan final | Belum dipilih | Batas kerja A/B atau langkah setelah pulang; tidak menentukan kelayakan tawaran |
| `riftSolved[1..3]` | Benar atau salah | Semua salah | Progres tiga perubahan ruang |
| Daily flags | Sudah merapikan, sudah jeda otomatis | Salah | Membatasi aksi sekali per hari |

Nilai Tenaga dan Tekanan selalu dibatasi pada rentangnya setelah perubahan. Tidak ada penurunan stat karena pemain berdiam diri, membaca lambat, membuka menu, atau menjelajahi kamar. Keahlian yang sudah dibuktikan tidak hilang akibat beristirahat.

![HUD dan Antarmuka Status Permainan](visuals/03-hud-gameplay-free-roam.webp)

![HUD, Focus, dan Status Interaksi](visuals/12-hud-focus-states.jpg)

Raka memiliki persediaan kebutuhan dasar untuk tujuh hari dan tiket pulang yang telah dibeli. Keduanya merupakan informasi cerita, bukan item yang bisa dijual atau habis karena aktivitas. Tidak ada simulasi kelaparan, uang harian, toko, atau utang. Batas finansial hadir sebagai berakhirnya masa sewa, sehingga permainan tidak membutuhkan ekonomi tambahan.

### 3.2 Time, Scheduling, and Resolution Order

Setiap aktivitas pada tabel 3.3 menghabiskan tepat satu sesi. Sebelum konfirmasi, panel menyebutkan nama aktivitas, perubahan kondisi, serta sesi setelahnya. Membatalkan panel tidak mengubah apa pun.

Urutan penyelesaian adalah: validasi syarat, jalankan interaksi aktivitas, terapkan biaya dan hasil satu kali, periksa kebutuhan Jeda, jalankan Retakan Ruang terjadwal, pindahkan sesi, lalu simpan checkpoint. Biaya dipastikan melalui konfirmasi sebelum interaksi dimulai. Setelah konfirmasi, aktivitas harus diselesaikan atau diulang dari checkpoint jika pemain keluar; percobaan jawaban di dalam aktivitas tidak membeli sesi lagi.

Saat hasil diterapkan, `sessionResolved` menjadi Benar. Setelah aktivitas pagi atau siang beserta peristiwanya selesai, sesi berpindah dan flag kembali Salah. Setelah aktivitas malam, sesi tetap Malam dan flag tetap Benar; semua tombol aktivitas satu sesi tidak aktif dengan keterangan Malam sudah selesai. Pemain masih dapat membaca, mengedit CV, dan meninjau tawaran. Tidur memulai hari berikutnya dengan sesi Pagi, flag sesi Salah, serta flag harian yang direset. Pada malam ketujuh, kasur membuka keputusan final, bukan hari kedelapan.

Tidur pada akhir hari 1-6 menambah Tenaga 2 dan mengurangi Tekanan 1. Tidak ada aktivitas lembur. Pemain yang ingin mengakhiri hari lebih cepat harus memilih aktivitas pemulihan untuk sesi yang tersisa; tidak ada tombol yang diam-diam melewati jadwal interview.

Interview selalu memakai sesi siang hari setelah lamaran dikirim. Pemain boleh melakukan aktivitas lain pada sesi tersebut setelah menyetujui peringatan bahwa undangannya akan terlewat. Interview yang terlewat berstatus ditolak dan menaikkan Tekanan 1 di samping efek aktivitas penggantinya, sebelum pemeriksaan Jeda. Tidak ada penolakan ganda untuk undangan yang sama.

Panel pengiriman mencantumkan Interview besok siang: perlu Tenaga 1. Pada awal pagi hari interview, kartu jadwal mengulang syarat itu bersama Tenaga saat ini agar pemain sempat memulihkan kondisi. Pada Tenaga 0 di sesi interview, pilihan pemulihan tetap tersedia dengan peringatan bahwa undangan akan terlewat. Tidak ada pemindahan jadwal otomatis.

![Kalender, Pergantian Sesi, dan Tidur](visuals/14-calendar-sleep.jpg)

### 3.3 Actions

| Action | Lokasi | Tenaga | Tekanan | Syarat dan hasil |
| --- | --- | --- | --- | --- |
| Rapikan contoh antarmuka | Meja/laptop | −1 | +1 | Sekali; menghasilkan E1 |
| Lengkapi catatan pengujian | Meja/laptop | −1 | +1 | Mulai hari 3, sudah memiliki E1; sekali; menghasilkan E2 |
| Periksa perusahaan | Laptop | −1 | 0 | Sekali per perusahaan; membuka ringkasan verifikasi |
| Kirim lamaran | Laptop | −1 | +1 | Sekali untuk A dan sekali untuk B selama jendela lamaran; menyimpan snapshot CV dan jadwal |
| Ikuti interview | Laptop | −1 | +2 | Hanya pada undangan yang sesuai; dua pertanyaan dan hasil langsung |
| Beristirahat | Kasur | +3 | −2 | Selalu tersedia, dapat diulang |
| Hubungi Dita | Ponsel di meja | +2 | −2 | Selalu tersedia; percakapan pertama tiap hari berbeda, pengulangan memakai teks singkat |
| Duduk tanpa layar | Kursi dekat jendela | +2 | −2 | Selalu tersedia; animasi singkat yang dapat dilewati |
| Rapikan meja | Meja fisik | +1 | −1 | Maksimal sekali sehari; mengganti susunan benda, tidak memengaruhi penilaian interview |

Aktivitas dengan biaya Tenaga 1 memerlukan Tenaga minimal 1. Pemain selalu dapat berjalan dan memilih aktivitas pemulihan pada Tenaga 0. Tidak ada pingsan atau penguncian semua pilihan.

Beristirahat pada siang hari adalah aktivitas pemulihan satu sesi, berbeda dengan tidur penutup hari. Mengulang aktivitas pemulihan sah; pemain menerima konsekuensi berupa berkurangnya kesempatan menyiapkan lamaran, bukan pengurangan kecerdasan atau penilaian moral.

Panel pemulihan menyebut pengalaman yang menyertai angka: Beristirahat memulihkan Tenaga paling banyak; Hubungi Dita menawarkan percakapan pertama hari itu; Duduk tanpa layar memberi pengamatan singkat; Rapikan meja memperlihatkan perubahan benda. Percakapan Dita memakai satu teks dasar per hari dengan sisipan pendek sesuai hasil lamaran. Duduk memakai tiga pengamatan untuk awal, tengah, dan akhir minggu; merapikan meja memakai dua catatan benda. Pengulangan memakai respons singkat tanpa membuka syarat kerja atau bantuan eksklusif. Semua aktivitas tetap memakai biaya dan hasil pada tabel.

Animasi hasil aktivitas berdurasi sekitar 2-4 detik dan bisa dilewati setelah hasil logis diterapkan. Pilihan singkat dalam pekerjaan E1/E2 dijelaskan pada bagian 3.5 dan tidak memakai batas waktu. Melewati animasi tidak mengubah hasil.

![Konfirmasi, Ketersediaan, dan Hasil Aktivitas](visuals/13-activity-states.jpg)

![Aktivitas Fisik di Kamar](visuals/24-room-activities.jpg)

### 3.4 Pressure and the Jeda Sequence

Pada Tekanan 0-3, benda memiliki posisi normal dan suara kamar terdengar utuh. Pada Tekanan 4-5, bayangan kusen bergeser sedikit dan suara notifikasi memiliki ekor gema pendek. Teks, jalur jalan, dan objek penting tetap jelas. Perubahan struktural hanya terjadi pada Retakan Ruang terjadwal.

Jika Tekanan mencapai 6, aktivitas selesai lebih dahulu. Permainan kemudian membuka Jeda: Raka dapat duduk sebentar atau mengirim pesan singkat kepada Dita. Keduanya menurunkan Tekanan menjadi 4 dan menetapkan Tenaga menjadi minimal 1. Urutan singkat ini tidak menggunakan sesi dan dapat dilewati melalui tombol Lanjut. Tidak ada latihan pernapasan wajib.

Jeda otomatis hanya terjadi sekali per hari. Jika Tekanan kembali mencapai 6 pada hari yang sama, nilainya tetap 6 sampai pemain pulih atau tidur; tampilan dan audio mempertahankan perlakuan Tekanan 4-5 tanpa menambah efek baru. Tidak ada pengulangan paksa. Tekanan mengatur suasana serta kesempatan jeda, tanpa menentukan penerimaan kerja, ending, atau ketepatan jawaban. Pratinjau aktivitas menyebut jika Jeda pertama akan aktif; ringkasan akhir menampilkan nilai setelah seluruh akibat aktivitas dan Jeda diselesaikan.

![Tingkat Tekanan dan Jeda](visuals/15-pressure-jeda.jpg)

### 3.5 Evidence and CV Assembly

Meja Bukti adalah tampilan dekat papan fisik di atas meja. Pemain memilih kartu melalui klik, kemudian memilih penjepit tujuan. Tidak perlu drag-and-drop presisi. Kartu yang dipilih dapat dikembalikan tanpa biaya sesi.

![Meja Bukti dan Penyusunan Portofolio](visuals/05-papan-bukti-closeup.webp)

| Evidence | Isi yang terlihat | Sumber | Penggunaan |
| --- | --- | --- | --- |
| E0: Proyek kuliah | Foto hasil proyek dan keterangan bahwa Raka bekerja sebagai anggota tim | Sudah tersedia sejak awal | Pengenalan tokoh dan jangkar Retakan pertama; belum memenuhi kebutuhan lowongan |
| E1: Antarmuka formulir | Gambar sebelum/sesudah, label input, pesan kesalahan yang diperbaiki | Aktivitas Rapikan contoh antarmuka | Syarat bukti untuk A dan B |
| E2: Catatan pengujian | Tiga kasus uji: kolom kosong, format salah, dan data valid; hasil serta batas pengujian | Aktivitas Lengkapi catatan pengujian | Syarat tambahan untuk B |

![Pekerjaan Portofolio dan Bukti E1/E2](visuals/25-portfolio-work.jpg)

Setiap pekerjaan bukti memiliki satu pilihan singkat melalui kartu yang sama dengan Meja Bukti. Pada E1, pemain membandingkan dua pasangan label dan pesan kesalahan: Input / Error dengan Alamat email / Isi alamat email dengan format yang sesuai, lalu memilih pasangan yang lebih membantu pengguna. Pada E2, ketiga kasus uji sudah terlihat; pemain memilih kesimpulan antara Tiga kasus yang dicatat berjalan sesuai hasil ini dan Seluruh kemungkinan kesalahan sudah teruji. Jawaban yang kurang tepat diberi alasan dan boleh dipilih ulang tanpa biaya, perubahan stat, atau pengurangan kualitas bukti. Setelah pilihan selesai, kartu E1/E2 diberikan dan satu biaya aktivitas diterapkan. Tidak ada editor kode atau tantangan mengetik.

Hasil E1 memperlihatkan perubahan yang dikerjakan Raka. Hasil E2 menyebut tiga kasus beserta batas pemeriksaannya. Isi tersebut tetap dapat dibaca di Catatan dan menjadi rujukan jawaban interview. Pilihan dalam persiapan tidak menghasilkan skor atau bukti tambahan.

CV memiliki dua baris klaim. Baris pertama memilih antara membantu pengembangan antarmuka proyek kuliah dan memimpin sistem produksi perusahaan. Baris kedua memilih antara menguji formulir secara manual dan menjamin sistem bebas seluruh kesalahan. Pilihan berlebihan selalu diberi catatan bahwa bukti saat ini tidak mendukungnya. Pemain tetap boleh menggunakannya.

Pemain memasang E1/E2 yang sudah dimiliki dan memilih kalimat klaim. Menyusun, mengganti, atau membaca CV tidak menggunakan sesi. Panel sebelum pengiriman menampilkan perusahaan, kedua klaim, kartu terpasang, bukti wajib yang belum ada, jadwal, serta biaya. Lamaran menyimpan klaim dan bukti pada saat dikirim; perubahan sesudahnya tidak mengubah lamaran yang telah masuk. Riwayat terkirim bersifat baca-saja dan terpisah dari Draf saat ini. Peringatan sebelum konfirmasi menyebut bahwa bukti yang dibuat kemudian tidak ditambahkan ke lamaran lama.

Saat interview, pemain boleh mengoreksi seluruh klaim berlebihan melalui satu pilihan pembuka. Koreksi dicatat sebagai penjelasan interview untuk lamaran tersebut; teks asli dalam snapshot tetap tersimpan. Koreksi memenuhi syarat kejujuran tanpa menambahkan bukti yang tidak ikut dikirim. Tidak ada blacklist global, jawaban yang hilang karena stat, atau hukuman terhadap perusahaan lain.

![Klaim CV dan Snapshot Lamaran](visuals/16-cv-snapshot.jpg)

### 3.6 Laptop and Offer Verification

Laptop berisi tiga tab: Lamaran, Pesan, dan Catatan. Lamaran memuat CV, lowongan, serta tabel tawaran. Pesan memuat undangan, hasil, pesan ibu, dan perekrut. Catatan menyimpan bukti serta fakta untuk puzzle. Tidak ada browser bebas atau aplikasi tambahan.

![Antarmuka Layar Laptop dan Verifikasi Tawaran](visuals/04-antarmuka-laptop-verifikasi.webp)

![Pesan, Catatan, dan Kontak Dukungan](visuals/19-messages-notes.jpg)

Membaca iklan dan kontrak tidak menggunakan sesi. Aktivitas Periksa perusahaan tersedia setelah lowongannya dibuka atau pesan tawaran langsung diterima. Satu sesi menampilkan rangkuman profil, kontak pembanding, dan jawaban klarifikasi atas pertanyaan yang tercantum di panel. Seluruhnya berupa konten lokal yang telah ditulis, bukan akses internet sebenarnya. Hasil pemeriksaan dapat dibaca ulang gratis.

Pemain menandai empat aspek sebagai Jelas, Perlu ditanyakan, atau Berisiko: tugas; kompensasi; jam kerja dan istirahat; perlindungan dokumen pribadi. Penandaan dapat diubah tanpa biaya. Setelah verifikasi, catatan pembanding menunjukkan bagian yang mendukung atau bertentangan dengan penandaan pemain. Game tidak menyembunyikan informasi keselamatan sampai setelah keputusan final.

| Offer | Jendela dan syarat | Isi tawaran fiktif | Hasil verifikasi |
| --- | --- | --- | --- |
| A: Studio Lajur, Junior UI Implementer | Lamaran hari 1 pagi sampai hari 3 malam; E1 harus ikut dikirim untuk memenuhi syarat tawaran | Rp4.200.000 per bulan; 40 jam kerja per minggu; dua hari istirahat; uraian tugas antarmuka; dokumen asli tetap dipegang pekerja; jaminan sosial disebutkan tertulis | Profil dan kontak sesuai; pendamping ditetapkan; lembur memerlukan persetujuan dan pencatatan kompensasi |
| B: Koperasi Pijar, Web Support | Lamaran hari 3 pagi sampai hari 5 malam; E1 dan E2 harus ikut dikirim untuk memenuhi syarat tawaran | Kontrak tiga bulan; Rp3.800.000 per bulan; 35 jam kerja per minggu; dua hari istirahat; tugas pemeliharaan formulir; pendamping; jaminan sosial disebutkan tertulis | Kontak dapat dikonfirmasi; tugas awal dibatasi pada formulir; evaluasi menjelang akhir kontrak tidak menjamin pembaruan; dokumen asli tidak diminta |
| C: Akses Prima, Operator Digital | Pesan hadir hari 4 pagi; tawaran berlaku sampai hari 7 malam; tanpa lamaran dan interview | Mengiklankan penghasilan sampai Rp12.000.000; jadwal mengikuti kebutuhan; kontrak meminta penyerahan dokumen asli dan membatasi pengunduran diri | Alamat dan kontak tidak konsisten; rincian tugas ditolak; syarat penahanan dokumen tetap dipertahankan |

Nominal tersebut hanya parameter cerita. Kota tidak mewakili wilayah hukum tertentu. Game tidak menyebut nominal tertentu sebagai upah minimum yang sah dan tidak memeringkat A di atas B berdasarkan gaji atau status kontrak.

Setiap hasil pemeriksaan menampilkan Pernyataan awal dan Jawaban klarifikasi pada empat aspek yang sama. Pertanyaan utama A adalah siapa yang mendampingi pekerjaan baru dan bagaimana lembur disetujui. Pertanyaan B adalah tugas awal serta kepastian setelah kontrak tiga bulan; jawaban mengakui bahwa kelanjutannya belum pasti. Pertanyaan C adalah pekerjaan harian dan alasan meminta dokumen asli; admin tetap menolak merinci tugas serta mempertahankan syarat dokumen. Penandaan pemain disandingkan dengan temuan tanpa skor benar/salah. Informasi yang belum terjawab tetap berlabel Perlu ditanyakan.

Akses Prima menawarkan penghasilan tinggi, proses cepat, dan janji penempatan sehingga menarik bagi Raka yang ingin segera membawa kabar baik. Drafnya sudah memperlihatkan syarat berisiko sebelum diverifikasi. Pemeriksaan memperlihatkan ketidakjelasan informasi dan kondisi eksploitatif; game tidak menyimpulkan adanya tindak pidana hanya dari satu tanda. Tidak ada negara atau kebangsaan yang dipakai sebagai penanda penipuan.

Lamaran resmi dengan bukti yang belum cukup tetap dapat dikirim dan mendapat undangan karena kedua perusahaan menyediakan sesi penjelasan portofolio dalam cerita. Panel memperingatkan bahwa tawaran tidak dapat diberikan jika bukti wajib tidak ikut dikirim, dan setiap perusahaan hanya menerima satu lamaran. Undangan tidak sama dengan kelulusan. Verifikasi menambah informasi pembanding tanpa menjadi syarat tersembunyi untuk menerima tawaran.

Setiap perusahaan resmi hanya menerima satu lamaran dalam rentang cerita. Jika sesi interview besok siang sudah terisi, tombol kirim untuk perusahaan lain tidak aktif dan menjelaskan benturan jadwal. Pemain dapat menunggu hari berikutnya selama jendela lamaran masih terbuka. Tidak ada undangan yang diam-diam menimpa undangan lain.

![Rekrutmen, Verifikasi, dan Jadwal Lamaran](visuals/17-recruitment-verification.jpg)

### 3.7 Interview and Results

Interview memakai potret 2D dengan tiga ekspresi, dialog teks, dan dua pertanyaan. Suara ketikan serta nada sambungan menggantikan kebutuhan voice acting penuh. Tidak ada webcam pemain, kamera sekunder, atau penilaian penampilan kamar.

Sebelum pertanyaan pertama, pemain dapat meluruskan klaim CV jika diperlukan. Semua pilihan dialog tetap tampil. Catatan bukti yang dikirim dapat dibuka selama interview tanpa biaya atau batas waktu.

| Interview | Pertanyaan dan respons | Nilai internal |
| --- | --- | --- |
| A, pertanyaan 1: Apa yang diperbaiki dan bagaimana kamu memeriksanya? | Menjelaskan perubahan label dan pesan kesalahan dengan E1 / menjelaskan bahwa label perlu spesifik tanpa contoh hasil / menganggap satu pengiriman berhasil sudah membuktikan semua pesan kesalahan tepat | 2 jika E1 dikirim, jika tidak 0 / 1 / 0 |
| A, pertanyaan 2: Bagaimana kamu menangani bagian yang belum dikuasai? | Menjelaskan bagian kerja sendiri lalu meminta pendampingan untuk bagian baru / meminta contoh tugas sebelum memperkirakan kebutuhan belajar / menyetujui seluruh tugas dahulu dan membahas kebutuhan bantuan setelah tenggat | 2 / 1 / 0 |
| B, pertanyaan 1: Apa yang dapat disimpulkan dari catatan pengujianmu? | Menjelaskan tiga kasus E2 dan batas cakupannya / menjelaskan perlunya mengulang pengujian setelah perbaikan tanpa merujuk kasus tercatat / menganggap percobaan data valid cukup untuk menyatakan formulir siap pada semua kondisi | 2 jika E2 dikirim, jika tidak 0 / 1 / 0 |
| B, pertanyaan 2: Bagaimana kamu memulai pekerjaan pemeliharaan? | Meminta prioritas dan ruang lingkup perbaikan lalu menyebut pemeriksaan dampaknya / meminta satu contoh laporan yang paling mendesak / langsung mengubah setiap laporan sesuai urutan masuk tanpa memastikan prioritas dan dampak | 2 / 1 / 0 |

Perusahaan memberi tawaran jika bukti wajib ada pada snapshot pengiriman, tidak ada klaim berlebihan yang belum dikoreksi, setiap jawaban bernilai minimal 1, dan total dua jawaban minimal 3 dari 4. Semua syarat bersifat deterministik. Tekanan, frekuensi merapikan kamar, dan aktivitas sosial tidak masuk perhitungan.

Jika beberapa syarat gagal, alasan utama ditampilkan dengan urutan bukti wajib belum lengkap, klaim belum diluruskan, lalu penjelasan jawaban belum cukup. Catatan hasil menyimpan alasan lain yang juga berlaku tanpa menampilkan nilai angka. Pesan menyebut kebutuhan posisi yang belum ditunjukkan dan langkah yang dapat dicoba pada kesempatan berikutnya. Penolakan menambah Tekanan 1 setelah biaya interview dan sebelum pemeriksaan Jeda. Hasil disimpan satu kali sehingga membuka pesan berulang tidak menambah tekanan.

Aturan deterministik hanya berlaku pada dua proses rekrutmen fiktif ini. Pesan Dita mengingatkan bahwa kecocokan posisi, kebutuhan perusahaan, dan jumlah kesempatan juga memengaruhi pencarian kerja. Penolakan lama tidak diberi penjelasan seolah semua terjadi karena kesalahan Raka.

Tawaran A/B yang berhasil diperoleh tetap berlaku sampai malam hari ketujuh. Pemain dapat memilih, mengganti pilihan, atau menarik pilihannya sebelum finalisasi. Menutup panel tanpa memilih bukan penolakan. Penarikan tawaran yang telah dipilih menggunakan konfirmasi, lalu mengembalikan `offerChoice` ke Tidak ada.

![Sesi Interview dan Hasil Rekrutmen](visuals/18-interview-results.jpg)

### 3.8 Retakan Ruang

Retakan terjadi sesudah aktivitas siang pada hari 2, 4, dan 6, termasuk jika aktivitasnya adalah pemulihan atau interview. Retakan tidak menggunakan sesi, tidak menaikkan Tekanan, dan tidak mengubah hasil rekrutmen. Cerita menyajikannya sebagai pengalaman subjektif Raka, tanpa diagnosis atau penjelasan supernatural yang pasti.

| Rift | Perubahan dan tujuan | Solusi | Umpan balik |
| --- | --- | --- | --- |
| 1: Meja yang Menjauh | Versi kamar memanjang; lembar penolakan tergantung di antara pemain dan meja | Ambil kartu E0 dari rak awal dan pasang pada penjepit berbentuk sudut foto di ujung meja | Panjang kamar kembali normal; tulisan Aku belum pernah menyelesaikan apa pun diganti keterangan proyek yang faktual |
| 2: Pintu Bernama | Lorong menampilkan tiga pintu: DITERIMA, TERLAMBAT, dan 07 | Ambil gantungan kunci 07 dari bangku lalu gunakan pada pintu 07, sesuai nomor kamar dan catatan yang tersedia | Dua pintu palsu menjadi bidang dinding; pintu 07 membuka kamar |
| 3: Kursi Kosong | Di sekeliling meja terdapat kursi tambahan dengan label tuntutan yang tidak pernah diucapkan | Ambil kartu pesan ibu, Tiketnya tetap boleh dipakai, dan letakkan pada dudukan di kursi dekat jendela | Kursi tambahan hilang; pemain mendengar sisa suara kamar tanpa penilaian menang atau kalah |

![Retakan Ruang 1: Meja yang Menjauh](visuals/07-retakan-ruang-1-meja-menjauh.webp)

![Retakan Ruang 2: Pintu Bernama](visuals/08-retakan-ruang-2-pintu-bernama.webp)

![Retakan Ruang 3: Kursi Kosong](visuals/09-retakan-ruang-3-kursi-kosong.webp)

E0, nomor 07, dan pesan ibu selalu tersedia untuk puzzle masing-masing, terlepas dari pilihan lamaran atau apakah pemain sudah membaca pesan secara sukarela. Saat memasuki Retakan, benda tersebut muncul sebagai salinan khusus puzzle; inventaris cerita tidak terhapus.

E0 diperkenalkan pada awal permainan dengan keterangan bagian antarmuka yang dikerjakan Raka bersama tim. Nomor 07 terlihat pada pintu dan kunci sejak hari 1. Pesan ibu tentang tiket masuk pada pagi hari 6 sebelum Retakan ketiga; percakapan hari 7 membahas tindak lanjutnya. Petunjuk menghubungkan fakta dengan masalah ruang: hasil kerja melawan pernyataan belum pernah selesai, nomor kamar membedakan alamat dari label hasil, dan pesan ibu membedakan ucapan nyata dari dugaan Raka.

Setiap Retakan memakai paling banyak dua kalimat pembuka alternatif berdasarkan riwayat yang sudah ada. Pada Retakan pertama, kalimat menyesuaikan apakah E1 sudah selesai. Pada Retakan kedua, tawaran resmi yang sudah diperoleh memunculkan kekhawatiran salah memilih; jika belum ada, kekhawatirannya tertinggal. Pada Retakan ketiga, pilihan kerja sementara memunculkan kebutuhan menjelaskan batas, sedangkan belum memilih memunculkan kebutuhan menjelaskan rencana pulang. Variasi teks tidak mengubah layout, solusi, atau akses bantuan. Ruang kembali normal setelah solusi, sementara ketidakpastian kerja tetap ada.

Pemain hanya membawa satu benda puzzle pada satu waktu. Klik tujuan yang salah mempertahankan benda dan menampilkan penjelasan singkat. Menekan E pada dudukan asal mengembalikannya. Pilihan Tampilkan petunjuk tersedia sejak awal; setelah tiga percobaan salah, label tujuan muncul otomatis. Pemain juga dapat memilih Selesaikan adegan dari menu jeda untuk melihat pemulihan ruang dengan hasil cerita yang sama. Tidak ada penalti atau ending yang terkunci.

![Interaksi Puzzle, Petunjuk, dan Bantuan](visuals/20-puzzle-help.jpg)

Perubahan skala dan posisi terjadi saat layar memudar singkat, bukan dengan meregangkan kamera ketika pemain berjalan. Area puzzle menggunakan collider tetap dan jalur tunggal yang terbaca. Tidak ada labirin prosedural atau sistem portal.

### 3.9 Controls and Camera

| Input | Free Roam | Focus / Laptop / Puzzle |
| --- | --- | --- |
| W, A, S, D | Bergerak | Tidak menggerakkan karakter saat panel fokus terbuka |
| Mouse | Menoleh | Menggerakkan kursor |
| E | Berinteraksi dengan objek yang disorot | Mengambil atau memasang benda puzzle jika panel tertutup |
| Klik kiri | Tidak menjalankan aktivitas tanpa prompt | Memilih tombol, kartu, dan respons |
| Scroll | Tidak ada fungsi wajib | Menggulir teks |
| Tab | Membuka Catatan | Menutup Catatan dan kembali ke keadaan sebelumnya |
| Esc | Membuka Pause | Menutup panel teratas dahulu; dari puzzle membuka Pause |
| Tombol panah | Tidak ada fungsi wajib | Memindahkan fokus antarkontrol dan pilihan dialog |
| Enter / Spasi | Tidak menjalankan aktivitas dunia | Mengaktifkan kontrol yang mendapat fokus; konfirmasi sesi tetap melalui panel |
| Page Up / Page Down | Tidak ada fungsi wajib | Menggulir panel baca yang sedang mendapat fokus |

Tidak ada sprint, lompat, jongkok, atau combat. Kecepatan jalan target 2,2 meter per detik; tinggi kamera 1,6 meter. FOV awal 70 derajat dan dapat diatur 60-90 derajat. Sensitivitas serta invert Y tersedia. Head bob dan camera shake tidak digunakan.

Raycast interaksi memiliki jarak maksimal 2 meter dan mengikuti collider terdekat yang menghalangi pandangan. Objek di balik dinding tidak bisa dipilih. Prompt hanya muncul pada target sah dan menjelaskan aksi, misalnya E · Baca catatan. Tombol aktivitas satu sesi berada di panel terpisah agar interaksi awal tidak menghabiskan waktu tanpa sengaja.

Focus memakai posisi kamera tetap menghadap meja atau laptop. Transisi singkat dapat diganti perpindahan langsung melalui pengaturan Reduce Motion. Saat keluar, kamera kembali ke posisi aman sebelumnya. Cursor terkunci hanya dalam Free Roam; Pause dan seluruh panel fokus melepasnya.

Fokus keyboard terlihat melalui garis tebal dan label, mengikuti urutan baca, serta kembali ke tombol pemanggil setelah dialog ditutup. Panel modal memblokir input dunia dan panel di belakangnya. Tab tetap menjadi tombol Catatan, sehingga navigasi kontrol memakai tombol panah; saat konfirmasi modal terbuka, Tab tidak membuka overlay tambahan. Memasang kartu di panel dapat dilakukan dengan memilih kartu dan tujuan melalui tombol panah serta Enter.

### 3.10 Sound Consideration

Kamar memiliki empat lapisan utama: kipas, aktivitas jauh di lorong, lalu lintas redup, dan suara benda dekat pemain. Titik sumber yang tetap membantu pemain mengenali kapan suasana berubah. Retakan pertama menghilangkan suara lorong, Retakan kedua memindahkan bunyi kunci ke arah pintu 07, dan Retakan ketiga menahan suara kipas sampai kartu dipasang.

Suara tidak menjadi satu-satunya petunjuk. Bunyi kunci memiliki teks arah opsional dan nomor pintu tetap terlihat. Notifikasi menggunakan bunyi pendek dengan volume seragam, bukan lonjakan keras. Tidak ada scream, suara ancaman dari belakang secara acak, atau jumpscare audio.

Master, Ambience, dan Effects memiliki slider terpisah. Semua percakapan tampil sebagai teks. Isyarat penting memiliki caption, misalnya Kunci bergeser di dekat pintu 07. Voice acting penuh tidak masuk cakupan. Musik hanya berupa satu tema singkat di menu dan satu variasi penutup; lisensi seluruh audio harus dicatat pada Credits.

### 3.11 Player Progression and Scoring System

Progres pemain berupa bertambahnya bukti, pemahaman tentang tawaran, dan keputusan yang sudah diambil. Tidak ada level karakter, pohon kemampuan, pengalaman, ranking, achievement, atau leaderboard.

Nilai interview 0-4 hanya membantu resolusi internal. Hasil tidak ditampilkan sebagai nilai kualitas manusia. Setelah interview, pemain menerima penjelasan berbasis bukti. Setelah ending, ringkasan menampilkan bukti yang dipakai, syarat yang telah diperiksa, dan keputusan kontrak.

Ringkasan tidak memberi medali untuk selalu produktif atau menjadikan pulang sebagai hasil bernilai rendah. Menyelesaikan verifikasi memperkaya alasan yang bisa dibaca, tetapi bukan syarat tersembunyi untuk ending kerja.

### 3.12 Win-Lose Conditions and Ending Priority

Keputusan dikunci hanya setelah sesi malam hari ketujuh selesai dan pemain memilih Selesaikan keputusan pada panel final. Panel menampilkan pilihan sementara dan meminta satu konfirmasi. Pemain dapat kembali membaca tawaran tanpa menghabiskan sesi.

Pada panel yang sama, pilihan A/B meminta satu catatan batas yang ingin dikonfirmasi: pendampingan serta ruang lingkup tugas, atau persetujuan serta pencatatan lembur. Keduanya sah dan dicatat sebagai `decisionNote` untuk pesan konfirmasi kerja. Catatan tidak mengubah kontrak, peluang diterima, atau jumlah ending. Pilihan Pulang meminta satu langkah berikutnya: menata bukti proyek atau menyusun jadwal lamaran yang terbatas. Jika pilihan tawaran diganti, catatan yang tidak sesuai direset dan dipilih kembali sebelum finalisasi. Pada C, catatan mengikuti pengakuan atas syarat berisiko pada konfirmasi tambahan.

![Papan Kontrak dan Form Verifikasi Keputusan Final](visuals/10-map-kontrak-keputusan-final.webp)

| Ending | Kondisi final | Konsekuensi cerita |
| --- | --- | --- |
| Mulai dengan Batas | `offerChoice` A atau B dan tawaran tersebut telah diperoleh | Raka mengonfirmasi jadwal mulai serta batas tugas. Variasi A/B mengubah surat dan beberapa kalimat epilog, bukan membuat ending tambahan |
| Pulang, Bukan Selesai | `offerChoice` Tidak ada | Raka menggunakan tiket, membawa portofolio yang sudah disiapkan, dan membuat rencana lanjutan bersama keluarga |
| Kontrak yang Mengunci | `offerChoice` C | Raka menyetujui syarat yang menyerahkan kendali dokumennya. Epilog menunjukkan pembatasan komunikasi dan kesulitan menarik diri, tanpa kekerasan grafis atau menyalahkan korban |

![Visualisasi Tiga Ending Utama SELA](visuals/11-tiga-ending-sela.webp)

Memilih C menampilkan peringatan khusus tentang dokumen dan pembatasan pengunduran diri, kemudian meminta konfirmasi tambahan. Pilihan itu masih bisa ditarik sampai panel final dikunci. Game tidak menjalankan transfer uang atau pengiriman dokumen sungguhan.

Satu variabel `offerChoice` mencegah dua kontrak diterima bersamaan. Jika tidak ada pilihan sah, hasil selalu Pulang, Bukan Selesai. Tidak ada ending yang dipicu Tekanan, kekurangan Tenaga, puzzle yang dilewati, atau jumlah penolakan. Ending Kontrak yang Mengunci adalah hasil berisiko, bukan rahasia yang diberi hadiah.

Masa sewa selesai pada pagi hari 8, bersamaan dengan jadwal bus pulang. A dan B memerlukan kehadiran di kota kos mulai hari 9. Sejak hari 1, pesan Dita menawarkan tempat menginap sementara selama satu bulan jika Raka memilih bekerja di kota; keluarga menyepakati bantuan kebutuhan dasar selama masa transisi. Kedua surat resmi mencantumkan pembayaran pertama sebelum masa tinggal itu berakhir. Pada ending kerja, Raka mengonfirmasi tempat tinggal sementara, tanggal mulai, dan batas yang dipilih. Tiket tidak dipakai. Kesepakatan ini disampaikan melalui teks tanpa area rumah Dita, biaya harian, atau sistem uang baru.

Pada ending pulang, Raka menggunakan bus pagi hari 8 dan membawa bukti yang benar-benar sudah selesai. Jika baru E0 yang ada, langkah berikutnya menyebut menyiapkan E1; jika E1/E2 sudah ada, langkahnya menyebut menata bukti atau menjadwalkan lamaran berikutnya. Pada C, epilog mengikuti instruksi penempatan admin dan akibat penyerahan kendali dokumen. Penutup menyebut informasi atau bantuan yang masih dapat dicari tanpa menggambarkan konsekuensi sebagai hukuman atas nilai diri Raka.

Layar akhir mengingatkan bahwa pengalaman game bukan diagnosis atau nasihat hukum. Halaman bantuan menyarankan dukungan orang tepercaya, konselor kampus, layanan kesehatan setempat, dan kanal resmi ketenagakerjaan. Nomor layanan tidak dicantumkan tanpa pemeriksaan wilayah serta ketersediaannya sebelum rilis.

![Pilihan Kontrak, Konfirmasi Akhir, dan Refleksi](visuals/21-contract-decision.jpg)

### 3.13 Screen Flow and Save System

```mermaid
flowchart LR
    M[Main Menu] --> N[Mulai baru]
    M --> L[Lanjutkan]
    M --> O[Pengaturan]
    M --> C[Credits dan sumber]
    N --> W[Informasi konten dan pengaturan awal]
    W --> R[Kamar 3D]
    L --> R
    R <--> F[Focus: meja atau laptop]
    R <--> J[Catatan]
    R <--> P[Pause]
    R --> T[Retakan Ruang]
    T --> R
    R --> E[Keputusan final]
    E --> K[Epilog dan refleksi]
    K --> M
    P --> M
```

Main Menu menyediakan Mulai baru, Lanjutkan, Pengaturan, Credits dan Sumber, serta Keluar. Pause menyediakan Lanjutkan, Pengaturan, Ulangi dari checkpoint, Kembali ke menu, dan Keluar. Saat Retakan berlangsung, Pause juga menyediakan Petunjuk dan Selesaikan adegan.

Penyimpanan terdiri atas satu autosave berjalan dan satu checkpoint awal hari. Autosave dibuat pada awal permainan, setiap hasil aktivitas beserta peristiwanya selesai, setelah panel draf CV atau penandaan verifikasi ditutup pada keadaan stabil, setiap perubahan pilihan tawaran atau catatan keputusan, serta sebelum finalisasi ending. Tanda pesan dibaca ikut disimpan ketika panel Pesan ditutup pada keadaan stabil. Checkpoint awal hari dibuat setelah pemulihan tidur dan reset flag, sebelum aktivitas pagi. Saat pekerjaan E1/E2, interview, hasil aktivitas, Jeda, atau Retakan masih berlangsung, file stabil sebelumnya dipertahankan. Keluar dari game mengulang seluruh aktivitas dan peristiwa yang belum mencapai checkpoint tersebut. UI menyebut hari serta sesi yang akan diulang.

Checkpoint awal hari memungkinkan mencoba ulang satu hari, dengan konfirmasi bahwa progres sesudahnya diganti. Mulai baru meminta konfirmasi sebelum menimpa progres. Tidak ada grid save, thumbnail, akun, atau cloud save.

Data menyimpan hari, sesi, `sessionResolved`, kondisi, himpunan bukti, draf CV beserta kartu terpasang, snapshot tiap lamaran, jadwal, koreksi klaim, jawaban dan hasil interview, penandaan serta hasil verifikasi, pesan yang dibaca, pilihan tawaran, `decisionNote`, flag Retakan, flag harian, flag variasi aktivitas yang sudah dibaca, serta versi format. Posisi scroll, sorotan tombol, panel sementara, dan benda puzzle yang sedang dibawa tidak disimpan; Lanjutkan membuka keadaan stabil dengan kursor serta kamera yang sesuai. Memuat malam dengan `sessionResolved` Benar tetap menonaktifkan aktivitas satu sesi.

File ditulis ke file sementara lalu menggantikan file utama setelah penulisan berhasil. Jika autosave gagal dibaca, game menawarkan checkpoint awal hari; jika keduanya tidak terbaca, game menjelaskan kegagalan dan tidak berpura-pura berhasil melanjutkan. Pengaturan disimpan terpisah. Setelah epilog, autosave sebelum finalisasi dipertahankan. Main Menu menjelaskan bahwa Lanjutkan kembali ke keputusan malam hari 7 sehingga pemain dapat meninjau pilihan atau melihat penutup lain. Checkpoint awal hari tetap hari 7; tidak ada hari 8 yang dapat dimainkan atau slot tamat tambahan.

![Menu, Checkpoint, dan Status Simpanan](visuals/22-menu-save-states.jpg)

### 3.14 Accessibility and Content Settings

Ukuran teks memiliki pilihan 100%, 125%, dan 150%, dengan area scroll yang tetap bisa dipakai. Target tampilan minimum 1280 × 720. Semua pilihan penting dapat dibaca kembali melalui Catatan. Teks tidak berjalan otomatis dan tidak dibatasi waktu.

Reduce Motion menonaktifkan gerak transisi kamera. Intensitas atmosfer Rendah menghilangkan pergeseran bayangan dan gema tambahan, tetapi tetap memperlihatkan geometri puzzle. Tidak ada kedipan cepat, chromatic aberration, efek blur, atau informasi yang bergantung pada warna saja. Label teks, bentuk penjepit, nomor pintu, dan garis penghubung membedakan objek.

Menu pengaturan awal muncul sebelum gameplay. Pause tersedia saat eksplorasi, puzzle, dan interview. Permainan tetap dapat diselesaikan dengan audio dimatikan dan bantuan puzzle digunakan seluruhnya.

Menu, pilihan dialog, kartu, dan konfirmasi dapat dioperasikan dengan tombol panah serta Enter/Spasi. Fokus selalu terlihat dan tidak berpindah ke panel di belakang modal. Teks diperiksa pada 720p untuk semua ukuran, termasuk panjang nama perusahaan dan peringatan snapshot. Informasi penting tidak dicetak permanen pada gambar konsep; isi akhir memakai elemen teks yang dapat diperbesar.

![Pengaturan, Keterbacaan, Credits, dan Bantuan](visuals/23-settings-reading.jpg)

### 3.15 Production Plan and Acceptance Criteria

Cakupan tetap mencakup satu ruang utama, satu lorong pendek, tiga variasi Retakan, dua persiapan bukti, dua interview, tiga tawaran, dan tiga ending. Tidak ada multiplayer, open world, NPC berjalan, inventory umum, simulasi ekonomi, minigame coding, generasi prosedural, atau shader khusus.

| Role | Tanggung jawab utama | Titik integrasi |
| --- | --- | --- |
| Anggota 1: Gameplay | Gerak first-person, interaksi, sesi, kondisi, save/load | Menyediakan state aktivitas dan event yang dipakai UI serta cerita |
| Anggota 2: Narrative and UI | Naskah, laptop, CV, interview, tawaran, ending | Memasukkan konten ke data terstruktur; tidak menggandakan logika sesi |
| Anggota 3: 3D and Audio | Model modular, tekstur, pencahayaan, tiga Retakan, audio | Menghubungkan objek ke identitas interaksi dan pemicu peristiwa |

| Week | Hasil kerja yang harus dapat dimainkan |
| --- | --- |
| 1 | Blockout kamar, gerak, focus, satu aktivitas, pergantian sesi, tidur, dan checkpoint dasar; inventaris naskah serta perangkat uji ditetapkan |
| 2 | Satu rangkaian utuh: persiapan E1, snapshot lamaran A, interview A, Retakan pertama, dan save/load; dimainkan orang di luar pembuat rangkaian |
| 3 | Seluruh aktivitas, E2, interview B, pemeriksaan tiga perusahaan, Retakan kedua/ketiga, pesan akhir minggu, dan tiga ending; seluruh tujuh hari dapat diselesaikan dengan aset sederhana |
| 4 | Playtest jalur lengkap, perbaikan aturan serta bug, keterbacaan 720p, pemeriksaan performa build Windows, dan audit lisensi |
| 5-6 jika tersedia | Penyelesaian aset, audio, dan perbaikan hasil playtest tanpa menambah sistem atau cabang cerita |

Jika waktu hanya empat minggu, seluruh jalur tetap selesai menggunakan aset sederhana. Pengayaan visual tidak menjadi syarat agar mekanisme bekerja.

Pada minggu pertama, setiap anggota mencatat jam kerja yang tersedia. Pembagian tugas diperiksa terhadap kapasitas tersebut. Pengurangan dimulai dari detail tekstur, variasi dekorasi, gerak transisi, dan ekspresi potret tambahan; berikutnya panjang teks pengulangan. Informasi syarat, bantuan puzzle, aksesibilitas dasar, serta semua jalur ending tetap tersedia.

Inventaris naskah memakai batas awal berikut. Anggota Narrative and UI menyimpan satu versi teks yang dipakai laptop, Catatan, dan panel akhir. Anggota Gameplay menangani pemicu serta flag; anggota 3D and Audio menghubungkan penyajian benda dan suara. Hitungan ini merupakan anggaran produksi yang diperiksa sebelum penulisan lengkap.

| Kelompok konten | Jumlah dan batas awal | Isi wajib |
| --- | --- | --- |
| Pesan harian | 7 pesan dasar, maksimal 60 kata per pesan; hari 5-7 masing-masing memiliki 3 sisipan konteks maksimal 25 kata | Perkembangan hari, pengingat kesempatan, dan persiapan jawaban terakhir |
| Percakapan Dita | 7 percakapan pertama, 60-100 kata; 3 sisipan hasil lamaran maksimal 25 kata; 1 respons pengulangan maksimal 20 kata | Perspektif pribadi Dita, tanggapan riwayat, dan batas bantuan yang bisa diberikan |
| Pengamatan pemulihan | 3 teks duduk maksimal 35 kata; 2 teks benda saat merapikan maksimal 25 kata; 1 teks singkat istirahat | Detail Raka dan kamar; pengulangan tidak menambah informasi wajib |
| Bukti dan CV | 3 keterangan kartu maksimal 80 kata; 2 pilihan persiapan beserta umpan balik maksimal 100 kata per aktivitas; 2 pasang klaim | Peran dalam proyek, hasil kerja, dan batas pembuktian |
| Lowongan dan verifikasi | 3 dokumen tawaran maksimal 180 kata; 3 ringkasan klarifikasi maksimal 120 kata | Empat aspek syarat, pertanyaan yang dijawab, dan ketidakpastian yang tersisa |
| Interview dan hasil | 2 interview, masing-masing pembuka serta 2 pertanyaan dengan 3 respons; 160-220 kata per interview; 2 template tawaran, 3 alasan penolakan, dan 1 template terlewat maksimal 60 kata masing-masing | Snapshot, koreksi klaim, penjelasan hasil, dan jadwal |
| Retakan | 3 adegan; masing-masing 2 pembuka maksimal 30 kata, 1 umpan balik solusi, 1 petunjuk bertahap, dan 1 respons tujuan salah maksimal 35 kata | Fakta pengantar, alasan solusi, dan bantuan |
| Penutup dan halaman baca | 4 teks epilog A/B/pulang/C, 120-180 kata; 3 sisipan progres bukti maksimal 30 kata; 1 halaman refleksi, 1 informasi konten, 1 bantuan, dan 1 Credits/Sumber | Pilihan final, langkah berikutnya, informasi dukungan, dan sumber yang benar-benar dipakai |
| UI dan tutorial | 1 rangkaian pengenalan bertahap, 9 panel aktivitas yang berbagi komponen, serta template pengingat, simpanan, dan konfirmasi | Biaya, keadaan tidak tersedia, kontrol, dan akibat tindakan |

Playtest awal memakai rangkaian minggu 2 untuk memeriksa biaya sesi, pengiriman, dan pembacaan bukti. Playtest minggu 4 mencatat durasi per hari, waktu persiapan dianggap selesai, aktivitas pemulihan yang dipilih beserta alasan, pemahaman snapshot, kegunaan klarifikasi, dan tanggapan terhadap Retakan. Jalur uji mencakup tawaran diperoleh sejak hari 4, tidak ada tawaran resmi, hanya pemulihan, interview terlewat, dan pemakaian seluruh bantuan puzzle. Jika hari 5-7 terasa seperti menunggu, perbaikan diarahkan ke urutan serta panjang pesan dan perbandingan pilihan, tanpa menambah pekerjaan wajib. Durasi 45-60 menit tidak dipenuhi dengan menahan teks atau memperlambat jalan.

Evaluasi pembelajaran memakai contoh tawaran terpisah pada bagian 1.4. Catat alasan peserta sebelum memberi penjelasan jawaban; hasilnya dipakai untuk memperbaiki keterbacaan syarat, tanpa klaim efektivitas klinis. Perangkat uji mencantumkan CPU, GPU, RAM, resolusi, dan hasil frame time dari build Windows.

Kriteria penerimaan build:

- Satu permainan dapat berjalan dari menu sampai keputusan malam ketujuh tanpa state buntu, termasuk jika pemain hanya memilih pemulihan.
- Bukti, jadwal interview, koreksi klaim, dan hasil mengikuti aturan deterministik yang tercantum.
- Semua Retakan dapat diselesaikan tanpa audio dan melalui bantuan, dengan hasil cerita yang setara.
- Tekanan 6 dan Tenaga 0 tidak mematikan gerak atau semua pilihan pemulihan.
- Memuat checkpoint tidak menggandakan hasil, pesan, biaya, atau jadwal.
- Memuat draf CV dan malam yang telah selesai mempertahankan kartu, klaim, serta larangan membeli sesi tambahan.
- Keluar saat persiapan bukti, interview, Jeda, atau Retakan mengulang dari checkpoint yang disebutkan tanpa membawa hasil setengah jadi.
- Lanjutkan setelah epilog kembali ke keputusan malam hari 7 dengan pilihan sah dan catatan yang sesuai.
- Semua menu, pilihan dialog, dan kartu dapat dipilih melalui keyboard dengan fokus terlihat pada ukuran teks 150%.
- Tiga ending mengikuti pilihan kontrak, bukan stat kesehatan.
- Build akhir diuji pada perangkat tim; target awalnya 30 fps stabil pada 720p dengan grafis terintegrasi. Target ini harus dilaporkan bersama spesifikasi dan hasil nyata setelah build tersedia.

## 4. Story and Narrative

### 4.1 Story Synopsis

Raka menyimpan surat kelulusan di bawah tumpukan kertas revisi CV. Delapan bulan setelah wisuda, ia lebih hafal kalimat penolakan daripada hasil proyeknya sendiri. Masa sewa tinggal seminggu. Ibunya sudah membelikan tiket pulang dan meminta Raka memberi kabar menjelang keberangkatan.

Raka menganggap tiket itu sebagai pengakuan bahwa ia gagal. Dita, teman kuliahnya, melihat hal lain: proyek Raka belum hilang hanya karena belum diterima perusahaan. Ketika dua kesempatan kerja muncul bersamaan dengan tawaran yang jauh lebih mudah, Raka harus memutuskan informasi mana yang ia percaya dan klaim apa yang bersedia ia pertanggungjawabkan.

Bagian proyek yang paling Raka ingat adalah memperbaiki formulir setelah seorang teman salah memahami labelnya. Ia menikmati pekerjaan antarmuka yang hasilnya bisa dijelaskan, tetapi belum nyaman menangani server sendiri. Studio Lajur menarik karena dekat dengan minat tersebut. Koperasi Pijar menawarkan tugas lebih terbatas, waktu kerja lebih pendek, dan pendampingan, dengan ketidakpastian kontrak tiga bulan. Perbandingan ini memberi alasan personal untuk kedua pilihan tanpa menempatkannya dalam urutan terbaik.

Kamar mengikuti cara Raka menilai dirinya. Kertas penolakan menutupi jalan, pintu hanya menawarkan label diterima atau terlambat, dan kursi kosong terasa seperti tempat orang lain menunggu jawaban. Benda sederhana mengoreksi keadaan itu: hasil kerja, nomor kamar, dan pesan ibu yang tidak menuntut kabar keberhasilan.

Tidak ada pengungkapan bahwa semua orang bersekongkol atau bahwa Raka menjadi ancaman bagi orang lain. Konflik tetap berpusat pada kesenjangan antara kebutuhan nyata, proses rekrutmen, dan rasa malu yang sulit ia ucapkan.

Dita kadang terlalu cepat menawarkan langkah praktis sebelum memahami bahwa Raka sedang ingin didengar. Ia tetap harus membagi waktu dengan pekerjaannya sendiri dan tidak bisa menjanjikan posisi untuk Raka. Ibu lebih sering membicarakan jadwal dan kebutuhan perjalanan; Raka beberapa kali menafsirkan pertanyaan itu sebagai desakan. Dukungan mereka membantu Raka mengambil langkah, sementara batas sewa, persaingan kerja, dan syarat yang tidak jelas tetap menjadi persoalan nyata.

### 4.2 Plot

#### Act I: Kalimat yang Sudah Dihafal, Days 1-2

Raka bangun, melihat tanggal berakhirnya sewa, lalu membaca pesan ibu tentang tiket. Tutorial tidak berupa daftar instruksi panjang: pemain mengambil kartu proyek, membuka laptop, dan memilih aktivitas pertama. Pesan penolakan lama sudah tersedia di inbox sebagai konteks, bukan hasil lamaran pemain yang dipaksakan gagal.

Pesan awal menyebut bus berangkat pagi hari 8. Tawaran Dita untuk tinggal sementara dan bantuan kebutuhan dasar dari keluarga sudah disampaikan sejak hari 1, sehingga pilihan bekerja di kota memiliki rencana transisi sebelum hasil interview diketahui. Raka tetap merasa perlu membuktikan bahwa ia dapat membiayai hidup sendiri. E0, nomor 07, dan pembicaraan tentang bagian formulir mengenalkan benda serta fakta yang dipakai Retakan.

Pada hari kedua, meja terasa menjauh. Kartu E0 menjadi bukti sederhana bahwa kalimat belum pernah menyelesaikan apa pun tidak cocok dengan apa yang ada di kamar. Menyelesaikan Retakan mengembalikan ruang, tetapi tidak menghapus kesedihan atau langsung memberi pekerjaan.

#### Act II: Tawaran yang Tidak Banyak Bertanya, Days 3-5

Koperasi Pijar membuka kesempatan yang membutuhkan catatan uji. Akses Prima menghubungi Raka dengan pujian umum dan penghasilan besar, lalu menghindari pertanyaan tentang tugas. Dita tidak mengambil keputusan untuk Raka; ia meminta Raka membaca bagian dokumen yang paling ingin ia lewatkan.

Pada Retakan kedua, dua pintu memakai label yang selama ini dipikirkan Raka. Pintu kamarnya sendiri masih bertuliskan 07. Pemain kembali melalui alamat yang bisa diperiksa, bukan melalui label keberhasilan.

Pada hari 5, perhatian beralih dari jumlah lamaran ke isi pilihan. Jika tawaran sudah ada, pesan Dita meminta Raka menunjuk syarat yang paling memengaruhi kesehariannya. Jika belum ada, percakapan membahas apa yang masih dapat dikirim sebelum batas waktu dan apa yang perlu disiapkan setelah pulang. Jawaban klarifikasi yang sudah dibaca tetap dipakai; pemain tidak diminta membeli pemeriksaan yang sama untuk kedua kalinya.

#### Act III: Jawaban yang Bisa Diucapkan, Days 6-7

Pada hari 6, interview terakhir berlangsung jika ada undangan, sementara tawaran yang sudah diperoleh tetap berada di map. Sejak pagi, Ibu mengirim pesan bahwa tiketnya tetap boleh dipakai. Pada Retakan ketiga, kursi tambahan mengisi kamar seolah ada banyak orang menunggu hasil, tetapi tidak ada orang yang duduk di sana. Catatan sesudahnya mengajak Raka membandingkan tugas, waktu kerja, pendampingan, serta hal yang belum pasti dengan kebutuhannya sendiri.

Pada hari 7, Ibu menanyakan jadwal bus atau rencana tinggal yang dipilih Raka. Pemain menyiapkan satu kalimat batas kerja atau langkah setelah pulang pada panel keputusan. Pada malam terakhir, pemain melihat kembali tawaran dan mengunci pilihan. Raka menyampaikan keputusan kepada ibu dengan kalimat yang berubah sesuai ending dan catatan tersebut. Penutup memperlihatkan langkah berikutnya serta konsekuensi syarat tanpa menyatakan seluruh masalah sudah selesai.

### 4.3 Game Story Progression

Peristiwa hari 2, 4, dan 6 berlangsung pada semua jalur. Variasi naskah menggunakan flag yang terbatas: bukti yang selesai, klaim yang dikoreksi, hasil interview, verifikasi, dan pilihan tawaran. Tidak ada nilai hubungan tersembunyi dengan ibu atau Dita.

Perubahan sikap Raka mengikuti tiga tahap. Pada awal minggu ia mencari bukti bahwa dirinya masih mampu. Pada pertengahan minggu ia mulai menanyakan pekerjaan yang sebenarnya akan dijalani. Pada akhir minggu ia berlatih menyebut pilihan serta batasnya kepada orang lain. Mendapat tawaran lebih awal mengubah kekhawatirannya menjadi takut salah memilih; penolakan mengubahnya menjadi pertanyaan tentang persiapan berikutnya. Keduanya tetap dapat berujung pada keputusan yang dijelaskan dengan sadar.

Jika pemain jarang menghubungi Dita, pesan penting tetap masuk tanpa biaya sesi. Aktivitas Hubungi Dita memberi percakapan tambahan dan pemulihan, bukan akses eksklusif pada bantuan. Jika pemain tidak memiliki E1/E2, epilog pulang menyebut rencana merapikan proyek, bukan mengklaim portofolio sudah lengkap.

Jika dua tawaran resmi tersedia, keputusan akhir menampilkan perbedaan tugas dan durasi kontrak. Jika tidak ada tawaran resmi, game tetap menawarkan pulang. Akses Prima tidak diaktifkan sebagai satu-satunya jalan untuk bertahan hidup.

Contoh dialog orisinal:

> Raka: Kalau pulang sekarang, nanti ditanya kerja di mana.
>
> Ibu: Jawab saja masih mencari. Ibu tanya jadwal busnya, Ka.

> Dita: Yang bagian ini kamu kerjakan sendiri?
>
> Raka: Formulirnya. Yang server dibantu Arman.
>
> Dita: Ya, tulis begitu. Orangnya beda, hasil kerjamu tetap ada.

> Dita: Aku tadi langsung bahas lamaran lagi. Kamu sebenarnya mau cerita dulu?
>
> Raka: Iya. Aku sudah baca sarannya, cuma belum sanggup buka laptop lagi.

> Raka: Kalau pilih Lajur, bagian server masih perlu didampingi.
>
> Dita: Tulis itu di konfirmasinya. Biar jelas dari awal siapa yang bantu.

Pada ending kerja, Raka mengirim konfirmasi tugas sebelum mengabarkan tanggal mulai. Pada ending pulang, ia menyimpan kartu proyek di dalam tas, bukan meninggalkannya di tempat sampah. Pada ending berisiko, epilog menyebut kebutuhan mencari dukungan dan akses bantuan, tanpa menyimpulkan bahwa orang yang tertipu pantas menerima akibatnya.

### 4.4 Storyboards

Seluruh adegan memakai sudut pandang pemain atau tampilan benda dekat. Tidak diperlukan animasi tubuh lengkap atau cutscene sinematik terpisah. Visualisasi tersedia di [storyboard](sketchs/05-storyboard.svg).

| Shot | Framing dan perkiraan durasi | Gerak atau aksi | Audio dan teks | Fungsi |
| --- | --- | --- | --- | --- |
| 01 | Pandangan dari sisi kasur ke meja, 8-12 detik | Layar memudar masuk; kendali segera diberikan | Kipas dan pesan ibu: Tiketnya sudah Ibu kirim | Memperkenalkan kamar serta pilihan pulang |
| 02 | Close-up map proyek, mengikuti pembacaan pemain | Pemain memilih kartu E0 | Gesekan kertas; keterangan peran Raka dalam proyek | Menetapkan fakta yang dipakai kemudian |
| 03 | Sumbu pandang meja yang memanjang, 10 detik pengantar lalu interaktif | Ruang berganti saat fade; pemain mencari kartu foto | Suara lorong menghilang; satu kalimat penolakan tampil pada kertas | Retakan pertama dan pengenalan jangkar |
| 04 | Laptop dengan draf Akses Prima, mengikuti pembacaan pemain | Kursor menandai syarat dokumen | Notifikasi pendek; tidak ada timer | Menempatkan risiko sebelum persetujuan |
| 05 | Lorong tiga pintu, 5 detik pengantar lalu interaktif | Pemain mengambil kunci 07 dan memilih pintu | Bunyi kunci dengan caption opsional | Memisahkan identitas dari label hasil |
| 06 | Kursi dekat jendela, 5 detik pengantar lalu interaktif | Pesan ibu dipasang; kursi tambahan hilang saat fade | Kipas kembali terdengar | Memberi ruang pada pilihan pulang |
| 07 | Meja final, mengikuti keputusan pemain | Satu surat dipilih atau semua ditutup; satu catatan batas atau rencana dipilih | Raka menulis pesan konfirmasi sesuai catatan | Mengunci satu keputusan yang dipahami |
| 08 | Komposisi pintu dan tas, 12-18 detik | Variasi surat kerja, tiket, atau map dokumen | Epilog singkat dan halaman refleksi | Menutup cerita sesuai kontrak yang dipilih |

### 4.5 References

- **[R1]** United Nations. *Goal 3: Good Health and Well-Being*, khususnya target 3.4. https://www.un.org/sustainabledevelopment/health/
- **[R2]** United Nations. *Goal 8: Decent Work and Economic Growth*, khususnya target 8.5 dan 8.8 serta penjelasan pekerjaan layak. https://www.un.org/sustainabledevelopment/economic-growth/
- **[R3]** World Health Organization. *Mental health at work*. https://www.who.int/news-room/fact-sheets/detail/mental-health-at-work
- **[R4]** Wouters, P., van Nimwegen, C., van Oostendorp, H., & van der Spek, E. D. (2013). *A meta-analysis of the cognitive and motivational effects of serious games*. Journal of Educational Psychology, 105(2), 249-265. https://doi.org/10.1037/a0031311. Abstrak: https://eric.ed.gov/?id=EJ1008015

Sumber digunakan untuk dasar tema dan pendekatan pembelajaran. Sumber tersebut tidak memvalidasi angka kondisi, kontrak fiktif, atau efektivitas game ini.

Referensi kreatif digunakan untuk mempelajari penyampaian suasana dan cerita:

| Reference | Aspek yang dipelajari | Batas penerapan |
| --- | --- | --- |
| Seri Pikabuu | Keseharian Indonesia sebagai sumber ketegangan sosial | Tidak mengambil tokoh, alur tragedi, atau rangkaian peristiwanya |
| MiSide | Keterbacaan ruang 3D bergaya anime dan perubahan rasa aman | Tidak memakai pendamping serupa, desain karakter, tata ruang, atau sekuens pengejarannya |
| Omori | Kontras antara ruang sehari-hari dan ruang subjektif | Tidak memakai dunia, simbol, sistem pertarungan, atau pengungkapan ceritanya |
| Papers, Please | Konsekuensi manusia di balik keputusan administratif | Tidak memakai pemeriksaan dokumen bertimer, stempel, antrean, atau aturan perbatasan |
| Needy Streamer Overload | Tekanan yang muncul melalui hubungan dan layar komputer | Tidak memakai simulasi pengikut, statistik popularitas, atau pola percabangannya |
| The NOexistenceN of you AND me | Dialog yang membuat tokoh mempertanyakan penilaian dirinya | Tidak mengambil dialog, tokoh, atau struktur metanaratifnya |
| Iron Lung | Ruang sempit yang memiliki identitas suara kuat | Tidak memakai navigasi buta, kendaraan, sonar, atau mekanisme fotografi |


## 5. Game Environment and Objects

### 5.1 Game Levels and Progression

Lingkungan dibangun sebagai satu scene permainan dengan kelompok objek untuk kondisi normal dan tiga Retakan. Menu memakai scene terpisah yang ringan. Setiap Retakan memanfaatkan mesh kamar atau modul lorong yang sama, dengan salinan layout serta collider yang sudah disusun.

| Area | Ukuran target | Progres dan batas |
| --- | --- | --- |
| Kamar 07 | Interior 3 × 4 meter; tinggi 2,8 meter | Area utama selama tujuh hari; jendela dan pintu menjadi orientasi tetap |
| Lorong kos | Panjang 6 meter, lebar 1,5 meter | Dalam keadaan normal pemain hanya dapat mencapai bangku dan ujung tertutup; akses keluar kota tidak tersedia |
| Retakan 1 | Salinan kamar dengan panjang 7 meter | Jalur tunggal dari kartu ke meja; kembali ke layout normal setelah solusi |
| Retakan 2 | Modul lorong yang sama dengan tiga muka pintu | Pintu 07 menjadi satu-satunya tujuan sah |
| Retakan 3 | Kamar normal dengan kursi tambahan | Pemain tetap memiliki jalur minimal 0,8 meter ke jendela |

Setiap transisi mengembalikan pemain ke titik aman yang ditentukan, bukan koordinat lama yang mungkin tertutup mesh. Tidak ada loading ke kota, kendaraan, atau interior perusahaan.

### 5.2 Game World and Environment

Kos berada di kawasan hunian dekat kampus. Detail lokal berupa lantai keramik kusam, kabel sambungan berlabel, sandal di dekat pintu, gantungan kunci nomor kamar, dan tirai pendek hasil jahitan ulang. Barang memiliki alasan berada di tempatnya; kamar tidak dipenuhi sampah untuk menandai kondisi psikologis.

Meja berada di dinding utara menghadap papan bukti. Kasur menempel pada sisi barat. Pintu berada di selatan, sedangkan jendela dan kursi berada di timur. Komposisi ini membuat laptop, kasur, pintu, dan kursi terlihat sebagai empat tujuan berbeda. Tidak ada kamar mandi yang dapat dimasuki karena tidak mendukung aktivitas inti.

![Kamar Kos Raka - Tampilan First-Person](visuals/01-kamar-kos-firstperson.webp)

Tiga waktu hari memakai perubahan warna ambient dan satu arah cahaya yang sederhana. Lampu meja tidak menjadi sumber cahaya realtime tambahan. Kota di luar jendela berupa bidang 3D sederhana dan siluet bangunan, bukan area yang bisa dijelajahi.

![Siklus Pencahayaan Pagi, Siang, dan Malam](visuals/02-pencahayaan-pagi-siang-malam.webp)

Art direction bernama *Cat Poster Kos*: bentuk 3D low-poly yang rapi, bidang warna matte, dan arsiran bayangan yang dilukis pada tekstur. Wajah potret memakai penyederhanaan anime dengan ciri pakaian serta ekspresi keseharian. Panduan warna, bentuk, aset, dan implementasi visual terdapat di [DESIGN.MD](DESIGN.MD).

### 5.3 Characters

| Character | Peran dan sifat | Bentuk aset | Batas penulisan |
| --- | --- | --- | --- |
| Raka, 23 tahun | Lulusan Informatika; senang memperjelas antarmuka, belum mandiri menangani server, menghindari percakapan tentang penolakan | Kamera first-person; identitas melalui benda dan dialog; tidak memerlukan model tubuh | Tidak dijadikan wakil semua pencari kerja atau diagnosis tertentu |
| Dita, 23 tahun | Teman kuliah yang juga bekerja; cepat memberi saran praktis, belajar mendengarkan, menawarkan tempat tinggal transisi dengan batas satu bulan | Satu potret dengan ekspresi netral dan tersenyum tipis; pesan teks | Tidak menjadi terapis, menjanjikan pekerjaan, atau selalu memiliki jawaban benar |
| Ibu | Orang tua yang menanyakan jadwal perjalanan, menyiapkan tempat pulang, dan menyepakati bantuan kebutuhan dasar sementara | Teks dan foto kecil bergaya ilustrasi | Tidak dipakai sebagai ancaman rasa bersalah atau hukuman atas kegagalan |
| Nara, Studio Lajur | Perekrut yang meminta penjelasan lingkup kerja | Potret 2D, tiga ekspresi | Menilai bukti, bukan kebersihan kamar atau kondisi mental |
| Bagas, Koperasi Pijar | Koordinator yang membutuhkan dokumentasi pengujian | Potret 2D, tiga ekspresi | Tidak menjanjikan perpanjangan kontrak otomatis |
| Admin Akses Prima | Kontak yang menghindari pertanyaan spesifik | Avatar logo geometris dan teks | Risiko ditunjukkan melalui perilaku serta kontrak, bukan bentuk wajah atau kebangsaan |

![Desain Karakter dan Potret 2D Sesi Interview](visuals/06-karakter-potret-interview.webp)

Tidak ada monster yang mewakili depresi, musuh ber-NPC AI, atau karakter 3D beranimasi penuh. Kehadiran manusia disampaikan lewat pesan, potret, dan cara benda ditempatkan.

### 5.4 Objects

| Object | Interaksi dan state | Kebutuhan aset |
| --- | --- | --- |
| Meja dan papan bukti | Focus; menyusun E0/E1/E2; dua keadaan rapi | Satu meja, papan, tiga mesh kartu yang berbagi material |
| Laptop | Membuka tiga tab dan interview | Satu model statis; layar berupa Canvas sederhana saat focus |
| Ponsel | Membaca pesan gratis atau memulai aktivitas Hubungi Dita | Satu model; tidak memerlukan simulasi perangkat |
| Kasur | Aktivitas istirahat; tidur penutup hari; membuka keputusan pada malam terakhir | Satu dipan, kasur, dan bantal |
| Kursi jendela | Aktivitas tanpa layar dan jangkar Retakan ketiga | Satu model yang diulang untuk variasi Retakan |
| Kalender tujuh hari | Menunjukkan hari, sesi, dan tanggal akhir sewa | Satu bidang tekstur dengan label teks yang diperbarui |
| Map tawaran | Membandingkan A/B/C dan memilih surat | Satu mesh map; isi ditampilkan melalui UI |
| Kartu proyek E0 | Dapat dibaca; salinan puzzle menjadi jangkar pertama | Satu gambar proyek fiktif |
| Kunci 07 | Dapat diperiksa; salinan puzzle dipasang pada pintu 07 | Satu model low-poly dan label besar |
| Kartu pesan ibu | Terlihat sejak hari 6; salinan puzzle menjadi jangkar ketiga | Kartu kertas dan teks yang terbaca |
| Pintu dan nomor kamar | Buka ke lorong; variasi muka untuk Retakan kedua | Satu modul pintu dengan tiga varian tulisan |
| Tas dan tiket | Menegaskan pilihan pulang; komposisi ending | Satu tas dan satu kartu tiket fiktif |
| Kipas, rak, tirai, sandal, kabel, buku | Pembentuk orientasi dan suasana; umumnya tidak interaktif | Bentuk sederhana, material bersama |

Objek interaktif memakai siluet atau penjepit yang konsisten, prompt, dan nama. Benda dekoratif tidak memberikan prompt palsu. Catatan tidak meminta pemain membaca tulisan kecil langsung dari tekstur dunia; memilihnya membuka salinan teks yang dapat diperbesar.

### 5.5 Graphics Scope and Asset Budget

Bagian grafika yang dikerjakan mencakup pemodelan modular, transformasi objek, proyeksi perspektif, UV dan tekstur, pencahayaan sederhana, material, raycast interaksi, serta komposisi kamera. Perubahan ruang menunjukkan penerapan transformasi dan pergantian kelompok objek, tanpa memerlukan teknologi rendering khusus.

Target aset adalah sekitar 25 mesh unik, satu atlas lingkungan 1024 × 1024, satu atlas properti 1024 × 1024, dan potret 2D terpisah. Geometri yang terlihat ditargetkan di bawah 80.000 triangle. Angka tersebut merupakan batas kerja awal, bukan jaminan performa.

Mayoritas pengalaman berlangsung dengan berjalan dan berinteraksi dalam ruang 3D. Komponen 2D dibatasi pada laptop, potret, label, serta teks. Laptop tidak membuka permainan 2D lain; pemain selalu kembali ke ruang fisik sebagai pusat aktivitas.

Visualisasi pendamping:

- [01: Room and Layout](sketchs/01-room-layout.svg)
- [02: First-Person Room](sketchs/02-first-person.svg)
- [03: Evidence and Laptop](sketchs/03-evidence-laptop.svg)
- [04: Character and Props](sketchs/04-character-props.svg)
- [05: Storyboard](sketchs/05-storyboard.svg)

Sketsa merupakan acuan komposisi dan produksi aset, bukan tangkapan layar build Unity.

Ilustrasi di visuals/ tetap digunakan untuk arah warna, ruang, dan susunan panel. Angka, teks, pilihan, dan syarat implementasi mengikuti GDD serta DESIGN.MD. Perbedaan kecil pada tulisan ilustrasi dicatat saat implementasi; aset gambar tidak perlu dibuat ulang untuk setiap revisi aturan. Teks penting dibuat sebagai elemen UI yang dapat diperbesar.
