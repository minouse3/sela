# SELA
## Art and Visual Design Guide

**Visual direction: Cat Poster Kos**

Ruang 3D dengan permukaan matte, warna seperti cat poster yang sedikit pudar, dan bentuk barang yang menunjukkan pemakaian. Pengaruh anime terlihat pada penyederhanaan wajah, pengelompokan rambut, dan ekspresi. Lingkungannya tetap kamar kos Indonesia yang dapat dikenali dari susunan benda, bukan kamar fantasi dengan dekorasi berlebihan.

Panduan ini digunakan bersama [Game Design Document](GDD.md). Gambar di [visuals/](visuals/README.md) menjadi acuan suasana dan desain layar. File 00-11 merupakan ilustrasi konsep; lembar 12-25 melengkapi state dan alur interaksi mekanik. Seluruh gambar merupakan acuan desain, bukan hasil render atau bukti implementasi Unity.

## 1. Visual Identity

### 1.1 Core Image

Meja hijau kelabu berada di bawah papan kertas. Satu sudut papan kosong, tetapi bekas selotipnya masih terlihat. Tirai berwarna tanah liat berhenti beberapa sentimeter di atas ambang jendela. Kunci nomor 07 menggantung pada paku yang sedikit miring. Barang-barang itu tidak mahal atau baru, tetapi masih digunakan dan dirawat.

Kamar dikenali dari tiga keputusan bentuk:

- Sudut kanan atas kartu bukti terlipat kecil. Lipatan ini muncul juga pada panel Catatan dan map tawaran.
- Dua jahitan pendek menandai kain yang pernah diperbaiki, terutama tirai dan tas. Jahitan menjadi detail tekstur, bukan geometri kecil.
- Nomor 07 memakai pelat persegi panjang dengan satu ujung terpotong. Siluet itu diulang pada gantungan kunci dan penanda tujuan puzzle.

Motif tidak disebarkan ke setiap benda. Nomor kamar hanya berada pada benda yang berhubungan dengan kamar; jahitan hanya berada pada kain. Hubungan seperti ini membuat detail terasa dibuat untuk tempat tertentu.

### 1.2 What to Avoid

Hindari kamar simetris seperti katalog, neon ungu-biru, material plastik mengilap pada semua benda, mata anime terlalu besar, garis tepi hitam tebal pada seluruh objek, serta simbol mata atau senyum menyeramkan yang muncul tanpa alasan cerita.

Kesan personal datang dari detail yang konsisten: label kabel bertuliskan layar, kursi yang sedikit berbeda warna dari meja, dan bekas lakban di papan. Jangan menggantinya dengan noise acak, tulisan palsu yang tidak terbaca, atau puluhan aksesori yang tidak berfungsi.

Logo judul cukup berupa tulisan SELA dan ruang kosong kecil di antara huruf E dan L. Tidak diperlukan lambang perusahaan, maskot, atau sistem identitas tambahan.

## 2. Color and Value

| Color Name | Hex | Pemakaian |
| --- | --- | --- |
| Kertas beras | `#E9DDC4` | Kertas, dinding hangat, bidang terang |
| Hijau meja | `#718579` | Meja, map, bagian furnitur |
| Tanah tirai | `#AE7462` | Tirai dan aksen kain |
| Biru tinta | `#596C80` | Antarmuka laptop, pakaian, informasi aktif |
| Arang hijau | `#303E38` | Teks, detail gelap, rambut |
| Kuning kusam | `#C2A262` | Kunci, tanda yang sedang dipilih |

Enam warna tersebut merupakan keluarga warna, bukan batas jumlah swatch mutlak. Variasi terang dan gelap dibuat dari keluarga yang sama. Warna kulit memakai turunan hangat dari kertas dan tanah, bukan warna putih polos.

Perbandingan luas bidang ditargetkan sekitar 60% warna terang netral, 30% hijau atau biru kelabu, dan 10% aksen hangat. Proporsi ini adalah panduan komposisi, bukan aturan shader.

Pada Tekanan 3-4, bidang bayangan bergeser ke biru kelabu dan beberapa permukaan terasa lebih dingin sebagai peringatan awal mendekati ambang Retakan (`5–6`). Jika Tekanan bertahan pada 6 setelah Jeda cadangan harian, gunakan perlakuan visual yang sama. Warna benda penting tidak berubah menjadi merah. Pada Retakan, perubahan struktur lebih dominan daripada perubahan palet. Merah hanya dipakai untuk peringatan kontrak dan selalu disertai teks Berisiko.

Semua teks menggunakan warna arang pada bidang kertas atau teks terang pada bidang biru tinta. Jangan meletakkan teks kecil di atas tekstur ramai. Kontras dan pembesaran 150% harus diperiksa pada resolusi 1280 × 720.

## 3. Shape Language and Modeling

### 3.1 Environment

Gunakan balok, bidang, dan bevel kecil pada benda yang sering terlihat dekat. Meja memiliki kaki kotak, tepi depan sedikit aus, dan satu laci tertutup. Kasur berupa volume sederhana dengan dua lipatan besar. Tirai cukup berupa beberapa bidang membentuk lipatan, tanpa cloth simulation.

Dinding tetap tegak dan ukuran pintu masuk akal pada kondisi normal. Ketidakteraturan hanya diterapkan pada benda: kemiringan buku, tepi kain, atau bentuk bantal. Jangan membuat semua garis miring demi terlihat digambar tangan; ruang yang stabil diperlukan agar Retakan terasa berbeda.

| Asset Group | Target Triangle per Asset | Modeling Rule |
| --- | --- | --- |
| Dinding, lantai, plafon | 2-100 | Modul berukuran tetap, bagian tak terlihat tidak perlu detail |
| Meja dan rak | 250-700 | Siluet jelas; isi rak memakai buku gabungan |
| Laptop | 300-700 | Engsel dan layar cukup terbaca; tombol keyboard menjadi tekstur |
| Kursi | 250-500 | Satu model yang bisa dipakai ulang |
| Kasur dan bantal | 300-800 | Lipatan besar dilukis, bukan subdivisi rapat |
| Pintu | 150-350 | Pelat nomor terpisah; gagang sederhana |
| Kunci dan gantungan 07 | 100-250 | Nomor cukup besar; gigi kunci tidak perlu realistis |
| Tas | 350-700 | Siluet agak mengempis; tali menyatu atau memakai strip sederhana |
| Kertas dan map | 2-80 | Lipatan hanya pada kartu yang dilihat dekat |

Target sekitar 25 mesh unik dan maksimal 80.000 triangle terlihat mengikuti cakupan GDD. Benda berulang menggunakan mesh serta material yang sama. Angka ini membatasi pekerjaan, bukan bukti performa.

### 3.2 Human Presence

Karakter pemain menggunakan prefab tubuh first-person beranimasi (`Ren_Animated.prefab` dengan `PergerakanPemain.controller`: idle, berjalan, duduk, berbaring, mengambil benda) dengan kamera sejajar mata (`eye-aligned`) dan jarak clearance depan agar mesh kepala tidak memotong pandangan saat menunduk. Gerak mengambil/duduk tetap dapat diganti perpindahan langsung melalui pengaturan Reduce Motion.

Potret manusia untuk tokoh lain tetap diperlakukan sebagai desain karakter 2D, bukan foto profil generik. Posisi bahu, jenis kerah, bentuk alis, dan arah tatapan harus membantu membedakan tokoh meskipun gambar dilihat tanpa warna.

## 4. Texture and Surface

Tekstur dibuat dengan blok warna, dua tingkat bayangan lukis, dan beberapa sapuan kuas yang mengikuti bentuk. Tidak diperlukan toon shader. Efek anime berasal dari ilustrasi tekstur dan pemilihan bentuk, bukan pencahayaan cel-shading realtime.

Modul dan properti Cat Poster berbagi atlas menurut jenis permukaan: `RoomFinish`, `DeskSet`, `Architecture`, dan `Utility`. Model lama dengan UV swatch memakai `T_SELA_Palette.png` (1024 × 1024, *Point*, tanpa *mipmap*); posisi sel tetap sama agar FBX rakitan lama tidak perlu dipetakan ulang. Potret dan ikon tetap terpisah dari atlas lingkungan.

