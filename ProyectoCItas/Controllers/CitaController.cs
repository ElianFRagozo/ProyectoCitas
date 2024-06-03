using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCItas.Models;
using ProyectoCItas.Services;
using System.Globalization;

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
                return BadRequest("Error de validación: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            try
            {
                // Validación y parseo de la fecha y hora
                if (!DateTime.TryParseExact(dto.Dia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                {
                    return BadRequest("El formato de la fecha es incorrecto. Use el formato 'yyyy-MM-dd'.");
                }

                var citasExistentes = await _citaService.GetCitasPorDoctorYHoraAsync(dto.MedicoId, dto.Hora);

                // Verificar si ya existe una cita a la misma hora con el mismo doctor
                if (citasExistentes.Any())
                {
                    return BadRequest("Ya existe una cita para la misma hora con el mismo doctor.");
                }
                else
                {
                    var cita = new Cita
                    {
                        MedicoId = dto.MedicoId,
                        PacienteId = dto.PacienteId,
                        Hora = dto.Hora, // Asignar la hora como string
                        Dia = fecha,
                        Especialidad = dto.Especialidad,
                        Estado = "Confirmada"
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

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCita(string id, CitaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error de validación: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            try
            {
                // Obtener la cita existente
                var citaExistente = await _citaService.GetCitaByIdAsync(id);

                if (!DateTime.TryParse(dto.Dia, out DateTime dia))
                {
                    return BadRequest("El formato de fecha es incorrecto.");
                }

                if (citaExistente == null)
                {
                    return NotFound("No se encontró la cita.");
                }

                // Actualizar los campos relevantes de la cita existente
                citaExistente.MedicoId = dto.MedicoId;
                citaExistente.PacienteId = dto.PacienteId;
                citaExistente.Hora = dto.Hora;
                citaExistente.Dia = dia;
                citaExistente.Especialidad = dto.Especialidad;

                // Guardar la cita actualizada
                await _citaService.UpdateCitaAsync(citaExistente);

                return Ok("Cita actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarCita(string id)
        {
            try
            {
                // Obtener la cita existente
                var citaExistente = await _citaService.GetCitaByIdAsync(id);

                if (citaExistente == null)
                {
                    return NotFound("No se encontró la cita.");
                }

                // Cambiar el estado de la cita a "Cancelada"
                citaExistente.Estado = "Cancelada";

                // Guardar la cita actualizada
                await _citaService.UpdateCitaAsync(citaExistente);

                return Ok("Cita cancelada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno500 Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCitas()
        {
            var citas = await _citaService.GetCitaAsync();
            return Ok(citas);
        }



    }
}
