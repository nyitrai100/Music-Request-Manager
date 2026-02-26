using System.Net;
using Microsoft.AspNetCore.Components;
using MusicApp.Models;
using DatabaseLayer.DbTables;
using MusicApp.Services;
using DbDj = DatabaseLayer.DbTables.Dj;

namespace MusicApp.Components.Pages.Performances
{
    public partial class EditPerformance : ComponentBase
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Parameter] public DjSets? PerformanceToEdit { get; set; }
        [Inject] private AdminService AdminService { get; set; } = default!;

        private EditPerformanceModel _model = new();
        private string? _error;
    
        protected override async Task OnParametersSetAsync()
        {
            _error = null;
            _model.Dj = await AdminService.GetAllDjs();
            _model.Clubs = await AdminService.GetAllClubs();
            
            if (PerformanceToEdit != null)
            {
                _model.Id = PerformanceToEdit.Id;
                _model.DjId = PerformanceToEdit.DjId;
                _model.ClubId = PerformanceToEdit.ClubId;
                _model.PerformanceTimeStarts = PerformanceToEdit.PerformanceTimeStarts;
                _model.PerformanceTimeEnds = PerformanceToEdit.PerformanceTimeEnds;
            }
        }

        private async Task HandleSubmit()
        {
            if (_model.PerformanceTimeEnds <= _model.PerformanceTimeStarts)
            {
                _error = "End time must be after start time";
                return;
            }

            var result = await AdminService.EditPerformance(_model);

            if (result.Success)
                await OnSaved.InvokeAsync();
            else
                _error = result.Error;
        }
    }
}