Detail permukaan harus terpilih:

| Surface | Detail yang dibuat | Detail yang tidak diperlukan |
| --- | --- | --- |
| Dinding | Dua bekas selotip dan batas cat sedikit tidak rata | Grunge menyeluruh atau retakan fotorealistis |
| Meja | Bagian tepi depan lebih terang; satu bekas lingkar gelas | Normal map goresan rapat |
| Kain | Dua bayangan lipatan dan jahitan pendek | Serat kain individual |
| Plastik laptop | Satu bidang lebih gelap pada engsel | Refleksi layar atau sidik jari realistis |
| Kertas | Lipatan sudut dan tepi sedikit kusam | Teks kecil sebagai dekorasi yang sulit dibaca |

Teks cerita, nomor, dan syarat kontrak tidak dibakar ke atlas kecil. Gunakan elemen teks terpisah atau panel baca. Semua contoh proyek pada kartu dibuat khusus untuk Raka; tidak memakai screenshot aplikasi nyata dengan merek lain.

## 5. Lighting and Rendering

Gunakan Unity dengan URP Forward dan shader bawaan sederhana. Environment memakai URP Simple Lit dengan material opaque, normal map tidak diperlukan, dan specular highlights dimatikan. Kertas UI dan potret memakai material unlit yang sesuai Canvas.

Satu Directional Light dan ambient color menjadi dasar. Bayangan realtime hanya dari sumber utama dengan resolusi rendah atau menengah dan jarak pendek yang mencakup kamar. Lampu meja memakai bidang terang yang dilukis; tidak ada Point Light berbayang, pantulan realtime, atau efek bloom. Bila bayangan realtime terlalu berat pada perangkat pengujian, bentuk bayangan penting dapat diganti bidang tekstur yang dipasang pada permukaan, tanpa mengubah puzzle.

| Time / State | Implementasi sederhana | Hasil yang diinginkan |
| --- | --- | --- |
| Pagi (`08:00–11:59`) | Cahaya hangat dari arah jendela; ambient netral | Meja, pintu, dan kartu terbaca sejak awal |
| Siang (`12:00–17:59`) | Cahaya lebih netral; bayangan kusen lebih pendek | Aktivitas laptop tidak tenggelam dalam kontras |
| Malam (`18:00–22:00`) | Intensitas utama lebih rendah; ambient tetap cukup terang | Kamar terasa lebih sunyi, bukan gelap sampai sulit dimainkan |
| Tekanan 3-4 | Aktifkan varian bidang bayangan pada posisi sedikit berbeda | Peringatan diegetik mendekati ambang Retakan (`≥ 5`) tanpa gerak kamera |
| Retakan (`Tekanan ≥ 5`) | Ganti kelompok geometri saat fade, pertahankan cahaya pada jangkar | Ruang berubah tetapi tujuan tetap terbaca |

Anti-aliasing dinonaktifkan sebagai konfigurasi awal. Tidak memakai TAA, supersampling, ray tracing, SSAO, SSR, depth of field, motion blur, chromatic aberration, atau volumetric fog. Outline tipis (`InteractionOutline`) hanya dipakai secara selektif pada objek interaktif yang sedang disorot raycast 2 m agar target interaksi mudah dikenali. Garis diagonal lingkungan dibantu oleh bentuk yang cukup tebal dan komposisi, bukan efek rendering mahal. Pengaturan render scale awal 1,0 pada 720p; perubahan kualitas dilakukan setelah pengukuran build.

Hindari material transparan bertumpuk. Kaca jendela dapat berupa bidang opaque berwarna pucat dengan bangunan di belakangnya. Fokus laptop menampilkan Canvas UI, sedangkan panggilan video interview menggunakan satu `RenderTexture` terisolasi (`512 × 512`) dari *studio booth* NPC 3D yang **hanya diaktifkan saat dialog interview berlangsung** (nonaktif pada Free Roam). Tidak ada cermin aktif realtime atau webcam pemain.

## 6. Environment Layout and Composition

Acuan tata ruang 3D menggunakan scene [progress.unity](../Assets/progress.unity), yang terdiri atas lorong tiga kamar (`SM_Lorong_3Kamar`) dan kamar Raka di sisi kanan (`SM_Kamar_Kanan`) lengkap dengan kamar mandi dalam.

Di dalam `SM_Kamar_Kanan`, pintu masuk dari lorong berada di sisi barat dengan kamar mandi dalam (`SM_Pintu_KM`) tepat di bagian depan kamar. Area utama di sisi timur menampung meja kerja, laptop, ponsel, rak buku, dan papan bukti di dinding selatan-timur; jendela, tirai, kipas, dan kalender di dinding timur; kursi dan tas di dekat area kerja/jendela; serta kasur di sisi utara-timur. Area berjalan tengah harus tersambung dan memiliki lebar minimal sekitar 0,8 meter.

Papan bukti berada sedikit di atas meja dan laptop agar kartu terlihat dari posisi berdiri. Kursi tidak menjadi penghalang collider di jalur utama. Kasur cukup rendah agar dari area masuk pemain segera mengenali meja, jendela, dan zona tidur.

Gunakan tiga tinggi komposisi: lantai untuk tas, kipas, dan furnitur dasar; pinggang untuk meja serta kursi; pandangan mata untuk kalender, papan bukti, saklar, dan nomor pintu. Benda kecil yang tidak mendukung interaksi tidak perlu mendapat posisi setinggi mata.

Lorong (`SM_Lorong_3Kamar`) memiliki tiga pintu: Kamar 07 di kanan (`SM_Kamar_Kanan`, kamar Raka), **Kamar 06 di kiri (`SM_Kamar_Kiri`, kamar tetangga Dita yang memakai ulang modul furnitur kos)**, dan pintu ujung yang terkunci pada kondisi normal, beserta bangku lorong (`SM_Bangku_Lorong`). Lorong ini dilalui pemain saat menemui Dita sekaligus menjadi dasar langsung untuk Retakan Ruang 2 (Pintu Bernama). Tidak diperlukan simulasi penghuni kos berjalan bebas (NPC Dita berposisi tetap dengan animasi halus di dalam kamarnya).

## 7. Retakan Ruang Visual Rules

### 7.1 Meja yang Menjauh

Salinan kamar bertambah panjang menjadi 7 meter. Lebar tetap dan UV tidak ikut diregangkan; tambahkan modul dinding serta lantai. Meja berada di ujung, sedangkan rak kartu E0 berada dekat titik masuk. Lembar penolakan berupa beberapa bidang opaque yang diam, bukan partikel terbang.

Penjepit tujuan memakai sudut foto yang sama dengan kartu E0. Begitu kartu terpasang, fade singkat mengembalikan panjang kamar. Jangan memindahkan lantai di bawah kaki pemain secara realtime.

### 7.2 Pintu Bernama

Tiga muka pintu memakai model yang sama, tetapi label serta tata letaknya berbeda. Pintu 07 mempertahankan bentuk pelat terpotong dari dunia normal. DITERIMA dan TERLAMBAT memakai tulisan di kertas tempelan, bukan mulut atau mata.

Kunci ditempatkan pada bangku dengan bidang terang sederhana. Dua pintu palsu berubah menjadi panel dinding saat fade. Tidak ada lorong tak berujung atau sistem portal.

### 7.3 Kursi Kosong

Kursi tambahan memakai mesh yang sama dengan kursi jendela. Susun di tepi ruangan dan hindari menutup jalur ke tujuan. Label berisi kalimat pendek seperti Sudah sampai mana?; label tidak menyerupai suara orang tertentu yang menyalahkan Raka.

Kartu pesan ibu dipasang pada dudukan datar di kursi jendela. Kursi tambahan hilang saat fade. Satu kursi asli tetap ada agar ruang tidak berubah menjadi gambar kemenangan yang sempurna.

Intensitas atmosfer Rendah mempertahankan bentuk ketiga puzzle, tetapi menghilangkan pergeseran bayangan serta gema tambahan. Pemain yang melewati puzzle melihat komposisi hasil yang sama.

## 8. Character Models & Portraits

