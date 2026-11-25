using System;
using System.IO;
using UnityEngine;

namespace SELA.Core
{
    public struct SaveOperationResult
    {
        public bool Success;
        public string Message;
    }

    public struct LoadOperationResult
    {
        public bool Success;
        public bool LoadedFromDayStartFallback;
        public GameState State;
        public string Message;
    }

    public class SaveSystem
    {
        public const string AutosaveFileName = "sela_autosave.json";
        public const string DayStartFileName = "sela_day_start.json";

        readonly string directory;

        public string AutosavePath => Path.Combine(directory, AutosaveFileName);
        public string DayStartPath => Path.Combine(directory, DayStartFileName);

        public SaveSystem(string customDirectory = null)
        {
            directory = string.IsNullOrEmpty(customDirectory)
                ? Application.persistentDataPath
                : customDirectory;
        }

        public SaveOperationResult SaveAutosave(GameState state)
        {
            return WriteAtomic(AutosavePath, state);
        }

        public SaveOperationResult SaveDayStartCheckpoint(GameState state)
        {
            return WriteAtomic(DayStartPath, state);
        }

        public bool HasAnySave() => File.Exists(AutosavePath) || File.Exists(DayStartPath);
        public bool HasAutosave() => File.Exists(AutosavePath);
        public bool HasDayStartCheckpoint() => File.Exists(DayStartPath);

        public LoadOperationResult LoadAutosave(bool allowFallbackToDayStart = true)
        {
            if (TryReadState(AutosavePath, out var state, out string err))
            {
                return new LoadOperationResult
                {
                    Success = true,
                    LoadedFromDayStartFallback = false,
                    State = state,
                    Message = $"Autosave terakhir: Hari {state.day} · {state.session}"
                };
            }

            if (allowFallbackToDayStart && TryReadState(DayStartPath, out var fallback, out string fbErr))
            {
                return new LoadOperationResult
                {
                    Success = true,
                    LoadedFromDayStartFallback = true,
                    State = fallback,
                    Message = $"Autosave tidak terbaca ({err}). Tersedia awal hari: Hari {fallback.day} · Pagi."
                };
            }

            return new LoadOperationResult
            {
                Success = false,
                LoadedFromDayStartFallback = false,
                State = null,
                Message = "Simpanan tidak terbaca. Mulai permainan baru diperlukan."
            };
        }

        public LoadOperationResult LoadDayStartCheckpoint()
        {
            if (TryReadState(DayStartPath, out var state, out string err))
            {
                return new LoadOperationResult
                {
                    Success = true,
                    LoadedFromDayStartFallback = false,
                    State = state,
                    Message = $"Awal hari: Hari {state.day} · Pagi"
                };
            }

            return new LoadOperationResult
            {
                Success = false,
                LoadedFromDayStartFallback = false,
                State = null,
                Message = $"Checkpoint awal hari tidak terbaca ({err})."
            };
        }

        public void DeleteAllSaves()
        {
            TryDelete(AutosavePath);
            TryDelete(DayStartPath);
            TryDelete(AutosavePath + ".tmp");
            TryDelete(DayStartPath + ".tmp");
        }

        SaveOperationResult WriteAtomic(string targetPath, GameState state)
        {
            try
            {
                Directory.CreateDirectory(directory);
                string json = JsonUtility.ToJson(state, true);
                string tempPath = targetPath + ".tmp";
                File.WriteAllText(tempPath, json);

                if (File.Exists(targetPath))
                    File.Delete(targetPath);
                File.Move(tempPath, targetPath);

                return new SaveOperationResult { Success = true, Message = "Tersimpan." };
            }
            catch (Exception ex)
            {
                return new SaveOperationResult
                {
                    Success = false,
                    Message = "Progres terbaru belum tersimpan: " + ex.Message
                };
            }
        }

        static bool TryReadState(string path, out GameState state, out string error)
        {
            state = null;
            if (!File.Exists(path))
            {
                error = "file tidak ditemukan";
                return false;
            }

            try
            {
                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    error = "file kosong";
                    return false;
                }

                var parsed = JsonUtility.FromJson<GameState>(json);
                if (parsed == null || parsed.saveVersion <= 0 || parsed.day < GameState.MinDay || parsed.day > GameState.MaxDay)
                {
                    error = "format simpanan tidak valid";
                    return false;
                }

                parsed.ClampStats();
                state = parsed;
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        static void TryDelete(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
