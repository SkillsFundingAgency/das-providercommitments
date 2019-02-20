namespace SFA.DAS.ProviderCommitments.Models
{
    /// <summary>
    ///     Indicates how the day should be determined for <see cref="MonthYearModel"/>
    ///     when deriving an actual date time from the month year.
    /// </summary>
    public enum DayAlignment
    {
        /// <summary>
        ///     The day will be set to the 1st.
        /// </summary>
        StartOfMonth,

        /// <summary>
        ///     The day will be set to the end of the month for the month year specified.
        /// </summary>
        EndOfMonth,

        /// <summary>
        ///     The day will be fixed regardless of the month year. Care should be taken to ensure
        ///     a day is chosen that is valid for all month and years. 
        /// </summary>
        Fixed
    }
}