Lihat [acuan potret karakter](visuals/06-karakter-potret-interview.webp) sebagai acuan siluet dan gaya anime *Cat Poster Kos*. Untuk implementasi di Unity:
- **Dita** hadir sebagai **NPC 3D di Kamar 06 (`SM_Kamar_Kiri`)** tepat di seberang kamar Raka, memakai animasi halus berposisi tetap (*sit/stand idle*, mengangguk) tanpa AI berjalan bebas.
- **Nara** dan **Bagas** hadir sebagai **NPC 3D pada *video-call studio booth*** tersembunyi di luar kamar, dirender ke `RenderTexture` `512 × 512` pada layar laptop hanya saat sesi interview berlangsung (gerakan halus: *idle*, menyimak, mengangguk, berpikir). Potret 2D tetap menjadi *fallback* resmi bila dibutuhkan.
- **Ibu** tampil sebagai foto kecil 2D di Ponsel, dan **Admin Akses Prima** tetap memakai avatar geometris `AP` tanpa wajah untuk mempertegas tanda risiko.

| Character | Silhouette and Costume | Face and Expression |
| --- | --- | --- |
| Dita | Rambut sebahu diikat rendah; kaus biru tinta dan overshirt hijau tanpa aksesori mencolok | Alis agak lurus; tatapan sejajar; netral dan senyum kecil |
| Nara | Rambut pendek dengan belahan samping; kemeja berkerah terbuka dan kartu nama meja dalam potret | Alis tipis melengkung; tiga ekspresi: mendengar, menjelaskan, mempertimbangkan |
| Bagas | Rambut sedikit bergelombang; kemeja kerja sederhana dengan satu kantong | Bentuk wajah lebih lebar; tiga ekspresi dengan perubahan alis dan mulut terbatas |
| Ibu | Foto kecil dalam pesan, dengan baju sehari-hari | Ekspresi tenang; tidak memerlukan variasi |
| Admin Akses Prima | Avatar geometris dengan inisial AP | Tidak memiliki wajah; identitas meragukan dijelaskan melalui isi pesan |

Mata memakai bentuk sederhana dengan iris kecil, satu warna gelap, dan highlight terbatas. Hidung dapat berupa dua garis bidang. Mulut tidak perlu animasi lip-sync. Ekspresi dibuat dengan mengganti beberapa bagian gambar, bukan menggambar ulang pakaian untuk setiap respons.

Jangan meniru rambut, kostum, proporsi, atau aksesori karakter dari game referensi. Tidak ada karakter pendamping dengan kostum maskot atau tampilan yang dibuat untuk kontras mendadak antara imut dan sadis.

## 9. Interface and Typography

### 9.1 Information Hierarchy

HUD memperlihatkan `Hari X/7 · HH:00 (Pagi/Siang/Malam)` disertai satu baris pengingat tujuan singkat di bawahnya (misalnya *"Interview Studio Lajur: 13:00 (butuh Tenaga 1)"* atau *"Siap menutup hari — menuju Kasur"*), serta `Tenaga` (empat kotak + angka) dan `Tekanan` (enam garis + angka). Bar stat beranimasi halus saat berubah (tidak meloncat instan). Keterangan bahwa Tekanan adalah kondisi permainan (bukan diagnosis) dicantumkan pada panel Catatan. Catatan rinci muncul saat Tab, bukan selalu memenuhi layar.

Prompt berada dekat bawah tengah dan berisi tombol serta kata kerja. Benda interaktif tidak dikelilingi garis menyala. Penjepit kuning, sudut kartu, dan prompt sudah cukup untuk membedakannya. Panel mendukung fokus keyboard melalui tombol panah, aktivasi Enter/Spasi, serta scroll Page Up/Page Down. Fokus memakai garis tebal dan mengikuti urutan baca. Modal mengunci input ke panel teratas; saat ditutup, fokus kembali ke tombol pemanggil. Tab tetap untuk Catatan dan tidak membuka overlay baru di atas konfirmasi modal.

Teks utama memakai Noto Sans atau font sans-serif berlisensi terbuka dengan karakter bahasa Indonesia yang lengkap. Gunakan satu keluarga font, dua weight, dan ukuran tubuh awal sekitar 22-24 px pada 1080p yang diskalakan sesuai resolusi. Nomor kartu dan hari dapat memakai angka tabular dari font yang sama. Lisensi font masuk Credits.

### 9.2 Laptop and Evidence Board

Lihat [Layar Laptop](visuals/04-antarmuka-laptop-verifikasi.webp) dan [Papan Bukti](visuals/05-papan-bukti-closeup.webp).

Laptop menampilkan tiga tab bernama Lamaran, Pesan, dan Catatan. Panel kiri berisi daftar; panel kanan memuat isi. Satu tawaran dibuka pada satu waktu agar syarat tidak dipadatkan menjadi kolom kecil. Semua daftar bisa digulir dan memiliki indikator posisi.

Meja Bukti memakai kartu yang dipilih lewat klik lalu dipasang ke penjepit. Pemilihan terlihat melalui garis tebal dan label Dipilih, bukan hanya perubahan warna. Klaim tanpa dukungan memiliki teks Bukti belum mendukung kalimat ini. Ikon gembok tidak dipakai karena opsi dialog tidak dikunci oleh stat.

Panel aktivitas menampilkan durasi jam dan biaya stat sebelum tombol `Jalankan aktivitas`. Pengiriman lamaran yang berbenturan jadwal pukul `13:00` memiliki alasan tertulis. Panel final selalu menyebut nama perusahaan atau pilihan pulang, kemudian konsekuensi utama. Tombol Kembali membaca sama mudah dicapai dengan tombol konfirmasi.

### 9.3 Menu

Menu utama memakai komposisi kamera tetap yang menampilkan meja kosong dan tirai. Judul berada pada bidang kosong di sebelah kiri, daftar menu tepat di bawahnya. Tidak ada karakter menatap pemain atau animasi latar berat.

Pause merupakan panel sederhana dengan teks terang pada bidang gelap yang cukup kontras. Submenu tetap berada pada satu tingkat. Pengaturan mencakup volume, sensitivitas, invert Y, FOV, ukuran teks, Reduce Motion, dan intensitas atmosfer. Tidak ada indikator skor atau statistik produktivitas di menu.

### 9.4 Screen States and HUD Specification

Lihat [12: HUD and Focus](visuals/12-hud-focus-states.jpg). Acuan posisi mengikuti ilustrasi HUD yang sudah ada: Hari/Sesi di kiri atas, Tenaga/Tekanan di kiri bawah, prompt di bawah tengah. Pada kanvas acuan 1280 × 720, sisakan margin aman minimal 32 px. Panel fokus menggunakan margin luar 48 px dan area isi yang dapat digulir. Nilai ini mengatur layout awal, bukan mengganti pengujian pada ukuran teks 150%.

Tenaga selalu memakai tepat empat kotak dan angka `3/4`, sedangkan Tekanan memakai tepat enam garis dan angka `2/6`. Kotak atau garis kosong tetap terlihat. Label Tekanan memiliki keterangan `Kondisi permainan, bukan diagnosis` pada panel Catatan; tidak ada detak visual yang makin cepat ketika nilainya meningkat.

| Screen State | Tampilan | Input dan perilaku visual |
| --- | --- | --- |
| Free Roam tanpa target | HUD terlihat, prompt kosong | Kamera mengikuti mouse; benda dekoratif tidak diberi tanda interaksi |
| Target sah dalam jarak 2 m | Titik bidik kecil dan prompt `E · Baca catatan` | Prompt hilang ketika target terhalang atau di luar jangkauan; tanpa outline shader |
| Panel baca atau Focus | Kartu isi, judul, kursor, dan `Esc · Kembali` | Gerak serta penoleh kamera berhenti; tidak menghabiskan sesi |
| Pilihan aktivitas | Panel konfirmasi durasi jam dan kondisi saat ini | Membaca panel tidak mengubah nilai; tombol utama baru menjalankan aktivitas dan memajukan jam |
| Pause atau Catatan | Satu panel teratas yang jelas | Dunia tidak menerima interaksi melalui panel; Esc menutup panel teratas terlebih dahulu |

