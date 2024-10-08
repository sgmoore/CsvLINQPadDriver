using System.ComponentModel;

namespace CsvLINQPadDriver;

public enum CsvModeOptions
{
    [Description("Use RFC 4180 format")]
    Default,

    [Description("Use escapes")]
    Escape,

    [Description("Do not use quotes or escapes")]
    NoEscape
}