# SELA – Paket Aset FBX (tema "Cat Poster Kos")

Dibuat dari GDD SELA dengan geometri low-poly, material matte, dan palet warna bersama.

## Isi
- `Art/Environments/` dan `Art/Props/` : model FBX, dikelompokkan berdasarkan fungsi.
- `Art/Textures/T_SELA_Palette.png` : atlas palet 1024x1024 (8x8 sel, 24 warna terpakai).
- `Art/Textures/Preview/` : render cek tata letak.
- `Scripts/Editor/PaletteSetup.cs` : script editor untuk pengaturan material.

## Set meja Cat Poster

`SM_Laptop.fbx`, `SM_Meja.fbx`, `SM_Papan_Bukti.fbx`, dan `SM_Kursi_Jendela.fbx` memakai model Cat Poster di path asli. Keempatnya berbagi `Art/Materials/M_CatPoster_DeskSet.mat` dan atlas `Art/Textures/T_CatPoster_DeskSet.png`. Objek yang merujuk FBX ini di `KosV2_PlayerTest`, `kamar2`, dan `progress` memakai model baru. Mesh yang tertanam dalam FBX rakitan kamar tidak ikut berubah.

Sumber Blender ada di `SourceArt/Laptop/SM_Laptop.blend` dan `SourceArt/DeskSet/DeskSet.blend`. `SM_Meja_Laci` tetap terpisah, `SM_Kursi_Jendela_Dudukan_Kartu` tetap tersedia, dan `SM_Laptop_Display` adalah bidang kosong yang ikut bergerak dengan engsel layar.

## Kasur, tirai, dan tas Cat Poster

`SM_Kasur.fbx`, `SM_Tirai.fbx`, dan `SM_Tas.fbx` juga diganti di path asli dengan atlas Cat Poster yang sama. Dipan, kasur, bantal, dan selimut tetap terpisah. Tirai dan tas memakai detail jahitan pada tekstur, bukan mesh tambahan. Sumber Blender: `SourceArt/SoftGoods/RoomTextiles.blend`.

## Pintu, jendela, dan kunci 07 Cat Poster

Empat varian pintu, enam ukuran jendela, dan `SM_Kunci_07` memakai `Art/Materials/M_CatPoster_Architecture.mat` dan atlas `Art/Textures/T_CatPoster_Architecture.png`. Engsel `*_Panel`, pintu ganda jendela, dan plat nomor tetap terpisah; plat dibiarkan tanpa teks agar label dapat diganti per scene. Sumber Blender: `SourceArt/Openings/DoorsWindowsKey.blend`.

## Properti cerita dan variasi material

`SM_Ponsel`, `SM_Kalender`, `SM_Map_Tawaran`, `SM_Kartu_Bukti`, `SM_Kartu_Pesan_Ibu`, dan `SM_Tiket` diganti di path asli. Model ini memakai ruang kosong di atlas `T_CatPoster_Architecture.png`, tanpa mengubah UV pintu atau jendela. Layar ponsel, bidang label, dan area isi dokumen tidak memuat teks cerita. Sumber Blender: `SourceArt/StoryProps/StoryProps.blend`.

Ikuti keluarga warna di `Docs/DESIGN.MD`, tetapi bedakan permukaan menurut bahannya: meja bercat hijau kelabu dengan bekas aus, kursi kayu madu dengan arah serat, papan bukti berlapis kertas pucat dengan bekas selotip, tas kanvas zaitun dengan jahitan, dan ponsel bercasing tanah liat. Laptop tetap memakai tekstur dan warna sebelumnya; jangan menyalin motif panel plastiknya ke kayu, kain, atau kertas.

## Finishing ruang Cat Poster

Kipas berlapis enamel hijau kelabu, lampu meja berwarna oker, lampu plafon dan neon berwarna krem, rak kayu, serta dua kelompok buku memakai `Art/Materials/M_CatPoster_RoomFinish.mat` dan atlas `Art/Textures/T_CatPoster_RoomFinish.png`. Atlas ini juga dipakai oleh modul lantai, dinding, dan plafon kamar serta lorong. Denah, bukaan, titik lampu, dan pivot kipas tetap sama. Sumber Blender: `SourceArt/RoomFinish/RoomFinish.blend`.