Panel tidak menampilkan prompt E milik dunia di belakangnya. Saat Focus ditutup, HUD kembali ke posisi yang sama. Tidak ada indikator inventaris umum karena hanya benda puzzle aktif yang dapat dibawa.

### 9.5 Activity Confirmation, Availability, and Results

Lihat [13: Activity States](visuals/13-activity-states.jpg), [24: Room Activity Presentation](visuals/24-room-activities.jpg), dan [25: Portfolio Work Presentation](visuals/25-portfolio-work.jpg). Semua aktivitas berdurasi memakai satu komponen panel: judul, lokasi, durasi (`1`, `2`, atau `3 jam`) beserta jam mulai→selesai, kondisi sebelum/sesudah, hasil khusus, peringatan Retakan/Jeda/Interview jika berlaku, tombol `Jalankan aktivitas`, dan `Batal`. Pratinjau nilai mengikuti batas Tenaga 4 dan Tekanan 6; menambah Tenaga 3 saat nilainya 3 ditulis `3 → 4`, bukan `3 → 6`.

| Activity | Isyarat visual dan hasil khusus | Availability |
| --- | --- | --- |
| Rapikan contoh antarmuka | Satu pilihan pasangan label/pesan, perbandingan sebelum/sesudah, dan kartu E1; `3 jam`, Tenaga −1, Tekanan +1 | Setelah selesai: `E1 sudah tersedia` |
| Lengkapi catatan pengujian | Tiga kasus uji, satu pilihan kesimpulan, dan kartu E2; `3 jam`, Tenaga −1, Tekanan +1 | Sebelum Hari 3: `Tersedia mulai Hari 3`; tanpa E1: `Selesaikan antarmuka terlebih dahulu` |
| Periksa perusahaan | Empat aspek syarat dan hasil klarifikasi; `2 jam`, Tenaga −1, Tekanan tetap | Selesai sekali per perusahaan; sesudahnya `Baca hasil verifikasi` gratis |
| Kirim lamaran | Nama perusahaan, dua klaim, bukti terkirim, dan jadwal besok `13:00`; `2 jam`, Tenaga −1, Tekanan +1 | Alasan tertulis untuk batas lamaran, pengiriman yang sudah dilakukan, dan benturan jadwal |
| Ikuti interview | Nama perekrut dan `2 pertanyaan`; `2 jam` (`13:00–15:00`), Tenaga −1, Tekanan +2 | Hari interview saat `jam ≤ 13:00`; kekurangan Tenaga tetap dijelaskan |
| Beristirahat | Pandangan kasur dan hasil pemulihan; `2 jam`, Tenaga +3, Tekanan −2 | Tersedia `08:00–17:59` (Pagi–Siang), termasuk Tenaga 0; mulai `18:00` kasur menawarkan `Tidur / Tutup hari` |
| Mandi | Pandangan kamar mandi (`SM_Shower`/`SM_Bak_Mandi`) dan catatan kesegaran; `1 jam`, Tenaga +2, Tekanan −3 | Maksimal sekali sehari (`Sudah mandi hari ini`); pemulihan Tekanan terbesar |
| Hubungi Dita | Ponsel dan percakapan aktivitas; `2 jam`, Tenaga +2, Tekanan −2 | Selalu tersedia selama waktu cukup sebelum `22:00`; baca pesan saja gratis |
| Duduk tanpa layar | Komposisi kursi/jendela; `1 jam`, Tenaga +2, Tekanan −2 | Selalu tersedia selama waktu cukup sebelum `22:00`; animasi dapat dilewati |
| Rapikan meja | Perbandingan dua susunan benda; `1 jam`, Tenaga +1, Tekanan −1 | Setelah sekali: `Meja sudah dirapikan hari ini`; tersedia kembali besok |
| Kemas barang & siapkan jawaban | Pandangan tas dan map tawaran; `2 jam`, Tenaga 0, Tekanan 0 | Mulai Hari 4 (atau kapan saja Hari 7, tidak diblokir batas `22:00`); membuka Keputusan Akhir |

Biaya selalu ditulis dengan nama stat dan durasi jam; warna saja tidak cukup. Tombol tidak aktif memakai garis putus-putus dan alasan yang tetap dapat dibaca. Tidak ada ikon gembok untuk kelelahan atau keterbatasan bukti. Pada Tenaga 0, tampilkan tautan informasi `Aktivitas pemulihan tetap tersedia`, bukan tombol pemulihan baru yang bekerja dari semua lokasi.

Setelah aktivitas, tampilkan ringkasan satu kali: hasil, nilai akhir, dan jam terbaru (`HH:00 · Pagi/Siang/Malam`). Animasi singkat dapat dilewati tanpa membatalkan hasil. Jika Tekanan mencapai `≥ 5` dan memicu Retakan Ruang (atau `6` untuk Jeda cadangan), checkpoint stabil disimpan lebih dahulu sebelum peristiwa dimulai. Hasil interview yang ditolak menyebut tambahan Tekanan 1 secara terpisah agar biaya dasar dan konsekuensi tidak tampak bertentangan.

Setelah biaya dikonfirmasi, E1 meminta pemain memilih pasangan label/pesan yang lebih jelas; E2 meminta satu kesimpulan yang sesuai tiga kasus tercatat. Pilihan memakai dua kartu besar dengan komponen Meja Bukti. Pilihan kurang tepat menampilkan alasan lalu dapat diulang tanpa biaya atau perubahan stat. Setelah pilihan selesai, tampilkan hasil, berikan bukti, dan terapkan biaya satu kali. Perbandingan formulir, tiga kasus, dan batas pengujian tetap berupa materi yang ditulis sebelumnya. Tidak ada editor kode atau tantangan mengetik.

Panel pemulihan menampilkan manfaat pengalaman di samping angka: istirahat untuk Tenaga terbesar, mandi untuk Tekanan terbesar sekali sehari, Dita untuk percakapan hari itu, duduk untuk pengamatan singkat, dan merapikan untuk perubahan benda. Teks pengulangan pendek. Aktivitas kamar memakai animasi tubuh `Ren_Animated` atau transisi langsung saat Reduce Motion aktif. Merapikan meja mengganti dua susunan objek tetap tanpa simulasi fisika.

Semua panel aktivitas memeriksa kecukupan waktu sebelum pukul `22:00` (kecuali `Kemas barang & siapkan jawaban` yang selalu dapat dijalankan saat terbuka). Pada pukul `22:00`, tombol aktivitas berdurasi biasa tidak aktif dengan alasan `Sudah larut malam (22:00). Gunakan kasur untuk tidur.`, sedangkan tombol baca, draf CV, serta peninjauan tawaran tetap tersedia. Pratinjau menyebut jika aktivitas akan memicu Retakan Ruang atau Jeda.

### 9.6 Calendar, Clock Changes, and Sleep

Lihat [14: Calendar and Sleep](visuals/14-calendar-sleep.jpg). Kalender menampilkan tujuh kotak hari (batas maksimal sewa); hari aktif memakai bingkai tebal dan label Hari ini. Di bawahnya terdapat jam berjalan (`08:00–22:00`) dan penanda suasana lingkungan `Pagi`, `Siang`, `Malam` yang otomatis mengikuti jam. Tidak ada hitung mundur real-time.

Undangan interview memakai teks lengkap, misalnya `Hari 4 · 13:00 (Siang) · Koperasi Pijar`. Batas lamaran menyebut akhir hari (`22:00`). Retakan tidak ditampilkan sebagai jadwal tetap pada kalender karena dipicu oleh Tekanan (`≥ 5`).

Panel pengiriman menyebut `Interview besok pukul 13:00 memerlukan Tenaga 1`. Kartu bangun pagi (`08:00`) dan baris pengingat HUD pada hari interview menyebut `Interview hari ini pukul 13:00 memerlukan Tenaga 1`, beserta kondisi saat ini. Kalender hari 7 menampilkan keberangkatan bus pagi hari 8 sebagai informasi cerita, tanpa kotak hari 8 yang dapat dimainkan.

