namespace Dt.Pdf.Object
{
    /// <summary>
    /// Interface of Graphics stream
    /// </summary>
    internal interface IGraphicsStream
    {
        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        PdfResources GetResources();
    }
}

