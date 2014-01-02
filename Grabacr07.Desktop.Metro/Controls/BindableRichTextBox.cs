using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Grabacr07.Desktop.Metro.Controls
{
	public class BindableRichTextBox : RichTextBox
	{
		static BindableRichTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BindableRichTextBox), new FrameworkPropertyMetadata(typeof(BindableRichTextBox)));
		}


		#region TextTemplates 依存関係プロパティ

		public DataTemplateCollection TextTemplates
		{
			get { return (DataTemplateCollection)GetValue(TextTemplatesProperty); }
			set { SetValue(TextTemplatesProperty, value); }
		}

		public static readonly DependencyProperty TextTemplatesProperty =
			DependencyProperty.Register("TextTemplates", typeof(DataTemplateCollection), typeof(BindableRichTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnNeedUpdate));

		#endregion

		#region TextSource 依存関係プロパティ

		public IEnumerable<object> TextSource
		{
			get { return (IEnumerable<object>)GetValue(TextSourceProperty); }
			set { SetValue(TextSourceProperty, value); }
		}
		public static readonly DependencyProperty TextSourceProperty =
			DependencyProperty.Register("TextSource", typeof(IEnumerable<object>), typeof(BindableRichTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnNeedUpdate));

		private static void OnNeedUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = d as BindableRichTextBox;
			if (instance == null) return;

			instance.Update();
		}

		#endregion


		public BindableRichTextBox()
		{
			TextTemplates = new DataTemplateCollection();
			Loaded += (sender, e) => Update();
		}

		private IEnumerable<BlockHolder> CreateTemplateInstance(IEnumerable<object> textSourcePart)
		{
			return textSourcePart.Select(o =>
			{
				BlockHolder result;

				var template = TextTemplates.FirstOrDefault(dt => (Type)dt.DataType == o.GetType());
				if (template == null)
				{
					var paragraph = new Paragraph();
					paragraph.Inlines.Add(new Run(o.ToString()));
					result = new BlockHolder { Blocks = new BlockSimpleCollection(new[] { paragraph }) };
				}
				else
				{
					result = (BlockHolder)template.LoadContent();
					result.DataContext = o;
					foreach (var block in result.Blocks)
					{
						block.DataContext = o;
					}
				}
				return result;
			});
		}

		private void Update()
		{
			this.Document.Blocks.Clear();

			if (this.TextSource == null) return;

			foreach (var block in this.CreateTemplateInstance(TextSource).SelectMany(holder => holder.Blocks))
			{
				this.Document.Blocks.Add(block);
			}
		}

	}

	[ContentProperty("Blocks")]
	public class BlockHolder : FrameworkElement
	{
		public BlockHolder() { Blocks = new BlockSimpleCollection(); }

		public BlockSimpleCollection Blocks { get; set; }
	}

	public class DataTemplateCollection : List<DataTemplate>
	{
		public DataTemplateCollection() { }
		public DataTemplateCollection(IEnumerable<DataTemplate> source)
		{
			AddRange(source);
		}
	}

	public class BlockSimpleCollection : List<Block>
	{
		public BlockSimpleCollection() { }
		public BlockSimpleCollection(IEnumerable<Block> source)
		{
			AddRange(source);
		}
	}
}
