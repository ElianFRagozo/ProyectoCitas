using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCItas.Models;
using ProyectoCItas.Services;

namespace ProyectoCitas.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : Controller
    {

        private readonly CitaService _citaService;

        public CitaController(CitaService citaService)
        {
            _citaService = citaService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CrearCita(CitaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState + "PRIMER FILTRO");
            }
            try
            {
                var resultado = await _citaService.GetCitaEstadoAsync(dto.MedicoId, dto.dia, dto.HoraInicio);
                if (resultado != null)
                {
                    return BadRequest("No hay horarios disponibles para la fecha y hora seleccionadas.");
                }
                else
                {
                    var cita = new Cita
                    {
                        MedicoId = dto.MedicoId,
                        PacienteId = dto.PacienteId,
                        HoraInicio = dto.HoraInicio,
                        HoraFin = dto.HoraFin,
                        Estado = "registrado"
                    };

                    await _citaService.CreateCitaAsync(cita);
                }

                return Ok("Cita creada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCitas()
        {
            var citas = await _citaService.GetCitasAsync();
            return Ok(citas);
        }

    }
}
