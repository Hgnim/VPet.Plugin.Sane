using Panuon.WPF.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.Sane {
	internal class GdPanelAction {
		internal class GdPanelItem {
			internal TextBlock text=new();
			internal TextBlock changeText = new();
			internal ProgressBar progressBar = new();
		}
		/// <summary>
		/// 面板中的条目
		/// </summary>
		GdPanelItem item;
		/// <summary>
		/// 添加条目到面板中
		/// </summary>
		void AddBar(Grid grid) {
			grid.RowDefinitions.Add(new RowDefinition());
			int barIndex = grid.RowDefinitions.Count - 1;

			//text
			grid.Children.Add(item.text);
			Grid.SetRow(item.text, barIndex);
			Grid.SetColumn(item.text, 0);

			//changeText
			item.changeText.Text = "0.00/t";
			item.changeText.HorizontalAlignment = HorizontalAlignment.Right;
			item.changeText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#039BE5"));
			grid.Children.Add(item.changeText);
			Grid.SetRow(item.changeText, barIndex);
			Grid.SetColumn(item.changeText, 4);

			//progressBar
			item.progressBar.Height = 20;
			item.progressBar.VerticalAlignment = VerticalAlignment.Center;
			ProgressBarHelper.SetCornerRadius(item.progressBar, new CornerRadius(10));
			item.progressBar.Background = null;
			item.progressBar.FontSize = 20;
			item.progressBar.Opacity = 1;
			item.progressBar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
			item.progressBar.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BCBCBC"));
			ProgressBarHelper.AddGeneratingPercentTextHandler(item.progressBar, ProgressBar_Change);//进度条更改时调用
			ProgressBarHelper.SetIsPercentVisible(item.progressBar, true);
			grid.Children.Add(item.progressBar);
			Grid.SetRow(item.progressBar, barIndex);
			Grid.SetColumn(item.progressBar, 2);
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="MW">IMainWindow</param>
		/// <param name="item_">条目</param>
		/// <param name="grid">目标Grid</param>
		internal GdPanelAction(IMainWindow MW, GdPanelItem item_,Grid grid) {
			item = item_;
			lastValue = item.progressBar.Value;
			AddBar(grid);
			ChangeForeground(MW);
		}

		internal static Brush GetForeground(IMainWindow MW, double value) =>
			value > 0.5
				? MW.Main.FindResource("SuccessProgressBarForeground") as Brush
				: value > 0.2
					? MW.Main.FindResource("WarningProgressBarForeground") as Brush
					: MW.Main.FindResource("DangerProgressBarForeground") as Brush;
		/// <summary>
		/// 更改进度条前景色
		/// </summary>
		/// <param name="customBr">自定义刷子。如果留空则使用默认</param>
		internal void ChangeForeground(IMainWindow MW,Brush customBr=null) =>
			item.progressBar.Foreground = customBr ?? GetForeground(MW, item.progressBar.Value / 100);
		/// <summary>
		/// 更改进度条时触发<br/>
		/// 用于修改进度条中的文本
		/// </summary>
		private void ProgressBar_Change(object sender, GeneratingPercentTextRoutedEventArgs e) =>
			e.Text = $"{item.progressBar.Value:f2} / {item.progressBar.Maximum:f0}";
		double lastValue = 0;
		/// <summary>
		/// 更改进度条的值
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="changeForeground">更改前景色的函数。如果留空，则执行默认函数</param>
		internal void ProgressBar_Change(IMainWindow MW,double value,Action changeForeground=null) {
			item.progressBar.Value = value;

			double valueDiff = value-lastValue;
			lastValue = value;
			{
				double abs = Math.Abs(valueDiff);
				if (abs > 0.1)
					item.changeText.Text = $"{valueDiff:f1}/t";
				else if (abs > 0.01)
					item.changeText.Text = $"{valueDiff:f2}/t";
				else if (abs > 0.001)
					item.changeText.Text = $"{valueDiff:f3}/t";
				else if (abs != 0)
					item.changeText.Text = $"{valueDiff:f4}/t";
				else
					item.changeText.Text = $"{valueDiff:f0}/t";
			}
			if (changeForeground != null)
				changeForeground.Invoke();
			else
				ChangeForeground(MW);
		}
	}
}