Mulai pukul `18:00` (`Malam`) pada Hari 1–6, prompt kasur berubah menjadi `E · Tidur / Tutup hari`. Panel menampilkan pemulihan tidur `Tenaga +2, Tekanan −1` beserta nilai akhir yang sudah dibatasi, lalu melompat ke hari berikutnya pukul `08:00`. Sebelum `18:00`, kasur menawarkan `Beristirahat` (`2 jam`, `+3 Tenaga, −2 Tekanan`). Pada Hari 7 mulai `18:00` (atau kapan saja melalui Tas/Map Tawaran mulai Hari 4), pemain dapat menjalankan `Kemas barang & siapkan jawaban` menuju panel `Keputusan Akhir`.

Jika pemain memilih aktivitas lain yang jam selesainya melampaui `13:00` pada hari interview, tampilkan nama undangan, akibat `Interview terlewat; Tekanan +1`, serta pilihan `Kembali` dan `Tetap lakukan aktivitas`. Konfirmasi ini mendahului pelaksanaan aktivitas dan hasil tidak diulang ketika inbox dibuka kembali.

### 9.7 Pressure Bands, Retakan Trigger, and Jeda

Lihat [15: Pressure and Jeda](visuals/15-pressure-jeda.jpg). Tekanan `0–2` memakai ruang normal. Tekanan `3–4` memakai varian bayangan dan ekor gema pendek sebagai peringatan mendekati ambang Retakan; bukan kedipan, kabut, blur, atau pengurangan keterbacaan. Pada angka `5–6`, layar tidak menampilkan pesan Game Over.

Saat Tekanan mencapai `5` atau `6` sesudah aktivitas dan Retakan babak tersebut belum diselesaikan hari ini, permainan masuk ke **Retakan Ruang** terkait. Menyelesaikan Retakan (langsung atau lewat bantuan `Selesaikan adegan`) menurunkan `Tekanan → 3` dan `Tenaga ≥ 1` tanpa menghabiskan jam.

Jika angka `6` tercapai ketika tidak ada Retakan yang memenuhi syarat (dan Jeda belum terjadi hari ini), **Jeda cadangan** membuka panel kertas dengan dua pilihan setara: `Duduk sebentar` dan `Kirim pesan kepada Dita`. Ringkasan menyebut `Tekanan menjadi 4; Tenaga minimal 1; tanpa memakai waktu`. Jika angka 6 tercapai lagi pada hari yang sama setelah Jeda, HUD tetap `6/6` tanpa membuka ulang modal. Mode atmosfer Rendah mematikan pergeseran bayangan dan gema tambahan; aturan Retakan dan Jeda tidak berubah.

### 9.8 CV Claims and Application Snapshot

Lihat [16: CV and Snapshot](visuals/16-cv-snapshot.jpg). Panel CV menampilkan tepat dua baris klaim dengan pilihan kalimat yang sudah ditentukan dalam GDD. Di samping setiap baris terdapat bukti yang dipasang atau label `Belum ada bukti`. Kartu yang belum diperoleh memiliki siluet kosong dan keterangan cara memperolehnya, bukan kartu isi fiktif.

Kalimat yang berlebihan memakai ikon segitiga dan teks `Bukti belum mendukung kalimat ini`. Pilihannya tetap dapat diklik. Jangan memberi tanda centang yang menyamakan klaim berlebihan dengan kelengkapan CV. E0 dapat dibaca di papan, tetapi tidak diberi label memenuhi syarat lowongan A/B.

Sebelum Kirim, panel menampilkan nama perusahaan, dua kalimat persis yang dipilih, identitas E1/E2 yang terpasang, bukti wajib yang belum ada, serta jadwal (`besok pukul 13:00`) dan kebutuhan Tenaga interview. Bukti yang belum lengkap memunculkan peringatan: `Bukti wajib belum ikut dikirim; tawaran tidak dapat diberikan dari snapshot ini. Bukti yang dibuat nanti tidak memperbarui lamaran ini.` Pengiriman tetap tersedia selama syarat waktu, Tenaga, dan jadwal terpenuhi. Panel juga menyebut satu kesempatan pengiriman per perusahaan serta durasi (`2 jam`) dan biaya stat.

Setelah terkirim, tab riwayat menampilkan salinan baca-saja berlabel `Dikirim pada Hari X · Sesi Y`. CV yang sedang diedit diberi label `Draf saat ini`. Jika pemain memperoleh E2 kemudian, E2 tidak diam-diam muncul pada riwayat pengiriman lama. Di awal interview, tombol `Luruskan seluruh klaim` menambahkan catatan koreksi untuk lamaran itu. Klaim asli tetap terlihat pada riwayat dan bukti snapshot tidak bertambah. Draf CV disimpan setelah panel ditutup pada keadaan stabil.

### 9.9 Job Availability, Verification, and Scheduling

Lihat [17: Recruitment and Verification](visuals/17-recruitment-verification.jpg). Daftar Lamaran menyebut status per perusahaan: `Belum dibuka`, `Terbuka sampai ...`, `Interview terjadwal`, `Ditolak`, `Tawaran tersedia`, atau `Jendela lamaran berakhir`. Nama perusahaan yang belum dibuka tetap memiliki alasan tanggal, bukan gembok tanpa keterangan. Akses Prima hadir sebagai tawaran langsung mulai hari 4; tidak mempunyai tombol lamaran atau interview.

Panel verifikasi memuat empat baris: Tugas, Kompensasi, Jam kerja dan istirahat, serta Dokumen pribadi. Setiap baris memiliki pilihan pemain `Jelas`, `Perlu ditanyakan`, dan `Berisiko`. Keadaan awal adalah `Belum ditandai`, bukan otomatis Jelas. Penandaan gratis waktu dan berbeda dari aktivitas Periksa perusahaan yang memakai `2 jam`.

Sesudah pemeriksaan, tampilkan `Penilaianmu`, `Pernyataan awal`, dan `Jawaban klarifikasi` pada aspek yang sedang dibaca. Pakai baris bertumpuk agar teks tetap terbaca pada 720p. Pertanyaan A membahas pendamping serta lembur; B membahas tugas awal dan ketidakpastian pembaruan kontrak; C membahas tugas harian serta permintaan dokumen. Ketidaksesuaian diberi penjelasan spesifik, bukan skor benar/salah. Informasi yang belum terjawab tetap terlihat sebagai ketidakpastian. Ubah penandaan tidak menghabiskan sesi dan tidak menghapus hasil klarifikasi. Pemeriksaan hanya tersedia setelah lowongan/pesan dibuka; hasilnya dapat dibaca ulang gratis. Draf Akses Prima tetap menunjukkan penahanan dokumen meskipun pemeriksaan belum dilakukan.

Jika jadwal besok pukul `13:00` sudah terisi, tombol pengiriman perusahaan lain tidak aktif dengan teks `Hari X pukul 13:00 sudah dipakai untuk interview [nama]`. Bila jendela berakhir sebelum kesempatan mengirim berikutnya, tampilkan status berakhir, bukan menawarkan penjadwalan ulang yang tidak ada di GDD. Semua hasil bersifat lokal dan deterministik; jangan menggambar indikator koneksi internet sungguhan.

### 9.10 Interview Screens and Outcome Feedback

Lihat [18: Interview and Results](visuals/18-interview-results.jpg). Gunakan tampilan video-call NPC 3D Nara atau Bagas dari *studio booth* (`RenderTexture` `512 × 512`, atau fallback potret 2D), satu tokoh per sesi. Dita tidak muncul sebagai perekrut. Panel kiri berisi bingkai video-call/potret dan nama; panel kanan berisi pertanyaan serta catatan bukti terkirim; pilihan dialog mengisi bagian bawah.

Tahap pembuka hanya menampilkan koreksi klaim jika ada klaim berlebihan. Pilihannya `Luruskan seluruh klaim` dan `Lanjut dengan klaim terkirim`; konsekuensinya dijelaskan tanpa menutup respons pertanyaan. Sesi tetap hanya mempunyai dua pertanyaan. Judul menampilkan `Pertanyaan 1/2`, lalu `2/2`, tanpa timer.

