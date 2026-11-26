using System;
using SELA.Core;

namespace SELA.Narrative
{
    public sealed class InterviewDialogue
    {
        readonly int firstQuestion;
        public DialogueSession Session { get; }

        internal InterviewDialogue(DialogueLine opening, DialogueLine correction, DialogueLine question1, DialogueLine question2)
        {
            firstQuestion = correction == null ? 1 : 2;
            Session = correction == null
                ? new DialogueSession(opening, question1, question2)
                : new DialogueSession(opening, correction, question1, question2);
        }

        public bool TryGetAnswers(out InterviewInput input)
        {
            input = default;
            if (!Session.IsComplete) return false;

            input.CorrectExaggeratedClaims = firstQuestion == 2 && Session.AnswerAt(1) == 0;
            input.Question1ChoiceIndex = Session.AnswerAt(firstQuestion);
            input.Question2ChoiceIndex = Session.AnswerAt(firstQuestion + 1);
            return true;
        }
    }

    public static class NarrativeContent
    {
        public static readonly DialogueLine MorningThought = new DialogueLine("Raka", "Mulai dari mana, ya? Contoh formulir di papan belum kurapikan.");
        public static readonly DialogueLine EvidenceThought = new DialogueLine("Raka", "Sekarang ada contoh yang bisa kutunjukkan. Jangan lupa lampirkan di CV.");
        public static readonly DialogueLine MotherGreeting = new DialogueLine("Ibu", "Sudah sarapan, Rak? Kabari Ibu kalau sempat, ya.");
        public static readonly DialogueLine MotherMessage = new DialogueLine("Ibu", "Tiketnya tetap boleh dipakai, Rak. Kalau sudah tahu mau berangkat kapan, kabari Ibu.");

        static readonly DialogueLine NaraOpening = new DialogueLine("Nara", "Halo, Raka. Saya Nara dari Studio Lajur. Kita bahas contoh formulir dan cara kamu bekerja.");
        static readonly DialogueLine NaraQuestion1 = new DialogueLine("Nara", "Di formulir itu, apa yang kamu ubah dan bagaimana kamu memeriksanya?",
            new DialogueChoice("Saya ubah label dan pesan kesalahan. Di E1 terlihat bedanya sebelum dan sesudah saya coba."),
            new DialogueChoice("Labelnya perlu lebih spesifik. Saya cek lagi apakah orang bisa mengerti maksudnya."),
            new DialogueChoice("Saya berhasil mengirim formulir sekali. Menurut saya itu sudah cukup."));

        static readonly DialogueLine NaraQuestion2 = new DialogueLine("Nara", "Kalau ada bagian pekerjaan yang belum kamu kuasai, apa yang kamu lakukan?",
            new DialogueChoice("Saya jelaskan dulu bagian yang bisa saya kerjakan sendiri, lalu minta pendampingan untuk bagian baru."),
            new DialogueChoice("Boleh lihat satu contoh tugasnya sebelum saya memperkirakan apa yang perlu saya pelajari?"),
            new DialogueChoice("Saya terima semua tugas dulu. Kalau perlu bantuan, saya bahas setelah tenggat."));

        static readonly DialogueLine BagasOpening = new DialogueLine("Bagas", "Siang, Raka. Saya Bagas dari Koperasi Pijar. Kita mulai dari catatan uji formulirmu.");
        static readonly DialogueLine BagasQuestion1 = new DialogueLine("Bagas", "Dari pengujian formulir itu, apa yang bisa kamu simpulkan?",
            new DialogueChoice("Di E2 saya catat kolom kosong, format salah, dan data valid. Kesimpulannya terbatas pada tiga kasus itu."),
            new DialogueChoice("Kalau formulirnya diperbaiki lagi, hasilnya perlu dicek ulang."),
            new DialogueChoice("Data valid berhasil dikirim, jadi formulirnya siap untuk semua kondisi."));

        static readonly DialogueLine BagasQuestion2 = new DialogueLine("Bagas", "Kalau beberapa laporan soal formulir masuk bersamaan, kamu mulai dari mana?",
            new DialogueChoice("Saya pastikan prioritas dan lingkupnya dulu, lalu cek apakah perubahan berdampak pada alur lain."),
            new DialogueChoice("Saya minta satu contoh laporan yang paling mendesak."),
            new DialogueChoice("Saya langsung ubah formulir sesuai urutan laporan masuk."));

        static readonly DialogueLine Correction = new DialogueLine("Raka", "Ada klaim di CV yang melebih-lebihkan pengalamanku. Perlu kuluruskan sekarang?",
            new DialogueChoice("Saya mau meluruskan CV saya. Saya membantu antarmuka dan menguji formulir secara manual, sebatas kasus yang saya catat."),
            new DialogueChoice("Langsung ke pertanyaan saja."));

        public static InterviewDialogue StartInterview(ApplicationRecord application)
        {
            var correction = application.SnapshotHasExaggeration ? Correction : null;
            switch (application.company)
            {
                case CompanyId.StudioLajur:
                    return new InterviewDialogue(NaraOpening, correction, NaraQuestion1, NaraQuestion2);
                case CompanyId.KoperasiPijar:
                    return new InterviewDialogue(BagasOpening, correction, BagasQuestion1, BagasQuestion2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(application));
            }
        }

        static readonly DialogueLine DitaAtDoor = new DialogueLine("Dita", "Eh, Rak. Masuk aja. Lagi ada yang pengin kamu ceritain?");
        public static readonly DialogueLine DitaMessage = new DialogueLine("Dita", "Pagi, Rak. Aku di Kamar 06 kalau mau mampir.");
        static readonly DialogueLine DitaReply = new DialogueLine("Dita", "Mau lanjut yang tadi, Rak? Aku masih bisa dengerin.");
        static readonly DialogueLine DitaCheckIn = new DialogueLine("Raka", "Aku mau mulai dari mana?",
            new DialogueChoice("Aku jadi ragu soal contoh kerja yang mau kukirim.",
                new DialogueLine("Dita", "Coba mulai dari bagian yang kamu kerjakan sendiri. Yang belum sempat kamu cek juga boleh ditulis apa adanya.")),
            new DialogueChoice("Boleh dengerin dulu? Aku belum pengin cari jalan keluarnya.",
                new DialogueLine("Dita", "Boleh. Aku hampir langsung kasih saran tadi. Cerita aja, Rak.")));

        public static DialogueSession StartDitaConversation(bool alreadyTalkedToday)
        {
            return alreadyTalkedToday
                ? new DialogueSession(DitaReply)
                : new DialogueSession(DitaAtDoor, DitaCheckIn);
        }
    }
}