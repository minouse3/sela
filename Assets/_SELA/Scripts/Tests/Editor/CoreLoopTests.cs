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
        public void CalmPlaythrough_KeepsPressureLow_AvoidsAllRifts_AndReachesPulangEnding()
        {
            for (int day = 1; day <= 7; day++)
            {
                Assert.AreEqual(day, engine.State.day);
                Assert.AreEqual(GameState.StartHour, engine.State.currentHour);
                Assert.AreEqual(SessionType.Pagi, engine.State.session);

                // 08:00 -> 10:00 (Pagi), 10:00 -> 12:00 (Siang), 12:00 -> 14:00 (Siang),
                // 14:00 -> 16:00 (Siang), 16:00 -> 18:00 (Malam) -> Sleep early!
                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
                Assert.AreEqual(10, engine.State.currentHour);
                Assert.AreEqual(SessionType.Pagi, engine.State.session);

                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);
                Assert.AreEqual(12, engine.State.currentHour);
                Assert.AreEqual(SessionType.Siang, engine.State.session);

                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Shower)).Executed); // 12->13
                Assert.IsTrue(engine.State.showeredToday);
                Assert.IsFalse(engine.ExecuteActivity(new ActivityRequest(ActivityType.Shower)).Executed); // max 1x/day

                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.CallDita)).Executed); // 13->15
                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.Rest)).Executed);     // 15->17
                Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.SitWithoutScreen)).Executed); // 17->18 (Malam)
                Assert.AreEqual(SessionType.Malam, engine.State.session);
                Assert.IsTrue(engine.State.CanSleepNow);

                // Option B: Calm player never hits Tekanan >= 5, so NO Rift triggers!
                Assert.AreEqual(GamePhase.FreeRoam, engine.State.phase);
                Assert.IsFalse(engine.State.IsRiftSolved(1));

                // Sleep early at 18:00 (Days 1-6) or open FinalDecision on Day 7 Malam
                Assert.IsTrue(engine.SleepOrOpenFinalDecision());
            }

            Assert.AreEqual(GamePhase.FinalDecision, engine.State.phase);
            Assert.IsTrue(engine.SetDecisionNote(DecisionNoteType.HomeOrganizeEvidence));
            Assert.IsTrue(engine.ConfirmFinalDecision());
            Assert.AreEqual(EndingType.PulangBukanSelesai, engine.State.ending);
            Assert.AreEqual(GamePhase.Epilogue, engine.State.phase);

            // Continue after epilogue returns to Day 7 pre-finalization state.
            var reload = engine.LoadAutosave();
            Assert.IsTrue(reload.Success);
            Assert.AreEqual(7, engine.State.day);
            Assert.AreEqual(GamePhase.FreeRoam, engine.State.phase);
        }

        [Test]
        public void OptionB_HighPressureTriggersRift_AndSolvingLowersPressureTo3()
        {
            // Day 1: push Tekanan to 5 -> Rift 1 triggers automatically!
            engine.State.pressure = 4;
            engine.State.energy = 2;

            var preview = engine.GetPreview(new ActivityRequest(ActivityType.ImproveUiExample));
            Assert.AreEqual(5, preview.PressureAfter);
            Assert.AreEqual(1, preview.WillTriggerRift);

            var res = engine.ExecuteActivity(new ActivityRequest(ActivityType.ImproveUiExample));
            Assert.IsTrue(res.Executed);
            Assert.AreEqual(1, res.TriggeredRift);
            Assert.AreEqual(GamePhase.InRift, engine.State.phase);
            Assert.AreEqual(1, engine.State.activeRiftIndex);

            // Solving Rift 1 grounds Raka: Tekanan -> 3, Tenaga >= 1.
            Assert.IsTrue(engine.ResolveRift(skippedViaHelp: false));
            Assert.AreEqual(GamePhase.FreeRoam, engine.State.phase);
            Assert.IsTrue(engine.State.IsRiftSolved(1));
            Assert.AreEqual(3, engine.State.pressure);
            Assert.GreaterOrEqual(engine.State.energy, 1);

            // Hitting Tekanan 6 again same day (no Rift eligible) -> Jeda fallback triggers!
            engine.State.pressure = 5;
            engine.State.energy = 1;
            var res2 = engine.ExecuteActivity(new ActivityRequest(ActivityType.SubmitApplication, CompanyId.StudioLajur));
            Assert.IsTrue(res2.Executed);
            Assert.AreEqual(0, res2.TriggeredRift);
            Assert.IsTrue(res2.TriggeredJeda);
            Assert.AreEqual(GamePhase.AwaitingJeda, engine.State.phase);

            Assert.IsTrue(engine.ResolveJeda(JedaChoice.SitBriefly));
            Assert.AreEqual(4, engine.State.pressure);
            Assert.GreaterOrEqual(engine.State.energy, 1);
            Assert.IsTrue(engine.State.jedaTriggeredToday);
        }

        [Test]
        public void EarlyFinalization2A_AllowedFromDay4ViaFinalPrepSession()
        {
            // Blocked before Day 4
            Assert.IsFalse(engine.ExecuteActivity(new ActivityRequest(ActivityType.FinalPrep)).Executed);

            // On Day 4 with an offer, FinalPrep opens FinalDecision immediately without waiting for Day 7!
            engine.State.day = 4;
            engine.State.currentHour = 10;
            engine.State.applicationA.status = ApplicationStatus.Offered;
            engine.State.ClampStats();

            Assert.IsTrue(engine.SetOfferChoice(OfferChoice.StudioLajur));
            var prepRes = engine.ExecuteActivity(new ActivityRequest(ActivityType.FinalPrep));
            Assert.IsTrue(prepRes.Executed);
            Assert.IsTrue(prepRes.OpenedFinalDecision);
            Assert.AreEqual(GamePhase.FinalDecision, engine.State.phase);
            Assert.AreEqual(12, engine.State.currentHour);

            Assert.IsTrue(engine.SetDecisionNote(DecisionNoteType.WorkMentoringAndScope));
            Assert.IsTrue(engine.ConfirmFinalDecision());
            Assert.AreEqual(EndingType.MulaiDenganBatas, engine.State.ending);
        }

        [Test]
        public void ClockInterviewAt1300_AllowsEarlyStartAndWarnsIfOvershooting()
        {
            // Day 1: get E1 (08->11), attach E1 with exaggerated claim, submit A (11->13) -> Day 2 13:00 interview.
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.ImproveUiExample)).Executed);
            engine.UpdateCvDraft(ClaimChoice.Exaggerated, ClaimChoice.Honest, attachE1: true, attachE2: false);
            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.SubmitApplication, CompanyId.StudioLajur)).Executed);

            // Edit CV draft afterward -> must NOT mutate submitted snapshot
            engine.UpdateCvDraft(ClaimChoice.Honest, ClaimChoice.Honest, attachE1: false, attachE2: false);
            Assert.AreEqual(ClaimChoice.Exaggerated, engine.State.applicationA.snapshotClaim1);
            Assert.IsTrue(engine.State.applicationA.snapshotHasE1);

            // Jump to Day 2 08:00
            engine.State.currentHour = 18;
            engine.State.ClampStats();
            Assert.IsTrue(engine.SleepOrOpenFinalDecision());
            Assert.AreEqual(2, engine.State.day);
            Assert.AreEqual(8, engine.State.currentHour);

            // At 12:00, a 2h activity (12->14) crosses 13:00 and warns of missed interview!
            engine.State.currentHour = 12;
            engine.State.ClampStats();
            var missPreview = engine.GetPreview(new ActivityRequest(ActivityType.VerifyCompany, CompanyId.StudioLajur));
            Assert.IsTrue(missPreview.Availability.WillMissScheduledInterview);

            // Attending interview at 12:00 jumps cleanly to 13:00 -> finishes 15:00 (Siang)
            var intRes = engine.ExecuteActivity(
                new ActivityRequest(ActivityType.AttendInterview, CompanyId.StudioLajur),
                new InterviewInput { CorrectExaggeratedClaims = true, Question1ChoiceIndex = 0, Question2ChoiceIndex = 0 });

            Assert.IsTrue(intRes.Executed);
            Assert.IsTrue(intRes.InterviewOutcome.HasValue && intRes.InterviewOutcome.Value.Offered);
            Assert.AreEqual(15, engine.State.currentHour);
            Assert.AreEqual(SessionType.Siang, engine.State.session);
            Assert.AreEqual(ApplicationStatus.Offered, engine.State.applicationA.status);
        }

        [Test]
        public void MandiAction3A_Relieves3PressureAndIsOncePerDay()
        {
            engine.State.pressure = 4;
            engine.State.energy = 1;

            var res = engine.ExecuteActivity(new ActivityRequest(ActivityType.Shower));
            Assert.IsTrue(res.Executed);
            Assert.AreEqual(9, engine.State.currentHour); // 1 jam
            Assert.AreEqual(3, engine.State.energy);      // +2
            Assert.AreEqual(1, engine.State.pressure);    // -3
            Assert.IsTrue(engine.State.showeredToday);

            // Cannot shower twice on same day
            Assert.IsFalse(engine.ExecuteActivity(new ActivityRequest(ActivityType.Shower)).Executed);
        }

        [Test]
        public void SaveSystem_FallsBackToDayStart_WhenAutosaveCorrupted()
        {
            File.WriteAllText(saveSystem.AutosavePath, "{ broken json");
            var loaded = engine.LoadAutosave(allowFallback: true);

            Assert.IsTrue(loaded.Success);
            Assert.IsTrue(loaded.LoadedFromDayStartFallback);
            Assert.AreEqual(1, engine.State.day);
            Assert.AreEqual(GameState.StartHour, engine.State.currentHour);
            Assert.AreEqual(SessionType.Pagi, engine.State.session);
        }

        [Test]
        public void RiskyOfferC_RequiresAcknowledgementAndResolvesToKontrakYangMengunci()
        {
            engine.State.day = 4;
            engine.State.currentHour = 18;
            engine.State.ClampStats();

            Assert.IsTrue(engine.ExecuteActivity(new ActivityRequest(ActivityType.FinalPrep)).Executed);
            Assert.AreEqual(GamePhase.FinalDecision, engine.State.phase);
            Assert.IsTrue(engine.SetOfferChoice(OfferChoice.AksesPrima));

            // Cannot confirm without acknowledging risky terms
            Assert.IsFalse(engine.ConfirmFinalDecision());

            Assert.IsTrue(engine.SetDecisionNote(DecisionNoteType.RiskyOfferAcknowledged));
            Assert.IsTrue(engine.ConfirmFinalDecision());
            Assert.AreEqual(EndingType.KontrakYangMengunci, engine.State.ending);
        }
    }
}
