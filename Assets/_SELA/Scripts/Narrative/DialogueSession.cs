using System.Collections.Generic;
using SELA.Core;

namespace SELA.Narrative
{
    public sealed class DialogueLine
    {
        public string Speaker { get; }
        public string Text { get; }
        public int ChoiceCount => choices.Length;

        readonly DialogueChoice[] choices;

        public DialogueLine(string speaker, string text, params DialogueChoice[] choices)
        {
            Speaker = speaker;
            Text = text;
            this.choices = choices;
        }

        public DialogueChoice ChoiceAt(int index) => choices[index];
    }

    public readonly struct DialogueChoice
    {
        public string Text { get; }
        public DialogueLine Reply { get; }

        public DialogueChoice(string text, DialogueLine reply = null)
        {
            Text = text;
            Reply = reply;
        }
    }

    public sealed class DialogueSession
    {
        readonly DialogueLine[] lines;
        readonly int[] answers;
        List<DialogueLogEntry> log;
        DialogueLine reply;
        int index;

        public bool IsComplete => index == lines.Length;
        public DialogueLine Current => IsComplete ? null : reply ?? lines[index];
        public bool WillFinishOnAdvance => index == lines.Length - 1 && Current.ChoiceCount == 0;

        public DialogueSession(params DialogueLine[] lines)
        {
            this.lines = lines;
            answers = new int[lines.Length];
        }

        internal int AnswerAt(int lineIndex) => answers[lineIndex];

        public void RecordTo(List<DialogueLogEntry> entries)
        {
            if (ReferenceEquals(log, entries)) return;
            log = entries;
            RecordCurrent();
        }

        void RecordCurrent()
        {
            var line = Current;
            if (line != null) log?.Add(new DialogueLogEntry(line.Speaker, line.Text));
        }

        public bool Advance(int choice = -1)
        {
            var line = Current;
            if (line == null) return false;
            if (reply != null || line.ChoiceCount == 0)
            {
                if (choice != -1) return false;
                reply = null;
                index++;
            }
            else
            {
                if ((uint)choice >= (uint)line.ChoiceCount) return false;
                answers[index] = choice;
                var answer = line.ChoiceAt(choice);
                log?.Add(new DialogueLogEntry("Raka", answer.Text));
                reply = answer.Reply;
                if (reply == null) index++;
            }

            RecordCurrent();
            return true;
        }
    }
}
