#region Assembly Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Microsoft.VisualBasic.dll
#endregion

using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Microsoft.VisualBasic.FileIO
{
    //
    // Summary:
    //     Provides methods and properties for parsing structured text files.
    public class TextFieldParser : IDisposable
    {
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   path:
        //     String. The complete path of the file to be parsed.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     path is an empty string.
        public TextFieldParser(string path);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   stream:
        //     System.IO.Stream. The stream to be parsed.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     stream is Nothing.
        //
        //   T:System.ArgumentException:
        //     stream cannot be read from.
        public TextFieldParser(Stream stream);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   reader:
        //     System.IO.TextReader. The System.IO.TextReader stream to be parsed.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     reader is Nothing.
        public TextFieldParser(TextReader reader);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   path:
        //     String. The complete path of the file to be parsed.
        //
        //   defaultEncoding:
        //     System.Text.Encoding. The character encoding to use if encoding is not determined
        //     from file. Default is System.Text.Encoding.UTF8.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     path is an empty string or defaultEncoding is Nothing.
        public TextFieldParser(string path, Encoding defaultEncoding);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   stream:
        //     System.IO.Stream. The stream to be parsed.
        //
        //   defaultEncoding:
        //     System.Text.Encoding. The character encoding to use if encoding is not determined
        //     from file. Default is System.Text.Encoding.UTF8.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     stream or defaultEncoding is Nothing.
        //
        //   T:System.ArgumentException:
        //     stream cannot be read from.
        public TextFieldParser(Stream stream, Encoding defaultEncoding);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   path:
        //     String. The complete path of the file to be parsed.
        //
        //   defaultEncoding:
        //     System.Text.Encoding. The character encoding to use if encoding is not determined
        //     from file. Default is System.Text.Encoding.UTF8.
        //
        //   detectEncoding:
        //     Boolean. Indicates whether to look for byte order marks at the beginning of the
        //     file. Default is True.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     path is an empty string or defaultEncoding is Nothing.
        public TextFieldParser(string path, Encoding defaultEncoding, bool detectEncoding);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   stream:
        //     System.IO.Stream. The stream to be parsed.
        //
        //   defaultEncoding:
        //     System.Text.Encoding. The character encoding to use if encoding is not determined
        //     from file. Default is System.Text.Encoding.UTF8.
        //
        //   detectEncoding:
        //     Boolean. Indicates whether to look for byte order marks at the beginning of the
        //     file. Default is True.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     stream or defaultEncoding is Nothing.
        //
        //   T:System.ArgumentException:
        //     stream cannot be read from.
        public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding);
        //
        // Summary:
        //     Initializes a new instance of the TextFieldParser class.
        //
        // Parameters:
        //   stream:
        //     System.IO.Stream. The stream to be parsed.
        //
        //   defaultEncoding:
        //     System.Text.Encoding. The character encoding to use if encoding is not determined
        //     from file. Default is System.Text.Encoding.UTF8.
        //
        //   detectEncoding:
        //     Boolean. Indicates whether to look for byte order marks at the beginning of the
        //     file. Default is True.
        //
        //   leaveOpen:
        //     Boolean. Indicates whether to leave stream open when the TextFieldParser object
        //     is closed. Default is False.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     stream or defaultEncoding is Nothing.
        //
        //   T:System.ArgumentException:
        //     stream cannot be read from.
        public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding, bool leaveOpen);

        //
        // Summary:
        //     Allows the Microsoft.VisualBasic.FileIO.TextFieldParser object to attempt to
        //     free resources and perform other cleanup operations before it is reclaimed by
        //     garbage collection.
        ~TextFieldParser();

        //
        // Summary:
        //     Denotes the width of each column in the text file being parsed.
        //
        // Returns:
        //     An integer array that contains the width of each column in the text file that
        //     is being parsed.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     A width value in any location other than the last entry of the array is less
        //     than or equal to zero.
        public int[] FieldWidths { get; set; }
        //
        // Summary:
        //     Indicates whether the file to be parsed is delimited or fixed-width.
        //
        // Returns:
        //     A Microsoft.VisualBasic.FileIO.TextFieldParser.TextFieldType value that indicates
        //     whether the file to be parsed is delimited or fixed-width.
        public FieldType TextFieldType { get; set; }
        //
        // Summary:
        //     Returns the number of the line that caused the most recent Microsoft.VisualBasic.FileIO.MalformedLineException
        //     exception.
        //
        // Returns:
        //     The number of the line that caused the most recent Microsoft.VisualBasic.FileIO.MalformedLineException
        //     exception.
        public long ErrorLineNumber { get; }
        //
        // Summary:
        //     Returns the line that caused the most recent Microsoft.VisualBasic.FileIO.MalformedLineException
        //     exception.
        //
        // Returns:
        //     The line that caused the most recent Microsoft.VisualBasic.FileIO.MalformedLineException
        //     exception.
        public string ErrorLine { get; }
        //
        // Summary:
        //     Returns the current line number, or returns -1 if no more characters are available
        //     in the stream.
        //
        // Returns:
        //     The current line number.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public long LineNumber { get; }
        //
        // Summary:
        //     Returns True if there are no non-blank, non-comment lines between the current
        //     cursor position and the end of the file.
        //
        // Returns:
        //     True if there is no more data to read; otherwise, False.
        public bool EndOfData { get; }
        //
        // Summary:
        //     Defines comment tokens. A comment token is a string that, when placed at the
        //     beginning of a line, indicates that the line is a comment and should be ignored
        //     by the parser.
        //
        // Returns:
        //     A string array that contains all of the comment tokens for the Microsoft.VisualBasic.FileIO.TextFieldParser
        //     object.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     A comment token includes white space.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string[] CommentTokens { get; set; }
        //
        // Summary:
        //     Defines the delimiters for a text file.
        //
        // Returns:
        //     A string array that contains all of the field delimiters for the Microsoft.VisualBasic.FileIO.TextFieldParser
        //     object.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     A delimiter value is set to a newline character, an empty string, or Nothing.
        public string[] Delimiters { get; set; }
        //
        // Summary:
        //     Denotes whether fields are enclosed in quotation marks when a delimited file
        //     is being parsed.
        //
        // Returns:
        //     True if fields are enclosed in quotation marks; otherwise, False.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool HasFieldsEnclosedInQuotes { get; set; }
        //
        // Summary:
        //     Indicates whether leading and trailing white space should be trimmed from field
        //     values.
        //
        // Returns:
        //     True if leading and trailing white space should be trimmed from field values;
        //     otherwise, False.
        public bool TrimWhiteSpace { get; set; }

        //
        // Summary:
        //     Closes the current TextFieldParser object.
        public void Close();
        //
        // Summary:
        //     Releases resources used by the Microsoft.VisualBasic.FileIO.TextFieldParser object.
        public void Dispose();
        //
        // Summary:
        //     Reads the specified number of characters without advancing the cursor.
        //
        // Parameters:
        //   numberOfChars:
        //     Int32. Number of characters to read. Required.
        //
        // Returns:
        //     A string that contains the specified number of characters read.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     numberOfChars is less than 0.
        public string PeekChars(int numberOfChars);
        //
        // Summary:
        //     Reads all fields on the current line, returns them as an array of strings, and
        //     advances the cursor to the next line containing data.
        //
        // Returns:
        //     An array of strings that contains field values for the current line.
        //
        // Exceptions:
        //   T:Microsoft.VisualBasic.FileIO.MalformedLineException:
        //     A field cannot be parsed by using the specified format.
        public string[] ReadFields();
        //
        // Summary:
        //     Returns the current line as a string and advances the cursor to the next line.
        //
        // Returns:
        //     The current line from the file or stream.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ReadLine();
        //
        // Summary:
        //     Reads the remainder of the text file and returns it as a string.
        //
        // Returns:
        //     The remaining text from the file or stream.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ReadToEnd();
        //
        // Summary:
        //     Sets the delimiters for the reader to the specified values, and sets the field
        //     type to Delimited.
        //
        // Parameters:
        //   delimiters:
        //     Array of type String.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     A delimiter is zero-length.
        public void SetDelimiters(params string[] delimiters);
        //
        // Summary:
        //     Sets the delimiters for the reader to the specified values.
        //
        // Parameters:
        //   fieldWidths:
        //     Array of Integer.
        public void SetFieldWidths(params int[] fieldWidths);
        //
        // Summary:
        //     Releases resources used by the Microsoft.VisualBasic.FileIO.TextFieldParser object.
        //
        // Parameters:
        //   disposing:
        //     Boolean. True releases both managed and unmanaged resources; False releases only
        //     unmanaged resources.
        protected virtual void Dispose(bool disposing);
    }
}