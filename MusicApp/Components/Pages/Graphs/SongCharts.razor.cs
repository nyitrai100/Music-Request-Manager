using Microsoft.AspNetCore.Components;
using Blazorise.Charts;
using DatabaseLayer.DbTables;

namespace MusicApp.Components.Pages.Graphs
{
    public partial class SongCharts : ComponentBase
    {
        [Parameter] public IEnumerable<Songs>? DiagramData { get; set; }
        private BarChart<double>? _topSongsChart;
        private PieChart<int>? _statusChart;
        private LineChart<int>? _timelineChart;
        private IEnumerable<Songs>? _lastDiagramData;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (DiagramData != null && DiagramData.Any() && DiagramData != _lastDiagramData)
            {
                _lastDiagramData = DiagramData;
                if (_topSongsChart != null && _statusChart != null && _timelineChart != null)
                {
                    await BuildTopSongsChart();
                    await BuildStatusChart();
                    await BuildTimelineChart();
                }
            }
        }

        private async Task BuildTopSongsChart()
        {
            var data = DiagramData!
                .GroupBy(s => s.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            await _topSongsChart!.Clear();
            await _topSongsChart.AddLabels(data.Select(x => (object)x.Title).ToArray());

            var colors = data.Select(_ => "rgba(54, 162, 235, 0.7)").ToList();
            await _topSongsChart!.AddDataSet(new BarChartDataset<double>
            {
                Label = "Requests",
                Data = data.Select(x => (double)x.Count).ToList(),
                BackgroundColor = colors
            });

            var options = new BarChartOptions
            {
                Responsive = true,
                Plugins = new ChartPlugins
                {
                    Legend = new ChartLegend
                    {
                        Labels = new ChartLegendLabel { Color = "white" }
                    }
                },
                Scales = new ChartScales
                {
                    X = new ChartAxis { Ticks = new ChartAxisTicks { Color = "white" } },
                    Y = new ChartAxis { Ticks = new ChartAxisTicks { Color = "white", Precision = 0, StepSize = 1 } }
                }
            };

            await _topSongsChart!.SetOptions(options);
        }

        private async Task BuildStatusChart()
        {
            var data = DiagramData!
                .GroupBy(s => s.Status!.Name)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            await _statusChart!.Clear();
            await _statusChart.AddLabels(data.Select(x => (object)x.Status).ToArray());

            var colors = data.Select(d => d.Status switch
            {
                "Accepted" => "rgba(0, 128, 0, 0.7)",
                "Rejected" => "rgba(255, 0, 0, 0.7)",
                "Pending"  => "rgba(255, 206, 0, 0.7)",
                _ => "rgba(128, 128, 128, 0.7)"
            }).ToList();

            await _statusChart.AddDataSet(new PieChartDataset<int>
            {
                Data = data.Select(x => x.Count).ToList(),
                BackgroundColor = colors
            });

            var options = new PieChartOptions
            {
                Responsive = true,
                Plugins = new ChartPlugins
                {
                    Legend = new ChartLegend
                    {
                        Labels = new ChartLegendLabel { Color = "white" }
                    }
                }
            };

            await _statusChart.SetOptions(options);
        }

        private async Task BuildTimelineChart()
        {
            var data = DiagramData!
                .GroupBy(s => s.RequestedTime.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd MMM"),
                    Count = g.Count()
                })
                .ToList();

            await _timelineChart!.Clear();
            await _timelineChart.AddLabels(data.Select(x => (object)x.Date).ToArray());

            await _timelineChart.AddDataSet(new LineChartDataset<int>
            {
                Label = "Requests per day",
                Data = data.Select(x => x.Count).ToList(),
                Fill = false,
                BorderColor = "rgba(75, 192, 192, 1)",
                BackgroundColor = "rgba(75, 192, 192, 0.2)",
                PointBackgroundColor = "rgba(255, 99, 132, 1)"
            });

            var options = new LineChartOptions
            {
                Responsive = true,
                Plugins = new ChartPlugins
                {
                    Legend = new ChartLegend
                    {
                        Labels = new ChartLegendLabel { Color = "white" }
                    }
                },
                Scales = new ChartScales
                {
                    X = new ChartAxis { Ticks = new ChartAxisTicks { Color = "white" } },
                    Y = new ChartAxis { Ticks = new ChartAxisTicks { Color = "white" } }
                }
            };

            await _timelineChart.SetOptions(options);
        }
    }
}