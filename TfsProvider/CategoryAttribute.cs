using System;

namespace Interop
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class CategoryAttribute : Attribute
	{
		public Guid Category { get; private set; }

		public CategoryAttribute(string guid)
		{
			Category = new Guid(guid);
		}
	}
}