Ketiga respons pada tiap pertanyaan tampil sebagai kalimat penuh. Tidak ada gembok, warna jawaban benar, nilai +1/+2, atau indikator peluang diterima. Memilih jawaban menampilkan kutipan jawaban Raka sebelum pemain menekan Lanjut, sehingga salah klik tidak langsung memindahkan pertanyaan. Catatan interview hanya memuat bukti pada snapshot lamaran, bukan seluruh portofolio terbaru.

Pilihan jawaban mengikuti rumusan GDD 3.7: perbedaan terletak pada contoh, batas pengujian, prioritas, dan kebutuhan pendampingan. Hindari jawaban lemah yang hanya berupa janji berlebihan tanpa alasan masuk akal.

Hasil positif berupa surat tawaran dengan ringkasan tugas, masa kontrak bila ada, tanggal mulai, pembayaran pertama, dan `Baca tawaran`. Jika beberapa syarat gagal, tampilkan alasan utama berurutan: bukti wajib belum lengkap, klaim belum diluruskan, lalu penjelasan jawaban belum cukup. Alasan lain tersimpan di Catatan tanpa angka skor. Undangan terlewat memakai judul tersendiri `Interview terlewat`, bukan dialog seolah percakapan sudah berlangsung. Tidak ada nilai total, blacklist, komentar kebersihan kamar, atau penilaian kondisi mental.

### 9.11 Messages, Notes, and Support Contact

Lihat [19: Messages and Notes](visuals/19-messages-notes.jpg). Pesan menggunakan daftar di kiri dan isi di kanan. Label pengirim membedakan Ibu, Dita, perusahaan, dan Admin Akses Prima. Pesan baru memiliki titik dan kata Baru; setelah dibuka, tanda itu hilang. Pesan penting tetap tersedia meskipun pemain tidak memilih aktivitas sosial.

Undangan memiliki tanggal/sesi dan tombol `Ikuti interview` hanya pada sesi yang sah. Pesan hasil memiliki tautan ke penjelasan atau surat tawaran. Pesan lama tidak mengulang biaya atau perubahan Tekanan. Notifikasi kecil menunjuk inbox tanpa memaksa pemain segera membacanya.

Ponsel membedakan tombol `Baca pesan` (Ibu & Dita, gratis waktu) dan `Temui / Hubungi Dita` (`2 jam`, dapat dilakukan lewat Ponsel maupun mengunjungi Kamar 06 di seberang lorong). Membaca ulang percakapan tidak memberi pemulihan baru. Aktivitas Temui / Hubungi Dita menampilkan dialog pertama yang berbeda tiap hari dan versi pendek untuk pengulangan.

Tab Catatan pada laptop dan overlay Tab menampilkan isi yang sama: Bukti, Jadwal, Fakta ruang, dan Riwayat hasil. Ini kategori dalam Catatan, bukan aplikasi tambahan. Fakta yang diperlukan puzzle selalu dapat dibuka ketika puzzle aktif. Dari interview, Catatan kembali ke interview; dari Free Roam kembali ke kamera semula. Riwayat hasil tidak memajang skor interview atau statistik produktivitas.

Hari 5-7 memakai pesan dasar dengan sisipan dua tawaran, satu tawaran, atau belum ada tawaran. Ajakan klarifikasi membuka hasil yang sudah diperoleh atau panel pemeriksaan (`2 jam`) yang belum dilakukan. Pesan tidak membeli pemeriksaan ulang dan tidak menahan syarat penting sampai akhir minggu. Pesan ibu tentang tiket (*Tiketnya tetap boleh dipakai*) masuk pagi Hari 5; Hari 7 membahas keberangkatan atau rencana tinggal (sementara sesi persiapan akhir sudah dapat dipilih mulai Hari 4). Materi ini memakai panel yang sama tanpa aplikasi baru.

### 9.12 Puzzle Interaction, Hints, and Audio Captions

Lihat [20: Puzzle Interaction and Help](visuals/20-puzzle-help.jpg). Ilustrasi Retakan 07-09 menetapkan suasana; lembar ini menetapkan apa yang dilakukan pemain. Saat belum membawa benda, prompt menyebut `E · Ambil kartu proyek` atau nama objek yang sesuai. Sesudah mengambil, label kecil `Dibawa: Kunci 07` muncul; tidak ada inventaris grid.

Saat mengarah ke tujuan sah, prompt menyebut tindakan dan tujuan. Tujuan salah menghasilkan teks faktual singkat, mempertahankan benda yang dibawa, dan tidak mengurangi stat. Dudukan asal menyediakan prompt Kembalikan. Satu benda aktif tidak dapat digandakan.

Pembuka Retakan mengikuti paling banyak dua variasi per adegan berdasarkan flag GDD 3.8. Fakta E0, nomor 07, dan pesan pagi Hari 5 ditampilkan melalui benda serta Catatan yang sudah ada. Petunjuk menyebut hubungan faktanya dengan ruang, tanpa menambah urutan puzzle atau objek baru. Layout dan solusi tetap sama pada semua riwayat lamaran.

Tombol Petunjuk tersedia di Pause sejak awal. Sesudah tiga percobaan salah pada Retakan yang sama, label tujuan muncul otomatis dan tetap terlihat sampai adegan selesai. `Selesaikan adegan` menampilkan konfirmasi bahwa hasil cerita sama dan tidak memakai sesi; sesudah dikonfirmasi pemain melihat keadaan ruang yang sudah pulih.

Caption audio berada di bawah tengah tetapi di atas prompt interaksi. Contoh: `[Kunci bergeser di dekat pintu 07]`. Jangan menumpuk caption dengan pilihan dialog; jika Focus terbuka, caption memakai ruang tersendiri di panel. Tidak perlu menambah toggle caption ke daftar pengaturan GDD; petunjuk penting tetap mempunyai caption saat relevan, sedangkan teks arah bunyi kunci dapat ditampilkan bersama petunjuk opsional. Tidak ada musuh, pengejaran, atau penghitung nyawa.

### 9.13 Temporary Choice, Risk Warning, and Final Decision

Lihat [21: Contract Decision States](visuals/21-contract-decision.jpg). Map 10 menjadi acuan benda fisik. Panel interaksinya memperlihatkan surat yang benar-benar diperoleh, bukan semua surat resmi dianggap tersedia sejak awal. Pilihan Pulang selalu ada. Surat yang belum diperoleh memiliki alasan `Belum ada tawaran`, bukan tombol Terima aktif.

Pilihan sementara memakai label `Pilihan saat ini`, nama perusahaan, dan tombol `Tarik pilihan`. Memilih surat lain mengganti satu pilihan tersebut, bukan menambah kontrak kedua. Menutup panel tidak mengubah pilihan. Menarik pilihan menampilkan konfirmasi dan mengembalikan keadaan ke tidak ada tawaran yang dipilih; itu berbeda dari menghapus surat dari daftar.

Memilih Akses Prima membuka peringatan tentang dokumen asli dan pembatasan pengunduran diri, dengan `Kembali membaca` serta `Pilih sementara meski berisiko`. Peringatan tidak disamarkan oleh ikon dekoratif atau nomor warna. Sebelum akhir malam hari 7, tombol finalisasi belum tersedia dan menjelaskan waktunya.

Panel final menampilkan `Hari 7 · Malam selesai`, pilihan yang sah atau Pulang, konsekuensi utama, dan tombol `Selesaikan keputusan`. Sesudah tombol ditekan, konfirmasi terakhir memakai label spesifik seperti `Konfirmasi pulang` atau `Konfirmasi Studio Lajur`. Tidak ada hitung mundur. Pada C, syarat berisiko tetap ditampilkan di konfirmasi terakhir, bukan dianggap telah dibaca pada layar sebelumnya saja.

Di panel yang sama, A/B memiliki dua catatan yang sama-sama sah: `Pendampingan dan lingkup tugas` atau `Persetujuan dan pencatatan lembur`. Pulang memiliki `Menata bukti proyek` atau `Menyusun jadwal lamaran terbatas`. Satu pilihan mengisi `decisionNote` tanpa memakai sesi atau mengubah kelayakan ending. Mengganti tawaran menghapus catatan yang tidak sesuai. C memakai pengakuan syarat pada konfirmasi tambahan. Pesan akhir menyebut catatan, jadwal, dan rencana tempat tinggal sesuai GDD 3.12; tidak ada panel negosiasi atau perubahan kontrak.

