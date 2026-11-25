using System;
using System.Collections.Generic;
using UnityEngine;

namespace SELA.Core
{
    [Serializable]
    public class CvDraftState
    {
        public ClaimChoice claim1 = ClaimChoice.Honest;
        public ClaimChoice claim2 = ClaimChoice.Honest;
        public bool attachE1;
        public bool attachE2;

        public bool HasExaggeratedClaim =>
            claim1 == ClaimChoice.Exaggerated || claim2 == ClaimChoice.Exaggerated;
    }

    [Serializable]
    public class ApplicationRecord
    {
        public CompanyId company;
        public ApplicationStatus status = ApplicationStatus.NotSent;
        public int submittedDay;
        public SessionType submittedSession;
        public int interviewDay;

        // Frozen snapshot captured at submission time (read-only afterward).
        public ClaimChoice snapshotClaim1;
        public ClaimChoice snapshotClaim2;
        public bool snapshotHasE1;
        public bool snapshotHasE2;

        // Interview outcome tracking.
        public bool claimsCorrectedInInterview;
        public int q1Score;
        public int q2Score;
        public bool missedInterview;
        public List<string> rejectionReasons = new List<string>();

        public bool SnapshotHasExaggeration =>
            snapshotClaim1 == ClaimChoice.Exaggerated || snapshotClaim2 == ClaimChoice.Exaggerated;

        public bool HasUncorrectedExaggeration =>
            SnapshotHasExaggeration && !claimsCorrectedInInterview;
    }

    [Serializable]
    public class VerificationRecord
    {
        public CompanyId company;
        public bool isVerified;
        public RiskTag tasksTag = RiskTag.Unmarked;
        public RiskTag compensationTag = RiskTag.Unmarked;
        public RiskTag workHoursTag = RiskTag.Unmarked;
        public RiskTag documentsTag = RiskTag.Unmarked;

        public RiskTag GetTag(VerificationAspect aspect)
        {
            switch (aspect)
            {
                case VerificationAspect.Tasks: return tasksTag;
                case VerificationAspect.Compensation: return compensationTag;
                case VerificationAspect.WorkHoursAndRest: return workHoursTag;
                case VerificationAspect.PersonalDocuments: return documentsTag;
                default: return RiskTag.Unmarked;
            }
        }

        public void SetTag(VerificationAspect aspect, RiskTag tag)
        {
            switch (aspect)
            {
                case VerificationAspect.Tasks: tasksTag = tag; break;
                case VerificationAspect.Compensation: compensationTag = tag; break;
                case VerificationAspect.WorkHoursAndRest: workHoursTag = tag; break;
                case VerificationAspect.PersonalDocuments: documentsTag = tag; break;
            }
        }
    }

    [Serializable]
    public class GameState
    {
        public const int CurrentSaveVersion = 1;
        public const int MinDay = 1;
        public const int MaxDay = 7;
        public const int MinEnergy = 0;
        public const int MaxEnergy = 4;
        public const int MinPressure = 0;
        public const int MaxPressure = 6;

        public int saveVersion = CurrentSaveVersion;
        public int day = 1;
        public SessionType session = SessionType.Pagi;
        public bool sessionResolved;
        public GamePhase phase = GamePhase.FreeRoam;

        public int energy = 3;
        public int pressure = 1;

        // Evidence set: E0 available from start, E1 & E2 added without removing E0.
        public bool hasE0 = true;
        public bool hasE1;
        public bool hasE2;

        public CvDraftState cvDraft = new CvDraftState();

        // Applications for A (Studio Lajur) and B (Koperasi Pijar).
        public ApplicationRecord applicationA = new ApplicationRecord { company = CompanyId.StudioLajur };
        public ApplicationRecord applicationB = new ApplicationRecord { company = CompanyId.KoperasiPijar };

