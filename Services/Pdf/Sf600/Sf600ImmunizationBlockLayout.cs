namespace Malama.Services.Pdf.Sf600;

/// <summary>
/// SF600 blank template grid: 18 text rows on page 1 (16 immunization + 2 notes),
/// same 18-row grid on page 2. Baselines sit inside row cells (above horizontal rules).
/// </summary>
internal static class Sf600ImmunizationBlockLayout
{
    // --- SF600 overlay tuning (change these, rebuild, regenerate PDF) ---
    //
    // GlobalBaselineYOffset: moves every row the same amount (use for first-line fit).
    // LineSpacingAdjustment: progressive spacing from row 0 downward (row 0 unchanged).
    //   Increase  -> lower rows move down (more space between lines).
    //   Decrease  -> lower rows move up (less space between lines).
    private const float GlobalBaselineYOffset = -16.5f;

    public const float LineSpacingAdjustment = 2.0f;

    public const int ImmunizationsPerPage = 4;
    public const int LinesPerImmunization = 4;
    public const int NoteLineCount = 2;
    public const int DataRowsPerPage = 18;

    /// <summary>
    /// Baseline Y per row, measured from SF600-Template.pdf grid.
    /// </summary>
    private static readonly float[] RowBaselines =
    [
        603.1f, 580.7f, 558.3f, 535.9f,
        513.6f, 491.2f, 468.8f, 446.4f,
        424.0f, 401.6f, 379.2f, 356.8f,
        334.4f, 312.0f, 289.7f, 267.3f,
        244.9f, 222.5f
    ];

    internal sealed record LineField(float LabelX, string Label, float ValueX);

    /// <summary>Fields on each of the 4 lines within one immunization block.</summary>
    public static readonly LineField[][] ImmunizationLines =
    [
        [
            new LineField(131.1f, "Immunization #{0}", 131.1f),
            new LineField(301.5f, "Dose #:", 360f),
            new LineField(424.7f, "Provided By:", 515f)
        ],
        [
            new LineField(131.1f, "Type:", 162f),
            new LineField(300.7f, "Manufacturer:", 390f)
        ],
        [
            new LineField(130.3f, "Lot#:", 168f),
            new LineField(299.9f, "Expiry Date:", 372f)
        ],
        [
            new LineField(131.1f, "Site:", 165f),
            new LineField(196.9f, "BodyPart:", 257f),
            new LineField(453.9f, "Type:", 489f)
        ]
    ];

    public const float DateX = 39f;
    public const float NotesLabelX = 127f;

    public static int GetPageNumber(int immunizationIndex) =>
        immunizationIndex / ImmunizationsPerPage + 1;

    public static int GetLocalImmunizationIndex(int immunizationIndex) =>
        immunizationIndex % ImmunizationsPerPage;

    public static int GetFirstRowIndexForImmunization(int localImmunizationIndex) =>
        localImmunizationIndex * LinesPerImmunization;

    public static float GetRowBaseline(int pageNumber, int rowIndex)
    {
        _ = pageNumber;
        if (rowIndex < 0 || rowIndex >= RowBaselines.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        return RowBaselines[rowIndex]
            + GlobalBaselineYOffset
            - (rowIndex * LineSpacingAdjustment);
    }

    public static int GetRequiredPageCount(int immunizationCount) =>
        immunizationCount <= ImmunizationsPerPage ? 1 : 2;

    public const int NotesRowIndex = 16;
}