Sesudah epilog, ringkasan memuat keputusan, bukti yang dipakai, aspek syarat yang diperiksa, serta alasan berbasis riwayat. Tidak ada medali, bintang, urutan ending terbaik, atau label kalah karena Tekanan. Jika E1/E2 belum selesai, teks tidak menyatakan portofolio lengkap. Variasi A/B tetap satu ending Mulai dengan Batas.

### 9.14 Main Menu, Pause, and Checkpoint Feedback

Lihat [22: Menu and Save States](visuals/22-menu-save-states.jpg). Main Menu berisi Mulai baru, Lanjutkan, Pengaturan, Credits dan Sumber, serta Keluar. Tanpa simpanan yang terbaca, Lanjutkan tidak aktif dan diberi alasan. Pause berisi Lanjutkan, Pengaturan, Ulangi dari checkpoint, Kembali ke menu, serta Keluar. Selama Retakan, tambahkan Petunjuk dan Selesaikan adegan dalam panel yang sama; jangan menambah tingkat menu permanen.

Checkpoint hanya menampilkan dua identitas teks: `Autosave terakhir: Hari X · Sesi Y` dan `Awal hari: Hari X · Pagi`. Tidak ada slot manual, thumbnail, atau daftar halaman. Ikon kecil Menyimpan berganti menjadi Tersimpan hanya setelah penulisan berhasil. Bila penulisan gagal, jangan menampilkan Tersimpan; beri pesan bahwa progres terbaru belum tersimpan dan sebutkan checkpoint stabil terakhir jika tersedia.

| Save State | Pesan dan tindakan yang ditampilkan |
| --- | --- |
| Keluar saat Free Roam stabil | Sebutkan autosave terakhir; konfirmasi Kembali ke menu atau Keluar sesuai tujuan |
| Keluar saat interview/Retakan | `Bagian yang belum mencapai checkpoint akan diulang`; tampilkan hari/sesi checkpoint, bukan posisi kamera sementara |
| Ulangi dari awal hari | Sebut hari tujuan dan progres sesudahnya yang akan diganti; tombol `Batal` dan `Ulangi hari ini` |
| Mulai baru dengan progres lama | Peringatan menimpa progres; `Batal` dan `Mulai baru` |
| Autosave tidak terbaca, awal hari terbaca | Penjelasan singkat dan `Muat awal hari`; tidak otomatis mengaku melanjutkan autosave |
| Kedua simpanan tidak terbaca | Lanjutkan tidak tersedia; pilihan Mulai baru dengan konfirmasi dan Kembali |

Pengaturan disimpan terpisah; pesan simpanan permainan tidak menjanjikan bahwa pengaturan ikut diulang dari checkpoint. Semua peringatan memakai komponen dialog yang sama, sehingga tambahan state tidak memerlukan desain jendela baru.

Menutup panel draf CV, penandaan verifikasi, atau Pesan pada keadaan stabil menyimpan perubahan terkait. Pilihan tawaran dan catatan keputusan juga disimpan. Saat aktivitas bukti, interview, hasil, Jeda, atau Retakan belum selesai, gunakan file stabil sebelumnya dan jelaskan aktivitas yang akan diulang. Panel sementara, scroll, serta sorotan tombol tidak disimpan. Lanjutkan membuka keadaan stabil; malam yang sudah selesai tetap hanya menawarkan penutupan hari dan interaksi gratis.

Sesudah epilog, Lanjutkan memakai autosave sebelum finalisasi dan diberi keterangan `Kembali ke keputusan Hari 7 · Malam`. Checkpoint awal hari tetap hari 7. Tidak ada slot tamat, panel pemilihan ending, atau hari 8 yang dapat dimainkan.

### 9.15 First Launch, Settings, Credits, and Support

Lihat [23: Settings and Reading Screens](visuals/23-settings-reading.jpg). Sebelum gameplay baru, panel informasi konten menyebut penolakan kerja, tekanan finansial, perbandingan sosial, dan rekrutmen eksploitatif. Pemain dapat membuka Pengaturan lebih dahulu, kembali ke menu, atau melanjutkan cerita. Jangan menampilkan pilihan terapi atau klaim manfaat klinis.

Pengaturan menggunakan satu panel scroll dengan kelompok Audio, Kamera, dan Keterbacaan. Audio memuat Master, Ambience, Effects; Kamera memuat sensitivitas, invert Y, FOV 60-90, dan Reduce Motion; Keterbacaan memuat ukuran teks 100/125/150% dan intensitas atmosfer Normal/Rendah. Tidak ada preset grafis yang menjanjikan fitur di luar cakupan. Tampilkan contoh teks, angka stat, dan prompt di samping kontrol keterbacaan.

Area scroll mempertahankan tombol Kembali yang tetap terlihat. Pada ukuran 150%, baris membungkus dan pilihan dialog ikut membesar; jangan hanya memperbesar judul. Reduce Motion menghapus transisi kamera, bukan menghilangkan informasi hasil atau puzzle.

Credits dan Sumber memakai halaman baca dengan bagian tim, aset serta lisensi yang benar-benar digunakan, dan sumber tema GDD. Nama pembuat atau lisensi yang belum dipilih tidak diisi dengan contoh seolah sudah final. Halaman refleksi dan bantuan menjelaskan bahwa game bukan diagnosis atau nasihat hukum, lalu menyarankan orang tepercaya, konselor kampus, layanan kesehatan setempat, dan kanal resmi ketenagakerjaan. Tidak ada nomor bantuan rekaan atau tombol yang tampak terhubung ke layanan darurat.

Lembar visual 23 memperlihatkan struktur halaman, bukan daftar kredit aset yang sudah dipastikan. Semua halaman baca memakai tipografi dan komponen scroll yang sama dengan Catatan.

Pengenalan pertama mengikuti urutan baca gratis, buka/batal aktivitas, hasil satu aktivitas pilihan, pergantian sesi, draf CV, dan panel pengiriman. Setiap penjelasan memakai komponen yang sedang digunakan serta tetap dapat dibaca di Catatan. Uji menu, pilihan dialog, kartu, dan konfirmasi memakai keyboard pada teks 150%; tombol kembali harus tetap terlihat.

## 10. Motion and Feedback

Gerak objek cukup berupa buka/tutup panel, perpindahan kartu, dan rotasi pintu sederhana. Durasi transisi focus sekitar 0,2-0,4 detik; Reduce Motion menggantinya dengan pergantian langsung. Aktivitas menampilkan hasil melalui satu baris ringkasan yang dapat dibaca kembali.

Fade Retakan memakai gelap singkat tanpa kilatan putih. Tidak ada camera shake, head bob, zoom mendadak, atau animasi menghirup napas yang memaksa pemain mengikuti ritmenya. Suara benda memberikan respons interaksi tanpa perlu partikel atau efek layar.

Perubahan state kartu harus terlihat saat animasi dimatikan. Memilih jawaban atau memasang benda tidak boleh memerlukan ketepatan waktu.

## 11. Asset Workflow and Review

Buat blockout dari primitive Unity terlebih dahulu. Uji lebar jalur, jarak interaksi 2 meter, serta arah pandang 70 derajat sebelum membuat tekstur. Setelah itu, ganti satu kelompok aset pada satu waktu dengan model dari Blender atau alat setara, mempertahankan pivot dan ukuran.

Gunakan satu unit sebagai satu meter. Pivot pintu berada di engsel, pivot kursi dan meja berada di dasar, dan pivot kartu berada di tengah bidang. Objek statis menggunakan collider balok sederhana; benda tipis tidak membutuhkan MeshCollider detail. Nama aset membedakan model, material, dan tekstur secara konsisten, misalnya `PRP_Key07`, `MAT_Props`, dan `TEX_Props`.

Konten teks memiliki pemilik naskah tunggal. Ilustrator tidak menulis ulang syarat kontrak di gambar. Gunakan data teks yang sama pada laptop, Catatan, dan panel final agar tidak muncul versi berbeda.

Checklist visual sebelum build:

