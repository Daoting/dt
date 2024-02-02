namespace Dt.Pdf.Object
{
    /// <summary>
    /// Interface of version depend
    /// </summary>
    internal interface IVersionDepend
    {
        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        PdfVersion Version();
    }
}

