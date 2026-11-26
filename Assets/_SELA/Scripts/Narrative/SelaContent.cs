using System.Collections.Generic;

namespace SELA.Narrative
{
    public enum MsgSender { Ibu, Dita, Perusahaan, AdminAksesPrima, Sistem }

    public sealed class Message
    {
        public MsgSender Sender;
        public string Title;
        public string Body;
        public bool IsNew = true;
        public bool HasLink;

        public Message(MsgSender sender, string title, string body, bool hasLink = false)
        {
            Sender = sender;
            Title = title;
            Body = body;
            HasLink = hasLink;
        }
    }

    /// <summary>Konteks hari 5-7: dua tawaran resmi, satu tawaran resmi, atau belum ada tawaran resmi.</summary>
    public enum OfferContext { DuaTawaran, SatuTawaran, BelumAdaTawaran }

    public enum EvidenceId { E0, E1, E2 }

    public enum OfferId { A, B, C }

    public enum ApplicationStatus { BelumDikirim, Terjadwal, Ditolak, Ditawari }

    public enum OfferChoice { TidakAda, A, B, C }

    public enum VerificationMark { Belum, Jelas, PerluDitanyakan, Berisiko }

    public enum SelaSession { Pagi, Siang, Malam }

    public enum SelaActivity
    {
        RapikanContohAntarmuka,
        LengkapiCatatanPengujian,
        PeriksaPerusahaan,
        KirimLamaran,
        IkutiInterview,
        Beristirahat,
        HubungiDita,
        DudukTanpaLayar,
        RapikanMeja
    }

    /// <summary>
    /// Satu-satunya sumber teks untuk seluruh UI dan cerita (GDD 3.15).
    /// Semua layar membaca dari sini; tidak ada teks cerita yang ditulis di file lain.
    /// </summary>
    public static partial class SelaContent
    {
        public const string GameTitle = "SELA";
        public const string Tagline = "Sebentar lagi harus menjawab. Belum tentu harus mengiyakan.";

        public static string SessionLabel(SelaSession session)
        {
            switch (session)
            {
                case SelaSession.Pagi: return "Pagi";
                case SelaSession.Siang: return "Siang";
                default: return "Malam";
            }
        }

        public static string DayLabel(int day)
        {
            return "Hari " + day;
        }

        public static string DaySessionLabel(int day, SelaSession session)
        {
            return "Hari " + day + " · " + SessionLabel(session);
        }

        public static string OfferName(OfferId offer)
        {
            switch (offer)
            {
                case OfferId.A: return "Studio Lajur";
                case OfferId.B: return "Koperasi Pijar";
                default: return "Akses Prima";
            }
        }

        public static string OfferTitle(OfferId offer)
        {
            switch (offer)
            {
                case OfferId.A: return "Junior UI Implementer";
                case OfferId.B: return "Web Support";
                default: return "Operator Digital";
            }
        }

        public static string EvidenceName(EvidenceId id)
        {
            switch (id)
            {
                case EvidenceId.E0: return "Proyek kuliah";
                case EvidenceId.E1: return "Antarmuka formulir";
                default: return "Catatan pengujian";
            }
        }

        public static string SenderLabel(MsgSender sender)
        {
            switch (sender)
            {
                case MsgSender.Ibu: return "Ibu";
                case MsgSender.Dita: return "Dita";
                case MsgSender.AdminAksesPrima: return "Admin Akses Prima";
                case MsgSender.Perusahaan: return "Perusahaan";
                default: return "Sistem";
            }
        }
    }
}
