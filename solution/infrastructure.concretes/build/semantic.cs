//-----------------------------------------------------------------------------
// <copyright file="semantic.cs" company="ImaginaryRealities">
// Copyright 2013 ImaginaryRealities, LLC
// </copyright>
// <summary>
// This file implements the SemanticVersion class. The SemanticVersion class
// represents a semantic version number for a program.
// </summary>
// <license>
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including but without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
// </license>
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;

namespace reexmonkey.infrastructure.build.concretes
{
    /// <summary>
    /// Stores a semantic version number for a program.
    /// </summary>
    [Serializable]
    public sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        /// <summary>
        /// A regular expression to detect whether a string contains only
        /// digits.
        /// </summary>
        private static readonly Regex AllDigitsRegex = new Regex(
            @"[0-9]+", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// The regular expression to use to parse a semantic version number.
        /// </summary>
        private static readonly Regex SemanticVersionRegex =
            new Regex(
                @"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-(?<prerelease>[A-Za-z0-9\-\.]+))?(\+(?<build>[A-Za-z0-9\-\.]+))?$",
                RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">
        /// The semantic version number to be parsed.
        /// </param>
        public SemanticVersion(string version)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(version));
            Contract.Ensures(0 <= this.MajorVersion);
            Contract.Ensures(0 <= this.MinorVersion);
            Contract.Ensures(0 <= this.PatchVersion);

            var match = SemanticVersionRegex.Match(version);
            if (!match.Success)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture, reexmonkey.infrastructure.concretes.Properties.Resources.InvalidSemanticVersion, version);
                throw new ArgumentException(message, "version");
            }

            this.MajorVersion = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);
            this.MinorVersion = int.Parse(match.Groups["minor"].Value, CultureInfo.InvariantCulture);
            this.PatchVersion = int.Parse(match.Groups["patch"].Value, CultureInfo.InvariantCulture);
            this.PrereleaseVersion = match.Groups["prerelease"].Success ? match.Groups["prerelease"].Value : null;
            this.BuildVersion = match.Groups["build"].Success ? match.Groups["build"].Value : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="majorVersion">
        /// The major version number.
        /// </param>
        /// <param name="minorVersion">
        /// The minor version number.
        /// </param>
        /// <param name="patchVersion">
        /// The patch version number.
        /// </param>
        public SemanticVersion(int majorVersion, int minorVersion, int patchVersion)
        {
            Contract.Requires<ArgumentException>(0 <= majorVersion);
            Contract.Requires<ArgumentException>(0 <= minorVersion);
            Contract.Requires<ArgumentException>(0 <= patchVersion);
            Contract.Ensures(0 <= this.MajorVersion);
            Contract.Ensures(0 <= this.MinorVersion);
            Contract.Ensures(0 <= this.PatchVersion);

            this.MajorVersion = majorVersion;
            this.MinorVersion = minorVersion;
            this.PatchVersion = patchVersion;
        }

        /// <summary>
        /// Gets the build number.
        /// </summary>
        /// <value>
        /// The value of this property is a string containing the build
        /// identifier for the version number.
        /// </value>
        public string BuildVersion { get; private set; }

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the major
        /// version number.
        /// </value>
        public int MajorVersion { get; private set; }

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the minor
        /// version number.
        /// </value>
        public int MinorVersion { get; private set; }

        /// <summary>
        /// Gets the patch version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the patch
        /// version number.
        /// </value>
        public int PatchVersion { get; private set; }

        /// <summary>
        /// Gets the pre-release version component.
        /// </summary>
        /// <value>
        /// The value of this property is a string containing the pre-release
        /// identifier.
        /// </value>
        public string PrereleaseVersion { get; private set; }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects for equality.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second semantic version object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or <b>false</b> if the
        /// objects are not equal.
        /// </returns>
        public static bool operator ==(SemanticVersion version, SemanticVersion other)
        {
            if (ReferenceEquals(null, version))
            {
                return ReferenceEquals(null, other);
            }

            return version.Equals(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects for equality.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are not equal, or <b>false</b> if the
        /// objects are equal.
        /// </returns>
        public static bool operator !=(SemanticVersion version, SemanticVersion other)
        {
            if (ReferenceEquals(null, version))
            {
                return !ReferenceEquals(null, other);
            }

            return !version.Equals(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects to determine if
        /// the first object logically precedes the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> precedes
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator <(SemanticVersion version, SemanticVersion other)
        {
            Contract.Requires<ArgumentNullException>(null != version);
            Contract.Requires<ArgumentNullException>(null != other);

            return 0 > version.CompareTo(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> object to determine if
        /// the first object logically precedes the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> follows
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator >(SemanticVersion version, SemanticVersion other)
        {
            Contract.Requires<ArgumentNullException>(null != version);
            Contract.Requires<ArgumentNullException>(null != version);

            return 0 < version.CompareTo(other);
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to this object.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// This instance precedes <paramref name="obj"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// This instance occurs in the same position in the sort order as
        /// <paramref name="obj"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// This instance follows <paramref name="obj"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not a <see cref="SemanticVersion"/>
        /// object.
        /// </exception>
        public int CompareTo(object obj)
        {
            var otherVersion = obj as SemanticVersion;
            if (null == otherVersion)
            {
                throw new ArgumentException(reexmonkey.infrastructure.concretes.Properties.Resources.ObjectIsNotASemanticVersion, "obj");
            }

            return this.CompareTo(otherVersion);
        }

        /// <summary>
        /// Compares the current object with another
        /// <see cref="SemanticVersion"/> object.
        /// </summary>
        /// <param name="other">
        /// The other <see cref="SemanticVersion"/> object to compare to this
        /// instance.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// This instance precedes <paramref name="other"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// This instance occurs in the same position in the sort order as
        /// <paramref name="other"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// This instance follows <paramref name="other"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(SemanticVersion other)
        {
            if (null == other)
            {
                throw new ArgumentNullException("other");
            }

            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            var result = this.MajorVersion.CompareTo(other.MajorVersion);
            if (0 == result)
            {
                result = this.MinorVersion.CompareTo(other.MinorVersion);
                if (0 == result)
                {
                    result = this.PatchVersion.CompareTo(other.PatchVersion);
                    if (0 == result)
                    {
                        result = ComparePrereleaseVersions(this.PrereleaseVersion, other.PrereleaseVersion);
                        if (0 == result)
                        {
                            result = CompareBuildVersions(this.BuildVersion, other.BuildVersion);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Compares this instance to another object for equality.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to this instance.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or <b>false</b> if the
        /// objects are not equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as SemanticVersion;
            return null != other ? this.Equals(other) : false;
        }

        /// <summary>
        /// Compares this instance to another <see cref="SemanticVersion"/>
        /// object for equality.
        /// </summary>
        /// <param name="other">
        /// The <see cref="SemanticVersion"/> object to compare to this
        /// instance.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or false if the objects are
        /// not equal.
        /// </returns>
        public bool Equals(SemanticVersion other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion
                   && this.PatchVersion == other.PatchVersion && this.PrereleaseVersion == other.PrereleaseVersion
                   && this.BuildVersion == other.BuildVersion;
        }

        /// <summary>
        /// Calculates the hash code for the object.
        /// </summary>
        /// <returns>
        /// The hash code for the object.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = (hashCode * 37) + this.MajorVersion;
            hashCode = (hashCode * 37) + this.MinorVersion;
            hashCode = (hashCode * 37) + this.PatchVersion;
            hashCode = null != this.PrereleaseVersion
                           ? (hashCode * 37) + this.PrereleaseVersion.GetHashCode()
                           : hashCode;
            hashCode = null != this.BuildVersion ? (hashCode * 37) + this.BuildVersion.GetHashCode() : hashCode;
            return hashCode;
        }

        /// <summary>
        /// Returns the string representation of the semantic version number.
        /// </summary>
        /// <returns>
        /// The semantic version number.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.{2}{3}{4}",
                this.MajorVersion,
                this.MinorVersion,
                this.PatchVersion,
                !string.IsNullOrEmpty(this.PrereleaseVersion) ? "-" + this.PrereleaseVersion : string.Empty,
                !string.IsNullOrEmpty(this.BuildVersion) ? "+" + this.BuildVersion : string.Empty);
        }

        /// <summary>
        /// Compares two build version values to determine precedence.
        /// </summary>
        /// <param name="identifier1">
        /// The first identifier to compare.
        /// </param>
        /// <param name="identifier2">
        /// The second identifier to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> precedes
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// The identifiers occur in the same position in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> follows
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        private static int CompareBuildVersions(string identifier1, string identifier2)
        {
            var result = 0;
            var hasIdentifier1 = !string.IsNullOrEmpty(identifier1);
            var hasIdentifier2 = !string.IsNullOrEmpty(identifier2);
            if (hasIdentifier1 && !hasIdentifier2)
            {
                result = 1;
            }
            else if (!hasIdentifier1 && hasIdentifier2)
            {
                result = -1;
            }
            else if (hasIdentifier1)
            {
                var dotDelimiter = new[] { '.' };
                var parts1 = identifier1.Split(dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var parts2 = identifier2.Split(dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var max = Math.Max(parts1.Length, parts2.Length);
                for (var i = 0; i < max; i++)
                {
                    if (i == parts1.Length && i != parts2.Length)
                    {
                        result = -1;
                        break;
                    }

                    if (i != parts1.Length && i == parts2.Length)
                    {
                        result = 1;
                        break;
                    }

                    var part1 = parts1[i];
                    var part2 = parts2[i];
                    if (AllDigitsRegex.IsMatch(part1) && AllDigitsRegex.IsMatch(part2))
                    {
                        var value1 = int.Parse(part1, CultureInfo.InvariantCulture);
                        var value2 = int.Parse(part2, CultureInfo.InvariantCulture);
                        result = value1.CompareTo(value2);
                    }
                    else
                    {
                        result = string.Compare(part1, part2, StringComparison.Ordinal);
                    }

                    if (0 != result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Compares two pre-release version values to determine precedence.
        /// </summary>
        /// <param name="identifier1">
        /// The first identifier to compare.
        /// </param>
        /// <param name="identifier2">
        /// The second identifier to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> precedes
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// The identifiers occur in the same position in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> follows
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        private static int ComparePrereleaseVersions(string identifier1, string identifier2)
        {
            var result = 0;
            var hasIdentifier1 = !string.IsNullOrEmpty(identifier1);
            var hasIdentifier2 = !string.IsNullOrEmpty(identifier2);
            if (hasIdentifier1 && !hasIdentifier2)
            {
                result = -1;
            }
            else if (!hasIdentifier1 && hasIdentifier2)
            {
                result = 1;
            }
            else if (hasIdentifier1)
            {
                var dotDelimiter = new[] { '.' };
                var parts1 = identifier1.Split(dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var parts2 = identifier2.Split(dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var max = Math.Max(parts1.Length, parts2.Length);
                for (var i = 0; i < max; i++)
                {
                    if (i == parts1.Length && i != parts2.Length)
                    {
                        result = -1;
                        break;
                    }

                    if (i != parts1.Length && i == parts2.Length)
                    {
                        result = 1;
                        break;
                    }

                    var part1 = parts1[i];
                    var part2 = parts2[i];
                    if (AllDigitsRegex.IsMatch(part1) && AllDigitsRegex.IsMatch(part2))
                    {
                        var value1 = int.Parse(part1, CultureInfo.InvariantCulture);
                        var value2 = int.Parse(part2, CultureInfo.InvariantCulture);
                        result = value1.CompareTo(value2);
                    }
                    else
                    {
                        result = string.Compare(part1, part2, StringComparison.Ordinal);
                    }

                    if (0 != result)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}