        // Verification records for A, B, and C (Akses Prima).
        public VerificationRecord verificationA = new VerificationRecord { company = CompanyId.StudioLajur };
        public VerificationRecord verificationB = new VerificationRecord { company = CompanyId.KoperasiPijar };
        public VerificationRecord verificationC = new VerificationRecord { company = CompanyId.AksesPrima };

        public OfferChoice offerChoice = OfferChoice.None;
        public DecisionNoteType decisionNote = DecisionNoteType.None;
        public EndingType ending = EndingType.None;

        // Retakan Ruang progress (indices 0, 1, 2 for Rifts 1, 2, 3).
        public bool[] riftSolved = new bool[3];
        public int activeRiftIndex; // 0 = none, 1..3 = active rift

        // Daily flags (reset on sleep).
        public bool deskTidiedToday;
        public bool jedaTriggeredToday;
        public bool ditaCalledToday;

        // Track desk visual state & read messages.
        public bool isDeskTidy;
        public List<string> readMessageIds = new List<string>();
        public int sitObservationsSeen;
        public int tidyObservationsSeen;

        public static GameState CreateInitial()
        {
            return new GameState();
        }

        public GameState Clone()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<GameState>(json);
        }

        public void ClampStats()
        {
            day = Mathf.Clamp(day, MinDay, MaxDay);
            energy = Mathf.Clamp(energy, MinEnergy, MaxEnergy);
            pressure = Mathf.Clamp(pressure, MinPressure, MaxPressure);
        }

        public bool HasEvidence(EvidenceId id)
        {
            switch (id)
            {
                case EvidenceId.E0: return hasE0;
                case EvidenceId.E1: return hasE1;
                case EvidenceId.E2: return hasE2;
                default: return false;
            }
        }

        public void AddEvidence(EvidenceId id)
        {
            switch (id)
            {
                case EvidenceId.E0: hasE0 = true; break;
                case EvidenceId.E1: hasE1 = true; break;
                case EvidenceId.E2: hasE2 = true; break;
            }
        }

        public ApplicationRecord GetApplication(CompanyId company)
        {
            switch (company)
            {
                case CompanyId.StudioLajur: return applicationA;
                case CompanyId.KoperasiPijar: return applicationB;
                default: return null;
            }
        }

        public VerificationRecord GetVerification(CompanyId company)
        {
            switch (company)
            {
                case CompanyId.StudioLajur: return verificationA;
                case CompanyId.KoperasiPijar: return verificationB;
                case CompanyId.AksesPrima: return verificationC;
                default: return null;
            }
        }

        public bool HasOffer(CompanyId company)
        {
            switch (company)
            {
                case CompanyId.StudioLajur: return applicationA.status == ApplicationStatus.Offered;
                case CompanyId.KoperasiPijar: return applicationB.status == ApplicationStatus.Offered;
                case CompanyId.AksesPrima: return day >= 4;
                default: return false;
            }
        }

        public bool HasScheduledInterviewOn(int targetDay, out CompanyId scheduledCompany)
        {
            if (applicationA.status == ApplicationStatus.Scheduled && applicationA.interviewDay == targetDay)
            {
                scheduledCompany = CompanyId.StudioLajur;
                return true;
            }
            if (applicationB.status == ApplicationStatus.Scheduled && applicationB.interviewDay == targetDay)
            {
                scheduledCompany = CompanyId.KoperasiPijar;
                return true;
            }
            scheduledCompany = CompanyId.StudioLajur;
            return false;
        }

        public bool IsNightFinished => session == SessionType.Malam && sessionResolved;

        public bool IsRiftSolved(int riftNumber)
        {
            if (riftNumber < 1 || riftNumber > 3 || riftSolved == null || riftSolved.Length < 3)
                return false;
            return riftSolved[riftNumber - 1];
        }

        public void MarkRiftSolved(int riftNumber)
        {
            if (riftSolved == null || riftSolved.Length < 3)
                riftSolved = new bool[3];
            if (riftNumber >= 1 && riftNumber <= 3)
                riftSolved[riftNumber - 1] = true;
        }
    }
}
