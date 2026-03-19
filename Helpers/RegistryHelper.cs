using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security;
using Win11Optimizer.Models;

namespace Win11Optimizer.Helpers;

/// <summary>レジストリ操作ヘルパー</summary>
public static class RegistryHelper
{
    private static RegistryKey GetBaseKey(string hive) => hive.ToUpperInvariant() switch
    {
        "HKEY_CURRENT_USER"  or "HKCU" => Registry.CurrentUser,
        "HKEY_LOCAL_MACHINE" or "HKLM" => Registry.LocalMachine,
        "HKEY_CLASSES_ROOT"  or "HKCR" => Registry.ClassesRoot,
        "HKEY_USERS"         or "HKU"  => Registry.Users,
        _ => throw new ArgumentException($"Unknown hive: {hive}")
    };

    /// <summary>現在の値を読み取る（存在しない場合は null）</summary>
    public static object? ReadValue(string hive, string keyPath, string valueName)
    {
        try
        {
            using var key = GetBaseKey(hive).OpenSubKey(keyPath, writable: false);
            return key?.GetValue(string.IsNullOrEmpty(valueName) ? "" : valueName);
        }
        catch { return null; }
    }

    /// <summary>値を書き込む</summary>
    public static bool WriteValue(string hive, string keyPath, string valueName, object value, string valueType)
    {
        try
        {
            var baseKey = GetBaseKey(hive);
            using var key = baseKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree)
                            ?? throw new InvalidOperationException($"CreateSubKey failed: {keyPath}");

            var kind = valueType.ToUpperInvariant() switch
            {
                "STRING"       or "REG_SZ"        => RegistryValueKind.String,
                "EXPANDSTRING" or "REG_EXPAND_SZ" => RegistryValueKind.ExpandString,
                "BINARY"       or "REG_BINARY"    => RegistryValueKind.Binary,
                "QWORD"        or "REG_QWORD"     => RegistryValueKind.QWord,
                _                                 => RegistryValueKind.DWord
            };

            object actualValue = kind == RegistryValueKind.DWord
                ? Convert.ToInt32(value) : value;

            key.SetValue(string.IsNullOrEmpty(valueName) ? "" : valueName, actualValue, kind);
            return true;
        }
        catch (SecurityException) { throw; }
        catch               { return false; }
    }

    /// <summary>値を削除する</summary>
    public static bool DeleteValue(string hive, string keyPath, string valueName)
    {
        try
        {
            using var key = GetBaseKey(hive).OpenSubKey(keyPath, writable: true);
            if (key == null) return true;
            key.DeleteValue(valueName, throwOnMissingValue: false);
            return true;
        }
        catch { return false; }
    }

    /// <summary>キー全体を削除する</summary>
    public static bool DeleteKey(string hive, string keyPath)
    {
        try { GetBaseKey(hive).DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false); return true; }
        catch { return false; }
    }

    /// <summary>キーが存在するか</summary>
    public static bool KeyExists(string hive, string keyPath)
    {
        try { using var key = GetBaseKey(hive).OpenSubKey(keyPath); return key != null; }
        catch { return false; }
    }

    /// <summary>ツイーク有効時の現在状態を読み取る</summary>
    public static bool ReadTweakCurrentState(TweakItem tweak)
    {
        if (tweak.RegistryEntries.Count == 0) return false;
        var first = tweak.RegistryEntries[0];
        var val   = ReadValue(first.Hive, first.KeyPath, first.ValueName);
        return val?.ToString() == first.ValueData;
    }

    /// <summary>バックアップ用に現在の値を収集する</summary>
    public static List<BackupRegistryValue> CollectBackup(List<RegistryEntry> entries)
    {
        var result = new List<BackupRegistryValue>(entries.Count);
        foreach (var entry in entries)
        {
            var val = ReadValue(entry.Hive, entry.KeyPath, entry.ValueName);
            result.Add(new BackupRegistryValue
            {
                Hive      = entry.Hive,
                KeyPath   = entry.KeyPath,
                ValueName = entry.ValueName,
                ValueData = val?.ToString(),
                ValueType = entry.ValueType,
                WasAbsent = val is null
            });
        }
        return result;
    }

    /// <summary>バックアップから復元する</summary>
    public static (int Success, int Failed) RestoreBackup(List<BackupRegistryValue> entries)
    {
        int ok = 0, fail = 0;
        foreach (var entry in entries)
        {
            try
            {
                if (entry.WasAbsent || entry.ValueData is null)
                    DeleteValue(entry.Hive, entry.KeyPath, entry.ValueName);
                else
                    WriteValue(entry.Hive, entry.KeyPath, entry.ValueName, entry.ValueData, entry.ValueType);
                ok++;
            }
            catch { fail++; }
        }
        return (ok, fail);
    }
}