## Properti utilitas dan sambungan ruang

Penjepit foto kuning dan lembar penolakan tanpa teks untuk Retakan 1 memakai `Art/Materials/M_CatPoster_Utility.mat`. Material ini juga dipakai untuk perlengkapan kamar mandi, saklar, sandal, kabel sambungan, dan bangku lorong. Sumber Blender: `SourceArt/Utility/Utility.blend`.

Keempat varian `SM_Pintu` memiliki gagang di kedua sisi daun pintu dan lis penahan pada kusen. Gagang mengikuti gerak daun pintu karena menyatu dengan mesh `_Panel`; pivot dan acuan interaksi tetap sama. `KosV2_PlayerTest`, `kamar2`, dan `progress` memakai model terpisah, bukan FBX rakitan lama. Gunakan model terpisah saat menata scene baru agar perubahan aset ikut diterapkan.

## Kenapa fleksibel di Unity
1. **Material bersama**: model awal memakai `M_SELA_Palette` dengan tiap face mengambil satu warna. Set meja Cat Poster memakai atlas terpisah agar detail yang dilukis tidak memengaruhi model awal.
2. **Modular**: kamar dipecah jadi Floor, Ceiling, Wall_N, Wall_S_Door, Wall_E_Window, Wall_W. Bisa dihapus/diganti per bagian.
3. **Bagian bergerak sudah terpisah** dengan pivot di engsel: `SM_Pintu_Panel` (engsel kiri), `SM_Meja_Laci`, `SM_Laptop_Screen` (terbuka 105 derajat, rotasi bisa diubah), `SM_Jendela_Sash_L/R`, `SM_Kipas_Baling` (pivot di hub, putar sumbu depan).
4. **Slot teks/UI** berupa mesh datar terpisah bernama `*_Label*` / `Label_Plane` (kalender, tiket, map, kartu ibu, plat pintu, label kunci 07) untuk TextMeshPro atau tekstur sendiri.
5. **Jangkar puzzle** sudah ada: `SM_Penjepit_Sudut_Foto` (Retakan 1), `SM_Kunci_07` + `SM_Pintu` (Retakan 2), `SM_Kursi_Jendela_Dudukan_Kartu` (Retakan 3).
6. Skala 1 unit = 1 meter. Pivot umumnya di tengah-bawah. Arah depan aset = sisi tempat pemain berdiri/duduk.
7. Ukuran kamar sesuai GDD: interior 3 x 4 m, tinggi 2,8 m; lorong 6 x 1,5 m; kursi/meja/kasur skala nyata. `SM_Kamar07_Shell_Retakan1_Panjang7m` = kamar 7 m untuk Retakan 1.

## Daftar aset
Ruang: SM_Kamar07_Shell, SM_Kamar07_Shell_Retakan1_Panjang7m, SM_Lorong_Modul, SM_Kota_Siluet, SM_Jendela, SM_Pintu (+ _Plat_Hijau, _Plat_Merah)
Furnitur: SM_Meja, SM_Kasur (Dipan, Kasur, Bantal, Selimut), SM_Kursi_Jendela, SM_Rak, SM_Bangku_Lorong, SM_Tirai
Interaktif: SM_Papan_Bukti, SM_Kartu_Bukti (E0/E1/E2), SM_Laptop, SM_Ponsel, SM_Kalender, SM_Map_Tawaran, SM_Kunci_07, SM_Kartu_Pesan_Ibu, SM_Tiket, SM_Tas, SM_Penjepit_Sudut_Foto, SM_Lembar_Penolakan
Dekor: SM_Lampu_Meja, SM_Kipas, SM_Sandal, SM_Kabel_Sambungan, SM_Buku_Tumpukan, SM_Buku_Berdiri
Rakitan: **SM_Kamar07_FullScene** (semua aset sudah ditata sesuai GDD: meja utara, kasur barat, pintu selatan, jendela+kursi timur), **SM_Lorong_Assembled** (3 pintu + bangku)

