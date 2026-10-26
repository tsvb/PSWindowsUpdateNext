using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PSWindowsUpdateNext.Gui
{
    public abstract class RowBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            Raise(name);
            return true;
        }

        protected void Raise(string name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class UpdateRow : RowBase
    {
        bool _selected;
        public bool Selected { get { return _selected; } set { Set(ref _selected, value); } }
        public string KB { get; set; }
        public string Title { get; set; }
        public string Size { get; set; }
        public decimal SizeBytes { get; set; }
        public string Category { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsInstalled { get; set; }
        public bool RebootRequired { get; set; }
        public string UpdateID { get; set; }
        public string Description { get; set; }
        public string MoreInfoUrl { get; set; }
        public DateTime Published { get; set; }
        public string PublishedText { get { return Published == DateTime.MinValue ? "" : Published.ToString("yyyy-MM-dd"); } }
        public string KBVisibility { get { return string.IsNullOrWhiteSpace(KB) ? "Collapsed" : "Visible"; } }
        public string StateText
        {
            get
            {
                if (IsInstalled) return "Installed";
                if (IsHidden) return "Hidden";
                if (IsDownloaded) return "Downloaded";
                return "Available";
            }
        }
        public string StateColor
        {
            get
            {
                if (IsInstalled) return "#2ECC8A";
                if (IsHidden) return "#8A97B0";
                if (IsDownloaded) return "#4F8CFF";
                return "#F5B731";
            }
        }
        public string SeverityColor
        {
            get
            {
                switch ((Severity ?? "").ToLowerInvariant())
                {
                    case "critical": return "#FF5C6C";
                    case "important": return "#F5B731";
                    case "moderate": return "#4F8CFF";
                    case "low": return "#8A97B0";
                    default: return "#3A4661";
                }
            }
        }
    }

    public class HistoryRow : RowBase
    {
        public DateTime Date { get; set; }
        public string DateText { get { return Date.ToString("yyyy-MM-dd HH:mm"); } }
        public string Title { get; set; }
        public string KB { get; set; }
        public string Result { get; set; }
        public string Operation { get; set; }
        public string ResultColor
        {
            get
            {
                switch ((Result ?? "").ToLowerInvariant())
                {
                    case "succeeded": return "#2ECC8A";
                    case "succeededwitherrors": return "#F5B731";
                    case "failed": return "#FF5C6C";
                    case "aborted": return "#FF5C6C";
                    case "inprogress": return "#4F8CFF";
                    default: return "#8A97B0";
                }
            }
        }
    }

    public class ServiceRow : RowBase
    {
        public string Name { get; set; }
        public string ServiceID { get; set; }
        public bool IsDefault { get; set; }
        public bool IsManaged { get; set; }
        public string ServiceUrl { get; set; }
        public string DefaultVisibility { get { return IsDefault ? "Visible" : "Collapsed"; } }
        public string ManagedVisibility { get { return IsManaged ? "Visible" : "Collapsed"; } }
    }

    public class JobRow : RowBase
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string LastRun { get; set; }
        public string NextRun { get; set; }
        public string Description { get; set; }
        public string StateColor
        {
            get
            {
                switch ((State ?? "").ToLowerInvariant())
                {
                    case "running": return "#4F8CFF";
                    case "ready": return "#2ECC8A";
                    case "disabled": return "#8A97B0";
                    default: return "#F5B731";
                }
            }
        }
    }

    public class CatalogRow : RowBase
    {
        bool _selected;
        string _result;
        public bool Selected { get { return _selected; } set { Set(ref _selected, value); } }
        public string Identity { get; set; }
        public string Title { get; set; }
        public string Product { get; set; }
        public string Classification { get; set; }
        public string LastUpdated { get; set; }
        public string Size { get; set; }
        public long SizeBytes { get; set; }
        public string Result { get { return _result; } set { if (Set(ref _result, value)) Raise("ResultColor"); } }
        public string ResultColor
        {
            get
            {
                switch ((_result ?? "").ToLowerInvariant())
                {
                    case "downloaded": return "#2ECC8A";
                    case "error": return "#FF5C6C";
                    case "canceled": return "#F5B731";
                    case "accepted": return "#4F8CFF";
                    default: return "#3A4661";
                }
            }
        }
    }

    public class SettingRow : RowBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string TimeText { get { return Time.ToString("HH:mm:ss"); } }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Color
        {
            get
            {
                switch ((Level ?? "").ToLowerInvariant())
                {
                    case "error": return "#FF5C6C";
                    case "warning": return "#F5B731";
                    case "verbose": return "#8A97B0";
                    case "debug": return "#5B6B8A";
                    case "task": return "#4F8CFF";
                    case "ok": return "#2ECC8A";
                    default: return "#E6EDF7";
                }
            }
        }
    }
}
