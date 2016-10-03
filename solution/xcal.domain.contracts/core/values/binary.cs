namespace reexjungle.xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain a character encoding of inline binary data.
    /// The character encoding is based on the Base64 encoding
    /// </summary>
    public interface IBINARY
    {
        /// <summary>
        /// Gets or sets the Base64 value of this type.
        /// </summary>
        string Value { get; }

        bool IsBase64(string value);
    }
}
