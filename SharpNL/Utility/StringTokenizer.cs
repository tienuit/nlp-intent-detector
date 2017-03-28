// 
//  Copyright 2015 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// The string tokenizer class allows an application to break a string into tokens.
    /// </summary>
    public class StringTokenizer {
        private StringToken peeked;

        private int startCol;
        private int startLine;
        private int startPos;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTokenizer"/> class using the specified string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public StringTokenizer(string value) {
            Value = value;
            Line = 1;
            Column = 1;
        }

        #endregion

        #region + Properties .

        #region . Column .

        /// <summary>
        /// Gets the current column.
        /// </summary>
        /// <value>The current column.</value>
        public int Column { get; protected set; }

        #endregion

        #region . IgnoreWhitespace .

        /// <summary>
        /// Gets or sets a value indicating whether to ignore white space tokens.
        /// </summary>
        /// <value><c>true</c> to ignore white space; otherwise <c>false</c>. The default is <c>false</c>.</value>
        public bool IgnoreWhitespace { get; set; }

        #endregion

        #region . Line .

        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <value>The current line.</value>
        public int Line { get; protected set; }

        #endregion

        #region . Position .

        /// <summary>
        /// Gets the current cursor position.
        /// </summary>
        /// <value>The current cursor position.</value>
        public int Position { get; protected set; }

        #endregion

        #region . Value .

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <value>The string value.</value>
        protected string Value { get; private set; }

        #endregion

        #endregion

        #region + Methods .

        #region . CreateToken .

        protected StringToken CreateToken(StringTokenKind kind) {
            return new StringToken(kind, Value.Substring(startPos, Position - startPos), startLine, startCol);
        }

        protected StringToken CreateToken(StringTokenKind kind, string value) {
            return new StringToken(kind, value, Line, Column);
        }

        #endregion

        #region . Skip .

        /// <summary>
        /// Skips the current token.
        /// </summary>
        /// <returns><c>true</c> if token was skipped, <c>false</c> otherwise.</returns>
        public bool Skip() {
            if (peeked == null)
                return Read() != null;

            peeked = null;
            return true;
        }

        #endregion

        #region . SkipPos .

        /// <summary>
        /// Skips the current position.
        /// </summary>
        protected void SkipPos() {
            Position++;
            Column++;
        }

        #endregion

        #region . Peek .

        /// <summary>
        /// Returns the <see cref="StringTokenizer"/> at the beginning of the <see cref="StringTokenizer"/> without removing it.
        /// </summary>
        /// <returns>The <see cref="StringToken"/> at the beginning of the <see cref="StringToken"/> without removing it.</returns>
        public StringToken Peek() {
            if (peeked != null)
                return peeked;

            return (peeked = Read());
        }

        protected bool Peek(out char? value) {
            if (Position >= Value.Length) {
                value = null;
                return false;
            }
            value = Value[Position];
            return true;
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Reads the next <see cref="StringToken"/> object.
        /// </summary>
        /// <returns>The next <see cref="StringToken"/> object.</returns>
        public StringToken Read() {
            if (peeked != null) {
                var tok = peeked;
                peeked = null;
                return tok;
            }

            ReadNext:

            char? chr;

            if (!Peek(out chr) || !chr.HasValue)
                return CreateToken(StringTokenKind.EndOfFile, string.Empty);

            switch (chr) {
                case ' ':
                case '\t':
                    if (IgnoreWhitespace) {
                        ReadWhitespace();
                        goto ReadNext;
                    }

                    return ReadWhitespace();
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadNumber();

                case '\r':
                    Start();

                    SkipPos();

                    if (Peek(out chr) && chr == '\n') SkipPos();

                    Line++;
                    Column = 1;

                    return CreateToken(StringTokenKind.EndOfLine);
                case '\n':
                    Start();

                    SkipPos();

                    Line++;
                    Column = 1;

                    return CreateToken(StringTokenKind.EndOfLine);
                case '"':
                case '\'':

                    return ReadString();
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                    Start();

                    SkipPos();

                    return CreateToken(StringTokenKind.Brackets);
                default:

                    if (char.IsLetter(chr.Value) || chr == '_')
                        return ReadWord();

                    if (char.IsSymbol(chr.Value)) {
                        Start();
                        SkipPos();
                        return CreateToken(StringTokenKind.Symbol);
                    }

                    Start();
                    SkipPos();
                    return CreateToken(StringTokenKind.Unknown);
            }
        }

        #endregion

        #region . ReadAll .

        /// <summary>
        /// Reads all tokens in the <see cref="StringTokenizer"/> object.
        /// </summary>
        /// <returns>A token array containing all tokens in the current tokenizer.</returns>
        public StringToken[] ReadAll() {
            var items = new List<StringToken>();

            StringToken token;
            while ((token = Read()).Kind != StringTokenKind.EndOfFile)
                items.Add(token);

            return items.ToArray();
        }

        #endregion

        #region . ReadNumber .

        protected StringToken ReadNumber() {
            Start();
            SkipPos();

            char? chr;

            while (Peek(out chr)) {
                if (!chr.HasValue)
                    break;
                if (char.IsDigit(chr.Value) || chr == '.' || chr == ',')
                    SkipPos();
                else
                    break;
            }

            return CreateToken(StringTokenKind.Number);
        }

        #endregion

        #region . ReadString .

        protected StringToken ReadString() {
            Start();

            char? chr;
            char? open;

            Peek(out open);

            SkipPos();

            while (Peek(out chr)) {
                if (!chr.HasValue)
                    break;

                if (chr == '\r') {
                    SkipPos();

                    if (Peek(out chr) && chr == '\n') SkipPos();

                    Line++;
                    Column = 1;
                    continue;
                }
                if (chr == '\n') {
                    SkipPos();

                    Line++;
                    Column = 1;
                    continue;
                }

                // TODO: Support for \' \" escapes... I'm tired to implement now - Knuppe

                if (chr == '\'') {
                    SkipPos();

                    // check for "" or '' escapes
                    if (Peek(out chr) && chr == open) {
                        SkipPos();
                        continue;
                    }

                    break;
                }

                if (chr == open) {
                    SkipPos();
                    break;
                }

                SkipPos();
            }

            return new StringToken(
                StringTokenKind.QuotedString,
                Value.Substring(startPos + 1, (Position - startPos) - 2), startLine, startCol);
        }

        #endregion

        #region . ReadWhitespace .

        /// <summary>
        /// Reads the whitespace token.
        /// </summary>
        /// <returns>The whitespace token.</returns>
        protected StringToken ReadWhitespace() {
            Start();

            SkipPos();

            char? chr;

            while (Peek(out chr)) {
                if (chr == ' ' || chr == '\t')
                    SkipPos();
                else
                    break;
            }

            return CreateToken(StringTokenKind.WhiteSpace);
        }

        #endregion

        #region . ReadWord .

        protected StringToken ReadWord() {
            char? chr;
            Start();

            SkipPos();

            while (Peek(out chr)) {
                if (!chr.HasValue)
                    break;

                if (char.IsLetter(chr.Value) || chr == '_')
                    SkipPos();
                else
                    break;
            }

            return CreateToken(StringTokenKind.Word);
        }

        #endregion

        #region . Start .

        protected void Start() {
            startCol = Column;
            startLine = Line;
            startPos = Position;
        }

        #endregion

        #endregion
    }
}