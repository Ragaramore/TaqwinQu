# TaqwinQu - Media Pembelajaran Interaktif Doa Harian

Aplikasi **TaqwinQu** adalah program komputer berbasis media pembelajaran interaktif yang dirancang khusus untuk memfasilitasi hafalan doa harian siswa jenjang TK B di lembaga PAUDQU At-Takwin. Aplikasi ini dikembangkan sebagai bentuk inovasi digital guna mengatasi keterbatasan metode hafalan konvensional di kelas serta menyediakan standar contoh pelafalan doa (Tahsin) yang konsisten dan selaras dengan target kurikulum lembaga.

## Fitur Utama Aplikasi
* **Eksplorasi Kontekstual (Point-and-Click):** Mekanisme penjelajahan ruangan simulasi (Masjid, Kamar Tidur, Kamar Rumah Sakit, dan Halaman Scene Taman) di mana pengguna dapat berinteraksi langsung mengeklik objek/karakter untuk menampilkan popup doa terkait.
* **Panel Konten Doa Terstruktur:** Menampilkan judul doa, sprite teks Arab, transliterasi Latin, serta teks terjemahan Bahasa Indonesia secara lengkap.
* **Kontrol Audio Non-Overlapping:** Logika sistem pemutar audio yang dilengkapi pelindung jeda waktu (*debounce*) untuk mencegah suara lafal doa saling bertabrakan bising saat tombol play ditekan secara berulang oleh anak usia dini.
* **Kuis Pilihan Ganda Interaktif:** Modul evaluasi sederhana terdiri atas 3 butir soal pilihan ganda (opsi jawaban A dan B) yang dilengkapi petunjuk audio (*voice over*) untuk menguji wawasan kontekstual siswa.

## Lingkungan Pengembangan
* **Game Engine:** Unity Engine
* **Bahasa Pemrograman:** C# (C-Sharp)
* **Code Editor:** Visual Studio Code
* **Arsitektur Data:** Unity ScriptableObject (Penyimpanan Data Lokal Offline)
* **Target Platform:** Perangkat Desktop Windows (.exe) Aspect Ratio 16:9
