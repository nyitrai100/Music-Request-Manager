using System.Net;
using Microsoft.AspNetCore.Components;
using MusicApp.Models;
using DatabaseLayer.DbTables;
using MusicApp.Services;


namespace MusicApp.Components.Pages.Performances
{
    public partial class CreatePerformance : ComponentBase
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Inject] private AdminService AdminService { get; set; } = default!;

        private CreatePerformanceModel _model = new();
        private string? _error;
        
        protected override async Task OnInitializedAsync()
        {
            _error = null;
            _model.PerformanceTimeStarts = DateTime.Now;
            _model.PerformanceTimeEnds = DateTime.Now.AddHours(1);
            _model.Dj = await AdminService.GetAllDjs();
            _model.Clubs = await AdminService.GetAllClubs();
        }
        
        private async Task HandleSubmit()
        {
            if (_model.PerformanceTimeEnds <= _model.PerformanceTimeStarts)
            {
                _error = "End time must be after start time";
                return;
            }

            var result = await AdminService.CreatePerformance(_model);

            if (result.Success)
                await OnSaved.InvokeAsync();
            else
                _error = result.Error;
        }
    }
}

