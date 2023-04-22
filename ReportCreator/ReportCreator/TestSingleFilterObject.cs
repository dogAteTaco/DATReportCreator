using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportCreator
{
	internal class TestSingleFilterObject
	{
		[DisplayName("Categoria")]
		public int category { get; set; }

		public List<TestObject> objects { get; set; }

		public TestSingleFilterObject(int category, List<TestObject> objects)
		{
			this.category = category;
			this.objects = objects;
		}
	}
}
