using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportCreator
{
	public class TestObject
	{
		[DisplayName("Identificador")]
		public int id {  get; set; }
		[DisplayName("Nombre")]
		public string name { get; set; }
		[DisplayName("Descripción")]
		public string description { get; set; }
		[DisplayName("Fecha")]
		public DateTime created { get; set; }
		[DisplayName("Precio")]
		public float price { get; set; }
		[DisplayName("Dato")]
		public String dato { get; set; }
		public TestObject(int id, string name, string description, float price, DateTime created)
		{
			this.id = id;
			this.name = name;
			this.description = description;
			this.price = price;
			this.created = created;
		}
	}
}
