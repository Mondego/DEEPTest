using System;

namespace FlowTestAPI
{
	// Example usage
	// [FTTarget(path=p1, arg1)]
	// [FTTarget(path=p2)]
	// [FTTarget(path=p3, "hello")]

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class FTTargetAttribute : Attribute
	{
		private string path;

		public virtual string Path
		{
			get { return path; }
			set { path = value; }
		}

		public FTTargetAttribute (string path)
		{
			this.path = path;
		}
	}
}

