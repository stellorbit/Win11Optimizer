using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Win11Optimizer.Models;

/// <summary>再起動が必要な種別</summary>
public enum RestartRequirement
{
    None,
    ExplorerRestart,
    SystemRestart
}

/// <summary>レジストリ操作の1エントリ</summary>
public class RegistryEntry
{
    public string Hive          { get; set; } = "";
    public string KeyPath       { get; set; } = "";
    public string ValueName     { get; set; } = "";
    public string ValueData     { get; set; } = "";
    public string DefaultData   { get; set; } = "";
    public string ValueType     { get; set; } = "DWORD";
    public bool   DeleteOnDisable { get; set; } = false;
}

/// <summary>1つのツイーク(設定項目)</summary>
public class TweakItem : INotifyPropertyChanged
{
    private bool   _initialIsEnabled;
    private bool   _isEnabled;
    private bool   _isLoading;
    private string _selectedPath = "";

    public string               Id                   { get; set; } = "";
    public string               Category             { get; set; } = "";
    public string               Name                 { get; set; } = "";
    public string               Description          { get; set; } = "";
    public RestartRequirement   RestartRequirement   { get; set; } = RestartRequirement.None;
    public List<RegistryEntry>  RegistryEntries      { get; set; } = [];
    public string?              PowerShellEnableCommand  { get; set; }
    public string?              PowerShellDisableCommand { get; set; }
    public bool                 RequiresUcpdDisable  { get; set; } = false;

    /// <summary>ファイルピッカー付き項目か（壁紙・ロック画面の画像選択）</summary>
    public bool   IsFilePickerItem  { get; set; } = false;
    public string FilePickerFilter  { get; set; } = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

    /// <summary>FilePickerItem で選択中のパス（表示用）</summary>
    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            if (_selectedPath != value)
            {
                _selectedPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedPath));
            }
        }
    }

    public bool HasSelectedPath => !string.IsNullOrEmpty(_selectedPath);

    public bool InitialIsEnabled
    {
        get => _initialIsEnabled;
        set
        {
            if (_initialIsEnabled != value)
            {
                _initialIsEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPendingChange));
            }
        }
    }

    public bool HasPendingChange => InitialIsEnabled != IsEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPendingChange));
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { if (_isLoading != value) { _isLoading = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

/// <summary>カテゴリグループ</summary>
public class TweakCategory : INotifyPropertyChanged
{
    private bool _isExpanded = true;

    public string           Name  { get; set; } = "";
    public List<TweakItem>  Items { get; set; } = [];

    public bool IsExpanded
    {
        get => _isExpanded;
        set { if (_isExpanded != value) { _isExpanded = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
