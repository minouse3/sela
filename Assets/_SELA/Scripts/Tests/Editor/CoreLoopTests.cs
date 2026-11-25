using System.IO;
using NUnit.Framework;
using SELA.Core;

namespace SELA.Core.Tests
{
    public class CoreLoopTests
    {
        string tempSaveDir;
        SaveSystem saveSystem;
        GameLoopEngine engine;

        [SetUp]
        public void SetUp()
        {
            tempSaveDir = Path.Combine(Path.GetTempPath(), "SELA_CoreTests_" + System.Guid.NewGuid().ToString("N"));
            saveSystem = new SaveSystem(tempSaveDir);
            engine = new GameLoopEngine(saveSystem);
            engine.StartNewGame(saveInitialCheckpoints: true);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempSaveDir))
                Directory.Delete(tempSaveDir, true);
        }

        [Test]
        public void RecoveryOnlyPlaythrough_ReachesDay7AndPulangEndingWithoutSoftlock()
        {
            for (int day = 1; day <= 7; day++)
            {
                Assert.AreEqual(day, engine.State.day);
                Assert.AreEqual(SessionType.Pagi, engine.State.session);

                // Pagi
                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
                Assert.AreEqual(SessionType.Siang, engine.State.session);

                // Siang (Days 2, 4, 6 trigger Retakan Ruang)
                var siangRes = engine.ExecuteActivity(new ActivityRequest(ActivityType.SitWithoutScreen));
                Assert.IsTrue(siangRes.Executed);
                if (day == 2 || day == 4 || day == 6)
                {
                    Assert.AreEqual(GamePhase.InRift, engine.State.phase);
                    Assert.AreEqual(day / 2, engine.State.activeRiftIndex);
                    Assert.IsTrue(engine.ResolveRift(skippedViaHelp: true));
                }
                Assert.AreEqual(SessionType.Malam, engine.State.session);

                // Malam
                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.CallDita)).Executed);
                Assert.IsTrue(engine.State.IsNightFinished);

                // Cannot buy another session once night is finished
                var blocked = engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest));
                Assert.IsFalse(blocked.Executed);

                // Sleep or open Night 7 Final Decision
                Assert.IsTrue(engine.SleepOrOpenFinalDecision());
            }

            Assert.AreEqual(GamePhase.FinalDecision, engine.State.phase);
            Assert.IsTrue(engine.SetDecisionNote(DecisionNoteType.HomeOrganizeEvidence));
            Assert.IsTrue(engine.ConfirmFinalDecision());
            Assert.AreEqual(EndingType.PulangBukanSelesai, engine.State.ending);
            Assert.AreEqual(GamePhase.Epilogue, engine.State.phase);

            // GDD §3.13: Continue after epilogue returns to Day 7 Night pre-finalization state.
            var reload = engine.LoadAutosave();
            Assert.IsTrue(reload.Success);
            Assert.AreEqual(7, engine.State.day);
            Assert.AreEqual(SessionType.Malam, engine.State.session);
            Assert.IsTrue(engine.State.IsNightFinished);
            Assert.AreEqual(GamePhase.FreeRoam, engine.State.phase);
        }

        [Test]
        public void CvSnapshot_IsImmutableAfterSubmission_AndInterviewRespectsClaimCorrection()
        {
            // Day 1 Pagi: get E1
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.ImproveUiExample)).Executed);
            Assert.IsTrue(engine.State.hasE1);

            // Attach E1 with exaggerated claim 1, then submit Application A on Day 1 Siang
            engine.UpdateCvDraft(ClaimChoice.Exaggerated, ClaimChoice.Honest, attachE1: true, attachE2: false);
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.SubmitApplication, CompanyId.StudioLajur)).Executed);

            // Edit CV draft afterward -> must NOT mutate submitted snapshot
            engine.UpdateCvDraft(ClaimChoice.Honest, ClaimChoice.Honest, attachE1: false, attachE2: false);
            var appA = engine.State.GetApplication(CompanyId.StudioLajur);
            Assert.AreEqual(ApplicationStatus.Scheduled, appA.status);
            Assert.AreEqual(2, appA.interviewDay);
            Assert.AreEqual(ClaimChoice.Exaggerated, appA.snapshotClaim1);
            Assert.IsTrue(appA.snapshotHasE1);

            // Advance to Day 2 Siang
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
            Assert.IsTrue(engine.SleepOrOpenFinalDecision());
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
            Assert.AreEqual(2, engine.State.day);
            Assert.AreEqual(SessionType.Siang, engine.State.session);

            // Attend Interview A: correct exaggerated claim + good answers -> Offered!
            var intRes = engine.ExecuteActivity(
                new ActivityRequest(ActivityType.AttendInterview, CompanyId.StudioLajur),
                new InterviewInput { CorrectExaggeratedClaims = true, Question1ChoiceIndex = 0, Question2ChoiceIndex = 0 });

            Assert.IsTrue(intRes.Executed);
            Assert.IsTrue(intRes.InterviewOutcome.HasValue && intRes.InterviewOutcome.Value.Offered);
            Assert.AreEqual(ApplicationStatus.Offered, appA.status);
            Assert.IsTrue(appA.claimsCorrectedInInterview);
            Assert.AreEqual(ClaimChoice.Exaggerated, appA.snapshotClaim1); // original snapshot claim preserved

            // Day 2 Siang should now trigger Retakan 1
            Assert.AreEqual(GamePhase.InRift, engine.State.phase);
            Assert.AreEqual(1, engine.State.activeRiftIndex);
        }

        [Test]
        public void MissingScheduledInterview_MarksRejectedAndAddsOnePressureOnce()
        {
            // Day 1 Pagi: submit A without E1 (schedules Day 2 Siang)
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.SubmitApplication, CompanyId.StudioLajur)).Executed);
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
            Assert.IsTrue(engine.SleepOrOpenFinalDecision());

            // Day 2 Pagi: rest -> Day 2 Siang (pressure is 0)
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
            Assert.AreEqual(0, engine.State.pressure);

            // Replace Day 2 Siang interview with VerifyCompany (base pressure +0, missed interview +1)
            var preview = engine.GetPreview(new ActivityRequest(ActivityType.VerifyCompany, CompanyId.StudioLajur));
            Assert.IsTrue(preview.Availability.WillMissScheduledInterview);
            Assert.AreEqual(1, preview.PressureAfter);

            var res = engine.ExecuteActivity(new ActivityRequest(ActivityType.VerifyCompany, CompanyId.StudioLajur));
            Assert.IsTrue(res.Executed);
            Assert.IsTrue(res.MissedInterview);
            Assert.AreEqual(ApplicationStatus.Rejected, engine.State.applicationA.status);
            Assert.IsTrue(engine.State.applicationA.missedInterview);
            Assert.AreEqual(1, engine.State.pressure);
        }

        [Test]
        public void JedaTriggersOncePerDayAtPressure6_AndDoesNotRepeatSameDay()
        {
            // Push pressure to 5 manually via state setup on Day 1 Pagi
            engine.State.pressure = 5;
            engine.State.energy = 1;

            // Activity adding +1 pressure hits 6 -> triggers Jeda
            var res = engine.ExecuteActivity(new ActivityRequest(ActivityType.ImproveUiExample));
            Assert.IsTrue(res.Executed);
            Assert.IsTrue(res.TriggeredJeda);
            Assert.AreEqual(GamePhase.AwaitingJeda, engine.State.phase);

            // Resolve Jeda -> pressure becomes 4, energy at least 1, session advances to Siang
            Assert.IsTrue(engine.ResolveJeda(JedaChoice.SitBriefly));
            Assert.AreEqual(4, engine.State.pressure);
            Assert.GreaterOrEqual(engine.State.energy, 1);
            Assert.IsTrue(engine.State.jedaTriggeredToday);
            Assert.AreEqual(SessionType.Siang, engine.State.session);

            // Hit pressure 6 again on the same day -> stays 6 without re-triggering Jeda
            engine.State.pressure = 5;
            var res2 = engine.ExecuteActivity(new ActivityRequest(ActivityType.SubmitApplication, CompanyId.StudioLajur));
            Assert.IsTrue(res2.Executed);
            Assert.IsFalse(res2.TriggeredJeda);
            Assert.AreEqual(6, engine.State.pressure);
            Assert.AreEqual(GamePhase.FreeRoam, engine.State.phase);
        }

        [Test]
        public void SaveSystem_FallsBackToDayStart_WhenAutosaveCorrupted()
        {
            // Corrupt autosave file while keeping valid day_start checkpoint
            File.WriteAllText(saveSystem.AutosavePath, "{ broken json");
            var loaded = engine.LoadAutosave(allowFallback: true);

            Assert.IsTrue(loaded.Success);
            Assert.IsTrue(loaded.LoadedFromDayStartFallback);
            Assert.AreEqual(1, engine.State.day);
            Assert.AreEqual(SessionType.Pagi, engine.State.session);
        }

        [Test]
        public void RiskyOfferC_RequiresAcknowledgementAndResolvesToKontrakYangMengunci()
        {
            // Fast-forward state to Day 7 Night finished
            engine.State.day = 7;
            engine.State.session = SessionType.Malam;
            engine.State.sessionResolved = true;

            Assert.IsTrue(engine.SleepOrOpenFinalDecision());
            Assert.IsTrue(engine.SetOfferChoice(OfferChoice.AksesPrima));

            // Cannot confirm without acknowledging risky terms
            Assert.IsFalse(engine.ConfirmFinalDecision());

            Assert.IsTrue(engine.SetDecisionNote(DecisionNoteType.RiskyOfferAcknowledged));
            Assert.IsTrue(engine.ConfirmFinalDecision());
            Assert.AreEqual(EndingType.KontrakYangMengunci, engine.State.ending);
        }
    }
}
