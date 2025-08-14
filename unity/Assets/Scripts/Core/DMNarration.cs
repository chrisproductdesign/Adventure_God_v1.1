using System;
using System.Collections.Generic;

public static class DMNarration
{
    private static readonly Dictionary<string, string> ActorToLastNote = new Dictionary<string, string>();
    public static event Action<string, string> NoteChanged; // (actorId, note)

    public static void SetLastNote(string actorId, string note)
    {
        if (string.IsNullOrEmpty(actorId)) return;
        ActorToLastNote[actorId] = note ?? string.Empty;
        NoteChanged?.Invoke(actorId, ActorToLastNote[actorId]);
    }

    public static string GetLastNote(string actorId)
    {
        if (string.IsNullOrEmpty(actorId)) return string.Empty;
        return ActorToLastNote.TryGetValue(actorId, out var note) ? note : string.Empty;
    }

    public static Dictionary<string, string> GetAllNotes()
    {
        return new Dictionary<string, string>(ActorToLastNote);
    }
}


