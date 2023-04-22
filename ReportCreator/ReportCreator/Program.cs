using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using ReportCreator;
using System.ComponentModel;
using System.Reflection;

internal class Program
{
	private static void Main(string[] args)
	{
		List<TestObject> list = new List<TestObject>();

		for (int i = 0; i < 20; i++)
			list.Add(new TestObject(i, $"Object {i}", "UNO", i * 1.07f, DateTime.Now));
		for (int i = 0; i < 40; i++)
			list.Add(new TestObject(i, $"Objetods {i}", "DOS", i * 1.07f, DateTime.Now));
		for (int i = 0; i < 10; i++)
			list.Add(new TestObject(i, $"Oxido {i}", "TRES", i * 1.07f, DateTime.Now));


		var document = createSimpleReport(list,"Reporte de Prueba Simple", "dogAteTaco", "ReportCreator");
		document = createSingleGroupingReport(list,"description","Reporte Seccionado", "dogAteTaco", "ReportCreator");

		document.GeneratePdf("X:\\ITM\\reporteee.pdf");
		document.ShowInPreviewer();
	}

	private static Document createSimpleReport<T>(List<T> objects,String reportTitle, String company, String software)
	{
		uint currentRow = 1;
		var document = Document.Create(container =>
		{
			container.Page(page =>
			{
				setTemplate(ref page,reportTitle,"dogAteTaco","ReportCreator");

				page.Content()
					.PaddingVertical(1, Unit.Centimetre)
				.Column(column =>
				{
					column.Item().Table(table =>
					{
						currentRow = createSimpleTable(table, objects, currentRow);
					});
				});
			});
		});
		return document;
	}

	private static Document createSingleGroupingReport<T>(List<T> objects, String groupBy, String reportTitle, String company, String software, List<String> filterOrder = null)
	{
		uint currentRow = 1;
		var document = Document.Create(container =>
		{
			container.Page(page =>
			{
				setTemplate(ref page, reportTitle,company,software);

				page.Content()
					.PaddingVertical(1, Unit.Centimetre)
				.Column(column =>
				{
					column.Item().Table(table =>
					{
						var queryables = objects.AsQueryable();
						HashSet<Object> set = new HashSet<Object>();
						foreach (var item in objects)
						{
							set.Add(item.GetType().GetProperty(groupBy).GetValue(item));
						}
						foreach (var item in set)
						{
							table.Cell().Row(currentRow).Column(1).Text("");
							currentRow++;
							table.Cell().Row(currentRow).Column(1).Text(text =>
							{
								text.Span($"{objects.First().GetType().GetProperty(groupBy).GetCustomAttribute<DisplayNameAttribute>().DisplayName}:  ");
								text.Span($"{item}").Bold().Italic();
							});

							currentRow++;
							currentRow = createSimpleTable(table, objects.Where(i => i.GetType().GetProperty(groupBy).GetValue(i) == item).ToList(), currentRow, 1);
						}
					});
				});
			});
		});
		return document;
	}


	private static void setTemplate(ref PageDescriptor page,String reportTitle, String company, String software)
	{
		page.Size(PageSizes.Letter);
		page.Margin(1, Unit.Centimetre);
		page.PageColor(Colors.White);
		page.DefaultTextStyle(x => x.FontSize(11));

		//Header
		page.Header().Column(column =>
		{
			column.Item().Table(table =>
			{
				table.ColumnsDefinition(columns =>
				{
					columns.ConstantColumn(100);
					columns.RelativeColumn();
					columns.ConstantColumn(60);
				});
				table.Cell().Row(1).Column(1).Text("Logo");
				table.Cell().Row(1).Column(2).Text(text =>
				{
					text.Span(company);
					text.AlignCenter();
					text.DefaultTextStyle(s => s.FontSize(12).Bold());
				});
				table.Cell().Row(2).Column(2).Text(text =>
				{
					text.Span(software);
					text.AlignCenter();
					text.DefaultTextStyle(s => s.FontSize(12).Bold());
				});
				table.Cell().Row(3).Column(2).Text(text =>
				{
					text.Span(reportTitle);
					text.AlignCenter();
					text.DefaultTextStyle(s => s.FontSize(15).Bold());
				});
				table.Cell().Row(1).Column(3).Text(text =>
				{
					text.Span(DateTime.Now.ToString("dd/MM/yy"));
					text.DefaultTextStyle(s => s.Italic());
				});
				table.Cell().Row(2).Column(3).Text(text =>
				{
					text.Span(DateTime.Now.ToString("HH:mm"));
					text.DefaultTextStyle(s => s.Italic());
				});
				table.Cell().Row(3).Column(3).Text(x =>
				{
					x.Span("Pág. ");
					x.CurrentPageNumber();
				});
			});
		});
	}
	/// <summary>
	/// Creates a table in the PDF by receiving a TableDescriptor to fill and the List of objects
	/// that will fill it.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="table"></param>
	/// <param name="objects"></param>
	static uint createSimpleTable<T>(TableDescriptor table,List<T> objects, uint currentRow, uint groupingColumnsn = 0)
	{
		//Does nothing if the list is empty
		uint currentColumn = 1;
		if (objects == null || objects.Count == 0) return currentRow;
		currentRow = printHeaders(table, objects, currentRow, groupingColumnsn);
		//table.ColumnsDefinition(columns =>
		//{
		//	foreach(var attribute in objects.First().GetType().GetProperties())
		//	{
		//		columns.RelativeColumn();
		//		table.Cell().Row(currentRow).Column(currentColumn).Text(text =>
		//		{
		//			text.Span(attribute.GetCustomAttribute<DisplayNameAttribute>().DisplayName);
		//			text.DefaultTextStyle(s => s.Bold());
		//		});
				
		//		currentColumn++;
		//	}
		//	currentRow++;
		//});
		
		foreach (var row in objects)
		{
			currentColumn = 1;
			foreach (var column in row.GetType().GetProperties())
			{
				if (column.GetValue(row) != null)
				{
					//if (currentRow % 40 == 0)
					//{
					//	table.Cell().PageBreak();
					//	currentRow++;
					//	currentRow = printHeaders(table, objects, currentRow, groupingColumnsn);
					//	currentRow++;
					//}

					if (groupingColumnsn < currentColumn)
					{
						if (column.GetValue(row).GetType() == typeof(DateTime))
							table.Cell().Row(currentRow).Column(currentColumn - groupingColumnsn).Text(((DateTime)column.GetValue(row)).ToString("dd/MM/yy"));
						else if (column.GetValue(row).GetType() == typeof(float))
							table.Cell().Row(currentRow).Column(currentColumn - groupingColumnsn).Text(((float)column.GetValue(row)).ToString("0.00"));
						else
							table.Cell().Row(currentRow).Column(currentColumn - groupingColumnsn).Text(column.GetValue(row));
					}
					
				}
				currentColumn++;
			}
			currentRow++;
		}
		return currentRow;
	}

	static uint printHeaders<T>(TableDescriptor table, List<T> objects, uint currentRow,uint groupingColumnsn)
	{
		uint currentColumn = 1;
		table.ColumnsDefinition(columns =>
		{
			foreach (var attribute in objects.First().GetType().GetProperties())
			{
				
				if (groupingColumnsn < currentColumn)
				{
					columns.RelativeColumn();
					table.Cell().Row(currentRow).Column(currentColumn - groupingColumnsn).Text(text =>
					{
						text.Span(attribute.GetCustomAttribute<DisplayNameAttribute>().DisplayName);
						text.DefaultTextStyle(s => s.Bold());
					});
				}
				currentColumn++;
			}
		});
		currentRow++;
		return currentRow;
	}

}