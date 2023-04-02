using System;
using System.Linq;
using Windows.Management.Deployment;

namespace ComApp.Wpf2
{
	public static class PackageHelper
	{
		public static string? GetFamilyName(string identityName)
		{
			var package = new PackageManager()
				.FindPackagesForUser(string.Empty)
				.Reverse()
				.FirstOrDefault(x => string.Equals(x.Id.Name, identityName, StringComparison.OrdinalIgnoreCase));

			return package?.Id.FamilyName;
		}
	}
}