- Dari titik bangun, pemain dapat mengenali meja, jendela, dan pintu tanpa prompt tambahan.
- Nomor 07, kartu E0, dan dudukan pesan ibu terbaca tanpa bergantung pada suara atau warna.
- UI 150% tidak menutup tombol dan tetap bisa digulir pada 720p.
- Mode atmosfer Rendah tidak menghilangkan tujuan puzzle.
- Tidak ada aset bermerek, tulisan tiruan, atau detail yang berubah tanpa alasan.
- Bayangan tidak berkedip atau membuat jalur hilang pada waktu malam.
- Pengukuran frame time dilakukan di build Windows, bukan disimpulkan dari angka triangle atau tampilan editor.

Jika waktu tersisa sedikit, dahulukan siluet ruangan, kartu, dan keterbacaan UI. Detail kain, variasi buku, dan bekas permukaan dapat dikurangi tanpa mengubah identitas utama. Fitur inti dan tiga ending tidak dikurangi.

## 12. Mechanic Coverage and Concept Interpretation

### 12.1 Coverage Map

Nomor 00-11 mengacu pada ilustrasi konsep yang tersedia di [galeri](visuals/README.md). Nomor 12-25 mengacu pada ilustrasi mekanik JPG. Lembar tersebut memperlihatkan state alternatif atau urutan layar, bukan beberapa jendela yang harus muncul sekaligus di dalam game. Aturan serta teks implementasi mengikuti GDD dan bagian 9; tulisan pada gambar dapat berbeda dari revisi terbaru.

| GDD Section | Visual yang tersedia | Pelengkap mekanik dan spesifikasi |
| --- | --- | --- |
| 3.1 Rules and State Variables | 03: HUD | [12: HUD/Focus](visuals/12-hud-focus-states.jpg), bagian 9.4 |
| 3.2 Time and Scheduling | 02: varian waktu | [14: kalender/tidur](visuals/14-calendar-sleep.jpg), bagian 9.6 |
| 3.3 Actions | Benda di 01 dan laptop 04 | [13: panel aktivitas](visuals/13-activity-states.jpg), [24: aksi fisik](visuals/24-room-activities.jpg), [25: pekerjaan portofolio](visuals/25-portfolio-work.jpg), bagian 9.5 |
| 3.4 Pressure and Jeda | Suasana ruang normal/Retakan | [15: band kondisi dan Jeda](visuals/15-pressure-jeda.jpg), bagian 9.7 |
| 3.5 Evidence and CV | 05: papan bukti | [16: klaim/snapshot](visuals/16-cv-snapshot.jpg), bagian 9.8 |
| 3.6 Laptop and Verification | 04: lowongan dan 10: map | [17: verifikasi/jadwal](visuals/17-recruitment-verification.jpg), bagian 9.9 |
| 3.7 Interview and Results | 06: acuan potret | [18: dialog/hasil](visuals/18-interview-results.jpg), bagian 9.10 |
| 3.8 Retakan Ruang | 07, 08, 09: tiga ruang | [20: ambil/pasang/petunjuk](visuals/20-puzzle-help.jpg), bagian 9.12 dan 7 |
| 3.9 Controls and Camera | 01: perspektif | [12: Focus](visuals/12-hud-focus-states.jpg), [19: overlay](visuals/19-messages-notes.jpg), bagian 9.4 dan 9.11 |
| 3.10 Sound Consideration | Acuan ruang 01 dan 07-09 | [20: caption](visuals/20-puzzle-help.jpg), [23: audio](visuals/23-settings-reading.jpg), bagian 9.12 dan 9.15 |
| 3.11 Progression and Scoring | 05: bukti | [18: alasan hasil tanpa skor](visuals/18-interview-results.jpg), [21: refleksi](visuals/21-contract-decision.jpg) |
| 3.12 Ending Conditions | 10: map dan 11: epilog | [21: pilihan/peringatan/finalisasi](visuals/21-contract-decision.jpg), bagian 9.13 |
| 3.13 Screen Flow and Save | Belum ada layar khusus pada 00-11 | [22: menu/checkpoint](visuals/22-menu-save-states.jpg), bagian 9.14 |
| 3.14 Accessibility | Belum ada layar khusus pada 00-11 | [23: pengaturan](visuals/23-settings-reading.jpg), bagian 9.15 |
| 3.15 Production Plan | Bukan layar permainan | Komponen UI dipakai ulang; tidak menambah mekanik atau aset 3D wajib |

Pesan, kontak dukungan, serta fakta puzzle dilengkapi pada [19: Messages and Notes](visuals/19-messages-notes.jpg). Panel informasi konten, Credits, dan halaman bantuan dilengkapi pada lembar 23.

### 12.2 Interpretation of Existing Illustrations

Ilustrasi digunakan sebagai acuan warna, suasana, dan komposisi. Untuk implementasi, teks dan state mengikuti GDD serta bagian 9. Teks cerita, angka, dan tombol dibuat sebagai elemen UI yang dapat diperbesar. Perbedaan tulisan dicatat saat implementasi; revisi aturan tidak memerlukan pembuatan ulang gambar.

| Illustration | Penyesuaian saat implementasi |
| --- | --- |
| 00: Key Art | Komposisi terbelah dan kertas melayang adalah ilustrasi promosi; bukan tuntutan efek partikel atau retakan shader realtime |
| 03: HUD | Jumlah garis Tekanan harus tepat enam; tambahkan angka Tenaga/Tekanan. Warna oranye pada ilustrasi tidak berarti bahaya klinis |
| 04: Laptop | Status perusahaan diberi alasan hari/jadwal, bukan gembok tanpa konteks. Tambahkan penandaan empat aspek serta pemisahan baca gratis dan pemeriksaan (`2 jam`) |
| 05: Bukti | Ganti tulisan dekoratif dengan isi E0/E1/E2 yang benar: label input, pesan kesalahan, kolom kosong, format salah, dan data valid. Tidak perlu menyalin teks ilustrasi yang belum terbaca |
| 06: Potret | Ini lembar acuan karakter, bukan layar pemilihan perekrut. Interview hanya menampilkan Nara atau Bagas yang sesuai undangan |
| 07: Meja menjauh | Gunakan satu kasur dan panjangkan modul ruang; pengulangan kasur pada ilustrasi tidak menambah kebutuhan aset atau aturan puzzle |
| 08: Pintu bernama | Lorong tetap modul pendek yang ditetapkan GDD, bukan lorong tak terbatas. Kunci berada di bangku sebelum diambil, bukan sudah tergantung pada pintu tujuan |
| 09: Kursi kosong | Kursi tambahan tetap menyisakan jalur minimal 0,8 m; hindari susunan bertumpuk yang menjadi rintangan fisik. Kartu pesan ibu diletakkan pada meja/rak untuk diambil lalu dipasang di kursi jendela, bukan sudah terpasang di kursi sejak awal |
| 10: Map kontrak | Tulisan kode warna pada label bukan teks dalam game. Surat resmi hanya aktif jika sudah diperoleh; label risiko harus didukung syarat, bukan satu stempel saja |
| 11: Ending | Ini tiga acuan komposisi epilog, bukan layar pemilihan ending. Hasil diputuskan oleh kontrak final, bukan warna kamar atau nilai Tekanan |
| 12-20: Mekanik | Angka, pilihan interview, pratinjau Jeda, pengingat Tenaga, dan teks snapshot mengikuti aturan terbaru di GDD; panel tetap memakai ilustrasi sebagai acuan komposisi |
| 21: Keputusan | Tambahkan satu catatan batas kerja atau langkah pulang pada panel yang sama; rencana transisi tempat tinggal disampaikan lewat teks |
| 22-23: Menu dan baca | Lanjutkan sesudah epilog kembali ke keputusan hari 7; draf dan penandaan disimpan pada keadaan stabil; fokus keyboard tetap terlihat |
| 24-25: Aktivitas | Pemulihan memakai variasi teks terbatas; E1/E2 memiliki satu pilihan singkat melalui kartu tanpa editor kode atau tantangan mengetik |

Pelengkapan ini tidak menambahkan sistem hubungan, toko, nilai klinis, UI pekerjaan baru, atau fitur save manual. Tujuannya membuat mekanik yang sudah ada dapat diterjemahkan menjadi layar serta state yang konsisten.