## Pengaturan di Unity
1. Aset proyek berada di `Assets/_SELA/`; aset pihak ketiga berada di `Assets/ThirdParty/`.
2. Cara cepat: jalankan menu **SELA > Setup Palette Material**.
3. Cara manual: buat material, pasang `T_SELA_Palette` sebagai Base Map, Smoothness 0. Di Inspector tiap FBX: tab Materials > Extract/Remap `M_SELA_Palette` ke material itu. Texture: Filter Mode **Point**, Generate Mip Maps **off**.
4. Model: Scale Factor 1, Convert Units on. Jika ada aset yang menghadap terbalik, putar 180 derajat pada prefab.
5. Tambahkan collider sendiri (sengaja tidak dibuat, sesuai GDD collider disusun manual).

## Catatan jujur
- Karena `DESIGN.MD` tidak ikut diupload, palet warna dan bentuk saya tentukan sendiri dari deskripsi GDD ("Cat Poster Kos"). Kalau DESIGN.MD punya kode warna resmi, cukup ubah PNG palet.
- Saya tidak bisa menjalankan Unity di sini, jadi impor ke Unity belum diuji langsung (preview dirender dari Blender).
- Arsiran bayangan "dilukis pada tekstur" belum ada; sekarang warna datar, arsiran bisa ditambah lewat shader toon atau tekstur.
- Karakter 2D, UI, dan audio tidak termasuk (bukan aset 3D).

## Script interaksi Unity
Script interaksi berada di `Scripts/Interaction/`, dengan script pemain di `Scripts/Player/` dan script UI di `Scripts/UI/`.
Dibuat tanpa bisa diuji di Unity; periksa Console jika ada error kompilasi.

## Update: Kamar V2 (lebih lega + kamar mandi dalam)
Interior 3,6 x 4,6 m (GDD asli 3 x 4 m). Kamar mandi dalam 1,3 x 1,8 m di sudut barat daya, dekat pintu masuk.
FBX baru: SM_Kos_Kamar07_V2_FullScene (rakitan lengkap), SM_Kamar07_V2_Shell_KM, SM_Pintu_KM, SM_Kloset_Jongkok,
SM_Bak_Mandi (+ gayung terpisah), SM_Ember, SM_Shower, SM_Gantungan_Handuk (rak, handuk, cermin).

## Perbaikan: scene rakitan tampak berputar
Penyebab: opsi "Apply Transform" saat ekspor FBX salah menghitung hierarki bertingkat (root > aset > mesh).
Sudah diekspor ulang tanpa opsi itu: SM_Kamar07_FullScene, SM_Kos_Kamar07_V2_FullScene, SM_Lorong_Assembled,
SM_Pintu, SM_Pintu_Plat_Hijau, SM_Pintu_Plat_Merah, SM_Pintu_KM.
Konsekuensi: di Unity, root file-file ini berotasi X = -90. Itu NORMAL, tampilannya tetap tegak. Jangan di-reset ke 0.
Setelah diseret ke scene, klik kanan > Prefab > Unpack Completely bila ingin memindahkan benda satu per satu.
Hasil diverifikasi dengan impor ulang di Blender (0 mesh salah posisi).

## Ruang kosong + view, perabot dipisah
- SM_Kos_V2_Ruang_Kosong_View : cangkang kamar V2, sekat KM, pintu, jendela, siluet kota (tanpa perabot)
- SM_Kos_V2_Isi_Perabot       : semua perabot dengan posisi sesuai layout V2 (tanpa ruangan)
Keduanya memakai titik nol yang sama, jadi bisa ditumpuk lalu perabot dipindah bebas.

## Dipisah total: bangunan, view, jendela, pintu (titik nol sama, tinggal ditumpuk)
- SM_Kos_V2_Bangunan_Kosong   : dinding/lantai/plafon/sekat KM saja (lubang pintu dan jendela dibiarkan kosong)
- SM_Kos_V2_View_Kota         : siluet kota saja
- SM_Kos_V2_Jendela_Terpasang : jendela dengan dua daun terpisah (Sash_L, Sash_R) pada posisi lubang
- SM_Kos_V2_Pintu_Terpasang   : pintu masuk dan pintu KM, daun pintu terpisah dengan pivot di engsel
