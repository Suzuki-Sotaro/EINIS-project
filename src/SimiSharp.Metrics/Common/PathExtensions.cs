// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathExtensions.cs" company="Reimers.dk">
//   Copyright © 
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PathExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;

namespace SimiSharp.CodeAnalysis.Common
{
	public static class PathExtensions
	{
		public static string GetParentFolder(this string path)
		{
			if (string.IsNullOrWhiteSpace(value: path))
			{
				return null;
			}

			if (File.Exists(path: path))
			{
				var directory = new FileInfo(fileName: path).Directory;
				return directory?.FullName ?? string.Empty;
			}

			var dir = new DirectoryInfo(path: path);

			return dir.Parent?.FullName ?? string.Empty;
		}

		public static string GetFullPath(this string path)
		{
			return Path.GetFullPath(path: path);
		}

		public static string CombineWith(this string path, string other)
		{
			return Path.Combine(path1: path, path2: other);
		}

		public static string GetFileNameWithoutExtension(this string path)
		{
			var fileName = Path.GetFileNameWithoutExtension(path: path);

			return string.IsNullOrWhiteSpace(value: fileName) ? string.Empty : fileName;
		}

		public static string ChangeExtension(this string path, string extension)
		{
			var fileName = Path.ChangeExtension(path: path, extension: extension);

			return string.IsNullOrWhiteSpace(value: fileName) ? string.Empty : fileName;
		}

		public static string GetLowerCaseExtension(this string path)
		{
			var extension = Path.GetExtension(path: path);

			return string.IsNullOrWhiteSpace(value: extension) ? string.Empty : extension.ToLowerInvariant();
		}

		public static string GetLowerCaseFullPath(this string path)
		{
			if (string.IsNullOrWhiteSpace(value: path))
			{
				return string.Empty;
			}

			var fullPath = Path.GetFullPath(path: path);

			return string.IsNullOrWhiteSpace(value: fullPath) ? string.Empty : fullPath.ToLowerInvariant();
		}

		public static string GetPathRelativeTo(this string path, string other)
		{
			if (string.IsNullOrWhiteSpace(value: path) || string.IsNullOrWhiteSpace(value: other))
			{
				return string.Empty;
			}

			var pathUri = new Uri(uriString: path);

			if (!other.EndsWith(value: Path.DirectorySeparatorChar.ToString(provider: CultureInfo.InvariantCulture)))
			{
				other += Path.DirectorySeparatorChar;
			}

			var folderUri = new Uri(uriString: other);
			return Uri.UnescapeDataString(stringToUnescape: folderUri.MakeRelativeUri(uri: pathUri).ToString().Replace(oldChar: '/', newChar: Path.DirectorySeparatorChar));
		}
	}
}