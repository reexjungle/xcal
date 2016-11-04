using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.io.writers
{
    /// <summary>
    /// Specifies an interface that provides custom forrmatting for iCalendar serialization.
    /// </summary>
    public interface ICalendarWriter
    {

        /// <summary>
        /// Writes the start tag of an iCalendar object or component with its specified name to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        /// <example>
        /// This example shows how to use the <see cref="WriteStartComponent"/>
        /// method for a VCALENDAR component.
        /// <code>
        /// WriteStartComponent("VCALENDAR");
        /// </code>
        /// Produces the following result:
        /// <para/>
        /// BEGIN:VCALENDAR
        /// </example>
        ICalendarWriter WriteStartComponent(string name);

        /// <summary>
        /// Appends the start tag of an iCalendar object or component with its specified name to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        ICalendarWriter AppendStartComponent(string name);

        /// <summary>
        /// Writes the end tag of an iCalendar object or component with its specified name to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        /// <example>
        /// This example shows how to use the <see cref="ICalendarWriter.WriteEndComponent"/> method
        /// for a VCALENDAR component.
        /// <code>
        /// WriteEndComponent("VCALENDAR");
        /// </code>
        /// Produces the following result:
        /// <para/>
        /// END:VCALENDAR
        /// </example>
        ICalendarWriter WriteEndComponent(string name);

        /// <summary>
        /// Appends the end tag of an iCalendar object or component with its specified name 
        /// to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar object or component.</param>
        /// <returns>The current <see cref="ICalendarWriter"/> instance.</returns>
        ICalendarWriter AppendEndComponent(string name);

        /// <summary>
        /// Encloses a string value with double quotes ("...") and escapes any pre-existing quote (")
        /// within the string with a (\").
        /// </summary>
        /// <param name="value">The value to be enclosed in double quotes.</param>
        /// <returns>The value enclosed by double quotes.</returns>
        string ConvertToQuotedString(string value);

        /// <summary>
        /// Escapes the following characters in the specified string value:
        /// <para/>
        /// ':' with '\:'
        /// <para/>
        /// ';' with '\;'
        /// <para/>
        /// ',' with '\,'
        /// </summary>
        /// <param name="value">The provided string value.</param>
        /// <returns>
        /// The string value where the colon, semicolon or comma characters have been escaped.
        /// </returns>
        string ConvertToSAFE_STRING(string value);

        /// <summary>
        /// Escapes the following characters in the specified string value:
        /// <para/>
        /// '\t' with '\0'
        /// <para/>
        /// '"' with '\";'
        /// </summary>
        /// <param name="value">The provided string value.</param>
        /// <returns>
        /// The string value where the horizontal tab or double quote characters have been escaped.
        /// </returns>
        string ConvertToQSAFE_STRING(string value);

        /// <summary>
        /// Writes the string value to the underlying text string or stream, in which the following characters have been escaped:
        /// <para/>
        /// ':' with '\:'
        /// <para/>
        /// ';' with '\;'
        /// <para/>
        /// ',' with '\,'
        /// </summary>
        /// <param name="value"></param>
        ICalendarWriter WriteSafeStringValue(string value);

        /// <summary>
        /// Writes the value enclosed by double quotes (") to the underlying text string or stream. 
        /// </summary>
        /// <param name="value">The string value to be enclosed by double quotes.</param>
        ICalendarWriter WriteQuotedStringValue(string value);

        /// <summary>
        /// Writes a double quote (") to the underlying text string or stream
        /// </summary>
        /// <returns></returns>
        ICalendarWriter WriteDQuote();

        /// <summary>
        /// Writes a comma character to the underlying text string or stream.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteComma();

        /// <summary>
        /// Writes a semicolon character to the underlying text string or stream.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteSemicolon();

        /// <summary>
        /// Writes a colon character to the underlying text string or stream.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteColon();

        /// <summary>
        /// Writes the equality character to the underlying text string or stream.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteEquals();

        ICalendarWriter WriteLine();

        /// <summary>
        /// Writes an iCalendar value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value to be written to the underlying text string or stream.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteValue(string value);

        /// <summary>
        /// Writes a sequence of iCalendar values concatenated by a comma character to the underlying text string or stream.
        /// </summary>
        /// <param name="values">The sequence of values to be written to the underlying text string or stream.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteValues(IEnumerable<string> values);

        /// <summary>
        /// Writes iCalendar parameters to the underlying text string or stream.
        /// <para/>
        /// It is assumed each parameter is correctly formatted in the iCalendar format.
        /// </summary>
        /// <param name="parameters">The parameters to be written to the underlying text string or stream.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteParameters(IEnumerable<string> parameters);

        ICalendarWriter WriteParameter(string name, string value);

        /// <summary>
        /// Writes an iCalendar parameter to the string or stream.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The values of the parameter.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteParameter(string name, IEnumerable<string> values);

        /// <summary>
        /// Writes the non-standard experimental parameter by prefixing the parameter name with an 'X-'.
        /// </summary>
        /// <param name="name">The name of the non-standard experimental parameter.</param>
        /// <param name="values">
        /// The values of the expermimental parameter.
        /// <para/>
        /// Note: It is assumed each value is correctly written in the iCalendar format.
        /// </param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteXParameter(string name, IEnumerable<string> values);

        /// <summary>
        /// Appends an iCalendar parameter with its value to the underyling text string or stream.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendParameter(string name, string value);

        /// <summary>
        /// Appends an iCalendar parameter with its values to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The sequence of values of the parameter.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendParameter(string name, IEnumerable<string> values);

        /// <summary>
        /// Appends a comma (",") and the specified value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendByComma(string value);

        /// <summary>
        /// Appends a semicolon (";") and the specified value to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendBySemicolon(string value);

        /// <summary>
        /// Appends a comma (",") and the specified sequence of iCalendar values, where each value is delimited by a comma (",").
        /// </summary>
        /// <param name="values">The sequence of iCalendar values to appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendByComma(IEnumerable<string> values);

        /// <summary>
        /// Appends a semicolon (";") and the specified sequence of iCalendar values, where each value is delimited by a comma (";").
        /// </summary>
        /// <param name="values">The sequence of iCalendar values to appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendBySemicolon(IEnumerable<string> values);


        /// <summary>
        /// Appends a specified formatted iCalendar paraneter to the underlying text string or stream.
        /// </summary>
        /// <param name="parameter">The parameter to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendParameter(string parameter);

        /// <summary>
        /// Appends a specified sequence of parameters to the underlying text string or stream.
        /// </summary>
        /// <param name="parameters">The sequence of parameters to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendParameters(IEnumerable<string> parameters);

        /// <summary>
        /// Appends the value of an iCalendar property to the underlying text string or stream.
        /// </summary>
        /// <param name="value">The value that is to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendPropertyValue(string value);

        /// <summary>
        /// Appends the sequence of values of an iCalendar property to the underlying text string or stream. 
        /// </summary>
        /// <param name="values">The sequence of values to be appended.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendPropertyValues(IEnumerable<string> values);

        /// <summary>
        /// Writes an iCalendar property by its name, value and parameters to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar property.</param>
        /// <param name="value">The value of the iCalendar property.</param>
        /// <param name="parameters">The sequence of parameter strings of the iCalendar property.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteProperty(string name, string value, IEnumerable<string> parameters = null);

        /// <summary>
        /// Appends an iCalendar property by its name, value and parameters to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the iCalendar property.</param>
        /// <param name="value">The value of the iCalendar property.</param>
        /// <param name="parameters">The sequence of parameter strings of the iCalendar property.</param>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter AppendProperty(string name, string value, IEnumerable<string> parameters = null);

        /// <summary>
        /// Writes properties, where each written property is followed by the line terminator.
        /// </summary>
        /// <param name="properties"></param>
        /// <remarks>This method assumes each property has been correctly formatted.</remarks>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteProperties(IEnumerable<string> properties);

        /// <summary>
        /// Writes an iCalendar COMPONENT with the specifed component name and properties of the
        /// component to the underlying text string or stream.
        /// </summary>
        /// <param name="name">The name of the component to be written.</param>
        /// <param name="properties">
        /// The properties of the component.
        /// <para/>
        /// Note: It is assumed each property is correctly written in the iCalendar format.
        /// </param>
        /// <remarks>This method assumes the parameters of the property have been correctly formatted.</remarks>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter WriteComponent(string name, IEnumerable<string> properties);

        /// <summary>
        /// Writes iCalendar COMPONENTs to the underlying text string or stream.
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Writes iCalendar parameters to the underlying text string or stream.</returns>
        ICalendarWriter WriteComponents(IEnumerable<string> components);

        /// <summary>
        /// Inserts line breaks after every 75 characters in the string representation.
        /// <para>
        /// That is for eack line, a carriage return line feed (CRLF) followed by a single linear
        /// white-space character(i.e., SPACE or horizontal tab) is inserted after every 76
        /// characters in the string representation.
        /// </para>
        /// <para>
        /// Any sequence of CRLF followed immediately by a single linear white-space character is
        /// ignored(i.e., removed) when processing the content type.
        /// </para>
        /// <para/>
        /// Note: Each split line is not longer than 75 octets excluding line breaks.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter InsertLineBreaks();

        /// <summary>
        /// Removes line breaks after every 75 characters in the string representation.
        /// </summary>
        /// <returns>This instance of the <see cref="ICalendarWriter"/> class.</returns>
        ICalendarWriter RemoveLineBreaks();
    }
}
