using System;
using System.Collections.Generic;

namespace Win11Optimizer.Models;

/// <summary>適用前バックアップの1エントリ</summary>
public class BackupRegistryValue
{
    public string  Hive      { get; set; } = "";
    public string  KeyPath   { get; set; } = "";
    public string  ValueName { get; set; } = "";
    public string? ValueData { get; set; }
    public string  ValueType { get; set; } = "DWORD";
    public bool    WasAbsent { get; set; } = false;
}

/// <summary>セッション全体のバックアップ</summary>
public class BackupSession
{
    public DateTime                  CreatedAt   { get; set; } = DateTime.Now;
    public string                    Description { get; set; } = "";
    public List<BackupRegistryValue> Entries     { get; set; } = [];
}
